using Azure.Storage.Blobs;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ASChurchManager.Console
{
    class Program
    {
        static void Main(string[] args)
        {

            FileStream ostrm;
            StreamWriter writer;
            TextWriter oldOut = System.Console.Out;

            System.Console.WriteLine($"Inicio do processo: {DateTime.Now.ToLongTimeString()}");
            try
            {
                ostrm = new FileStream("./log.txt", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
                System.Console.SetOut(writer);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Falha ao abri o arquivo de Log.txt");
                System.Console.WriteLine(e.Message);
                return;
            }

            int qtdSucesso = 0,
                qtdErro = 0;
            try
            {
                System.Console.WriteLine($"Inicio - Pesquisa Membro");
                string conn = ConfigurationManager.AppSettings["ConnStrLocal"];
                string azureConn = ConfigurationManager.AppSettings["AzureStorageHml.ConnectionString"];

                //string conn = ConfigurationManager.AppSettings["ConnStrAzure"];
                //string azureConn = ConfigurationManager.AppSettings["AzureStoragePrd.ConnectionString"];

                var repos = new Infra.Data.Repository.EnterpriseLibrary.MembroRepository(conn);
                var listaMembro = repos.ListarFotosMembros();

                var sortedDict = (from entry in listaMembro select entry)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);//.Take(20);


                System.Console.WriteLine($"Fim - Pesquisa Membro");

                foreach (var item in sortedDict)
                {

                    if (!string.IsNullOrWhiteSpace(item.Key.FotoPath))
                    {
                        try
                        {
                            System.Console.WriteLine($"Convertendo a foto do Membro {item.Key.Id} - {item.Key.Nome}");
                            var ret = UploadBase64Image(item.Key.FotoPath, item.Value, $"foto_{item.Key.Id}.{item.Value}", azureConn);

                            repos.AtualizarMembroFotoUrl(item.Key.Id, ret);

                            System.Console.WriteLine($"Url: {ret}");
                            System.Console.WriteLine($"Foto do Membro {item.Key.Id} - {item.Key.Nome} convertida com Sucesso");
                            System.Console.WriteLine();
                            qtdSucesso++;
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine();
                            System.Console.WriteLine("=============================ERRO================================================");
                            System.Console.WriteLine($"Erro ao converter foto. Mensagem: {ex.Message}");
                            System.Console.WriteLine($"Membro {item.Key.Id} - {item.Key.Nome}");
                            System.Console.WriteLine("=============================ERRO================================================");
                            System.Console.WriteLine();
                            qtdErro++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }

            System.Console.WriteLine($"Quantidade de Sucessos: {qtdSucesso}");
            System.Console.WriteLine($"Quantidade de Erros: {qtdErro}");
            

            System.Console.SetOut(oldOut);
            writer.Close();
            ostrm.Close();
            System.Console.WriteLine($"Fim do processo: {DateTime.Now.ToLongTimeString()}");
        }

        public static string UploadBase64Image(string base64Image, string formato, string fileName, string azureConn)
        {
            // Limpa o hash enviado
            var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(base64Image, "");

            // Gera um array de Bytes
            byte[] imageBytes = Convert.FromBase64String(data);

            // Define o BLOB no qual a imagem será armazenada
            var blobClient = new BlobClient(azureConn, "fotos", fileName);

            blobClient.DeleteIfExists();

            // Envia a imagem
            using (var stream = new MemoryStream(imageBytes))
            {
                blobClient.Upload(stream);
            }

            // Retorna a URL da imagem
            return blobClient.Uri.AbsoluteUri;
        }

        //public static Bitmap ResizeImage(Image image, int width, int height)
        //{
        //    var destRect = new Rectangle(0, 0, width, height);
        //    var destImage = new Bitmap(width, height);

        //    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        //    using (var graphics = Graphics.FromImage(destImage))
        //    {
        //        graphics.CompositingMode = CompositingMode.SourceCopy;
        //        graphics.CompositingQuality = CompositingQuality.HighQuality;
        //        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //        graphics.SmoothingMode = SmoothingMode.HighQuality;
        //        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        //        using (var wrapMode = new ImageAttributes())
        //        {
        //            wrapMode.SetWrapMode(WrapMode.TileFlipXY);

        //            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        //        }
        //    }

        //    return destImage;
        //}
    }
}
