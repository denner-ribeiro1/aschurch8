using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ASChurchManager.Application.AppServices
{

    public class ArquivosAzureAppService : IArquivosAzureAppService
    {
        private readonly StorageCredentials storageCredentials;
        private readonly CloudStorageAccount storageAccount;
        private readonly IConfiguration _configuration;
        public string AccountName => _configuration["AzureStorage:AccountName"];
        public string AccountKey => _configuration["AzureStorage:AccountKey"];

        public ArquivosAzureAppService(IConfiguration configuration)
        {
            _configuration = configuration;

            storageCredentials = new StorageCredentials(AccountName, AccountKey);
            storageAccount = new CloudStorageAccount(storageCredentials, true);
        }

        public async Task<bool> DeleteSpecificFileAsync(string fileName, string containerName)
        {
            try
            {
                CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);

                if (await container.ExistsAsync())
                {
                    CloudBlob file = container.GetBlobReference(fileName);
                    if (await file.ExistsAsync())
                        await file.DeleteAsync();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<RetornoAzure> DownloadFromStorageAsync(string fileName, string containerName)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = BlobClient.GetContainerReference(containerName);

                if (await container.ExistsAsync())
                {
                    CloudBlob file = container.GetBlobReference(fileName);
                    if (await file.ExistsAsync())
                    {
                        await file.DownloadToStreamAsync(ms);
                        Stream blobStream = (await file.OpenReadAsync());
                        return new RetornoAzure()
                        {
                            BlobStream = blobStream,
                            ContentType = file.Properties.ContentType,
                            Nome = file.Name
                        };
                    }
                }
                return new RetornoAzure();
            }
            catch (System.Exception ex)
            {
                var ret = new RetornoAzure();
                ret.PreencherStatusErro(ex);
                return ret;
            }
        }

        public RetornoAzure DownloadFromUrl(string url)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                var blob = new CloudBlockBlob(new Uri(url), storageAccount.Credentials);

                var blobRequestOptions = new BlobRequestOptions
                {
                    RetryPolicy = new Microsoft.Azure.Storage.RetryPolicies.NoRetry()
                };

                blob.DownloadToStream(ms, null, blobRequestOptions);

                return new RetornoAzure()
                {
                    BlobStream = ms,
                    Nome = blob.Name,
                    ContentType = blob.Properties.ContentType,
                    BlobArray = ms.ToArray()
                };
            }
            catch (Exception ex)
            {
                var ret = new RetornoAzure();
                ret.PreencherStatusErro(ex);
                return ret;
            }
        }

        public async Task<string> RetornaURLArquivo(string fileName, string containerName)
        {
            CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = BlobClient.GetContainerReference(containerName);

            if (await container.ExistsAsync())
            {
                CloudBlob file = container.GetBlobReference(fileName);
                if (await file.ExistsAsync())
                {
                    return file.Uri.AbsoluteUri;
                }
            }
            return "";
        }
        private async Task<string> UploadFileToBlobAsync(Stream fileStream, string fileName, string containerName, string fileMimeType)
        {
            try
            {
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
                await cloudBlobContainer.CreateIfNotExistsAsync();
                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                if (fileName != null && fileStream != null)
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    cloudBlockBlob.Properties.ContentType = fileMimeType;
                    await cloudBlockBlob.UploadFromStreamAsync(fileStream);
                    return cloudBlockBlob.Uri.AbsoluteUri;
                }
                return "";
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string UploadFileToStorage(Stream fileStream, string fileName, string containerName, string fileMimeType)
        {
            try
            {
                var _task = Task.Run(() => UploadFileToBlobAsync(fileStream, fileName, containerName, fileMimeType));
                _task.Wait();

                string fileUrl = _task.Result;
                return fileUrl;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteSpecificContainerAsync(string containerName)
        {
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            if (await container.ExistsAsync())
            {
                await container.FetchAttributesAsync();
                await container.DeleteIfExistsAsync();
            }

            return true;
        }

        public string UploadBase64Image(string base64Image, string fileName, string containerName)
        {
            // Limpa o hash enviado
            var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(base64Image, "");

            // Gera um array de Bytes
            byte[] imageBytes = Convert.FromBase64String(data);

            return UploadArrayByteAsync(imageBytes, fileName, containerName).Result;
        }

        public async Task<string> UploadArrayByteAsync(byte[] arrayBytes, string fileName, string containerName)
        {
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
            if (cloudBlobContainer.CreateIfNotExists())
            {
                cloudBlobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            var fileStream = new MemoryStream(arrayBytes);
            if (fileName != null && fileStream != null)
            {
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                await cloudBlockBlob.UploadFromStreamAsync(fileStream);
                return cloudBlockBlob.Uri.AbsoluteUri;
            }
            return string.Empty;
        }

        public List<ArquivoAzure> ListaArquivos(string containerName)
        {
            CloudStorageAccount backupStorageAccount = CloudStorageAccount.Parse(_configuration["AzureStorage:ConnectionString"]);
            var backupBlobClient = backupStorageAccount.CreateCloudBlobClient();
            var backupContainer = backupBlobClient.GetContainerReference(containerName);

            var lista = new List<ArquivoAzure>();
            if (backupContainer.Exists())
            {
                var blobs = backupContainer.ListBlobs().OfType<CloudBlockBlob>().ToList();

                foreach (var blob in blobs)
                {
                    var arq = new ArquivoAzure()
                    {
                        Nome = blob.Name,
                        Tamanho = blob.Properties.Length,
                        UltimaModificacao = blob.Properties.LastModified.GetValueOrDefault().DateTime,
                        Tipo = blob.Properties.ContentType
                    };
                    lista.Add(arq);
                }
            }
            return lista;
        }
    }
}
