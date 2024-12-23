using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASChurchManager.Application.Interfaces
{
    public interface ICursoArquivoMembroAppService : ICursoArquivoMembroRepository
    {
        bool UploadFile(CursoArquivoMembro arquivoMembro);
        Task<string> DeleteFileAsync(CursoArquivoMembro arquivoMembro);
        Task<RetornoAzure> DownloadFileAsync(CursoArquivoMembro arquivoMembro);

    }
}
