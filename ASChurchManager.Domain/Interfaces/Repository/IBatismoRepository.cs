using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Types;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface IBatismoRepository : IRepositoryDAO<Batismo>
    {
        void AtualizarMembroBatismo(IEnumerable<BatismoMembro> candidatoBatismo, long batismoId);
        void AtualizarStatusBatismo(long batismoId, SituacaoCandidatoBatismo situacao);
        Batismo SelecionaUltimoDataBatismo();
        IEnumerable<Membro> SelecionaMembrosParaBatismo(long batismoId, StatusBatismo status, long usuarioID);
        IEnumerable<Membro> ListarPastorCelebrante(long batismoId);
        IEnumerable<Membro> ListarCandidatosBatismoPaginada(int pageSize, int rowStart, string sorting, string campo, string valor, long usuarioID, out int rowCount);
        IEnumerable<Batismo> ListarBatismoPaginada(int pageSize, int rowStart, string sorting, out int rowCount);
    }
}
