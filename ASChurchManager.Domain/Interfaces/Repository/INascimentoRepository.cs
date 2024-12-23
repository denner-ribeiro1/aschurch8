using ASChurchManager.Domain.Entities;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface INascimentoRepository : IRepositoryDAO<Nascimento>
    {
        IEnumerable<Nascimento> ListarNascimentoPaginado(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor, long usuarioID);
    }
}
