using ASChurchManager.Domain.Entities;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface ICursoArquivoMembroRepository : IRepositoryDAO<CursoArquivoMembro>
    {
        IEnumerable<CursoArquivoMembro> GetArquivoByMembro(long membroId);

        void BeginTran();
        void Commit();
        void RollBack();
    }
}
