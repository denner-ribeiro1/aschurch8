using ASChurchManager.Domain.Entities;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface ICasamentoRepository : IRepositoryDAO<Casamento>
    {
        Casamento VerificarCasamentoCongregacao(Casamento casamento);
        IEnumerable<Casamento> ListarCasamentoPaginado(int pageSize, int rowStart, string sorting, string campo, string valor, long usuarioID, out int rowCount);
        Casamento GetById(long id);
    }
}
