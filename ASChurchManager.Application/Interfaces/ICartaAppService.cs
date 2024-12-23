using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using System.Collections.Generic;

namespace ASChurchManager.Application.Interfaces
{
    public interface ICartaAppService : ICartaRepository
    {
        TipoStatusRetorno AprovarCarta(long pId, string codigoRecebimento, long usuarioId, out List<ErroRetorno> ErrosRetorno);
    }
}
