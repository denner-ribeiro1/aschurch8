using ASChurchManager.Domain.Entities;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface ICongregacaoRepository : IRepositoryDAO<Congregacao>
    {
        short DefinirSede(long id);
        IEnumerable<Congregacao> GetAll(bool completo = true);
        Congregacao GetById(long id);
        Congregacao GetSede();
        IEnumerable<CongregacaoGrupo> ListarGrupoCongregacao(long congregacaoId);
        IEnumerable<CongregacaoObreiro> ListarObreirosCongregacao(long congregacaoId);
        IEnumerable<ObservacaoCongregacao> ListarObservacaoCongregacao(long congregacaoId);
        FichaCongregacao RelatorioFichaCongregacao(long id);
        IEnumerable<CongregacaoObreiro> ListarObreirosCongregacaoPorMembroId(long membroId);
        IEnumerable<Congregacao> BuscarCongregacao(int pageSize, int rowStart, string sorting, out int rowCount, string campo, string valor, long usuarioId);
        IEnumerable<Congregacao> ListarCongregacaoPaginado(int pageSize, int rowStart, string sorting, string filtro, string conteudo, long usuarioID, out int rowCount);
        IEnumerable<QuantidadePorCongregacao> ConsultarQtdMembrosCongregacao(long congregacaoId);
        int Delete(long id, long congregacaoId, long usuarioID);
    }
}
