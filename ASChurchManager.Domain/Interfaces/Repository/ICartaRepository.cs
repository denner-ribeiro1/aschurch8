using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Types;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface ICartaRepository : IRepositoryDAO<Carta>
    {
        IEnumerable<Carta> ListarCartasPorMembroId(long membroId);
        bool CancelarCarta(long pId, long usuarioID, out string erro);
        long AprovarCarta(long pId, long usuarioID);
        long ConsultarCodReceb(long pIdMembro);
        IEnumerable<Carta> VerificaCartaAguardandoRecebimento(long pIdMembro);
        IEnumerable<Carta> GetAllTipoEStatus(TipoDeCarta? pTipoCarta, StatusCarta? pStatusCarta, long pUsuarioID);
        bool TransferirSemCarta(IEnumerable<Membro> membros, long congregacaoDestino, long pUsuarioID);
        IEnumerable<Carta> ListarCartaPaginado(int pageSize, int rowStart, string sorting, string campo, string valor, StatusCarta? statusCarta, long usuarioID, out int rowCount);
    }
}
