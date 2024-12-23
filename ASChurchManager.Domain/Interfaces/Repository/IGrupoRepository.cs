using ASChurchManager.Domain.Entities;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface IGrupoRepository : IRepositoryDAO<Grupo>
    {
        IEnumerable<CongregacaoGrupo> ListarGrupoCongregacao(int congregacaoId);
    }
}
