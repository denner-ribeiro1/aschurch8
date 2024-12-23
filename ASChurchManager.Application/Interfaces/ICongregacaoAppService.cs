using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASChurchManager.Application.Interfaces
{
    public interface ICongregacaoAppService : ICongregacaoRepository
    {
        bool FichaCongregacao(int id, int usuarioId, out byte[] relatorio, out string mimeType);

        List<ArquivoAzure> ListaArquivos(int id);

        Task<RetornoAzure> DownloadArquivo(int id, string nomeArquivo);

        Task<bool> DeleteFilesAsync(int id, string nomeArquivo);

        string UploadFileToStorage(int id, IFormFile file);
    }
}