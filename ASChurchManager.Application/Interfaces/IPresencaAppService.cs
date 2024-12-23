using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ASChurchManager.Application.Interfaces
{
    public interface IPresencaAppService : IPresencaRepository
    {
        Task<List<PresencaMembro>> LerArquivoExcelAsync(IFormFile file, int id, long usuarioID);

        byte[] Etiquetas(int idPresenca, int congregacao, int tipo, int membroId, string cpf, string posicao, int margemTop, int margemEsq, long usuarioID);

        List<ArquivoAzure> ListaArquivos(int idPresenca);

        Task<RetornoAzure> DownloadArquivo(int idPresenca, string nomeArquivo);

        Task<bool> DeleteFilesAsync(int idPresenca, string nomeArquivo);

        string UploadFileToStorage(int idPresenca, IFormFile file);
    }
}
