using ASChurchManager.Domain.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ASChurchManager.Application.Interfaces
{
    public interface IArquivosAzureAppService
    {
        string AccountName { get; }
        string AccountKey { get; }
        string UploadFileToStorage(Stream fileStream, string fileName, string containerName, string fileMimeType);
        Task<string> UploadArrayByteAsync(byte[] arrayImage, string fileName, string containerName);
        string UploadBase64Image(string base64Image, string fileName, string containerName);
        Task<RetornoAzure> DownloadFromStorageAsync(string fileName, string containerName);
        Task<bool> DeleteSpecificFileAsync(string fileName, string containerName);
        Task<bool> DeleteSpecificContainerAsync(string containerName);
        List<ArquivoAzure> ListaArquivos(string containerName);
        Task<string> RetornaURLArquivo(string fileName, string containerName);
        RetornoAzure DownloadFromUrl(string url);
    }
}