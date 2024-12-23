using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASChurchManager.Domain.Interfaces.Repository
{
    public interface IPresencaRepository : IRepositoryDAO<Presenca>
    {
        IEnumerable<Presenca> ListarPresencaPaginado(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor, bool naoMembro, long usuarioID);

        IEnumerable<Presenca> ListarPresencaEmAberto(long usuarioID);

        int SalvarInscricao(PresencaMembro entity);

        int SalvarInscricaoArquivo(string nomeArquivo);

        List<PresencaMembro> ConsultarPresencaInscricaoPorPresencaId(long idPresenca, long usuarioID);

        PresencaMembro ConsultarPresencaInscricao(long idInscricao);

        PresencaMembro ConsultarPresencaInscricao(long idPresenca, long idMembro, string cpf, long usuarioID);

        PresencaMembro ConsultarPresencaInscricaoDatas(long idInscricao, int idData);

        List<PresencaMembro> ConsultarPresencaInscricoesDatas(int idPresenca, int idData);

        Task<int> DeleteInscricaoAsync(int id);

        int AtualizarPagoInscricao(int id, bool pago);

        List<PresencaDatas> ListarPresencaDatas(long idPresenca);

        int AtualizarStatusData(int idData, StatusPresenca status);

        IEnumerable<Presenca> ConsultarPresencaPorStatusData(int id, StatusPresenca status);

        int SalvarPresencaInscricaoDatas(int idInscricao, int idData, SituacaoPresenca situacao, TipoRegistro tipo, string justificativa = "");

        List<PresencaMembro> ConsultarPresencaEtiquetas(int idInscricao, int idCongregacao, int tipo, int membroId, string cpf, long usuarioId);

        List<PresencaDatas> ListarPresencaDatasEmAndamento();

        IEnumerable<PresencaMembro> ListarPresencaDatasPaginado(int pageSize, int rowStart, out int rowCount, string sorting,
            int idPresenca, int idData, string campo, string valor, long usuarioID);

        bool ExisteInscricaoDatas(int idData);

        IEnumerable<Presenca> ConsultarPresencaIdData(int idData);
    }
}
