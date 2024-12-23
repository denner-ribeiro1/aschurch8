using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using System.Collections.Generic;

namespace ASChurchManager.Application.AppServices
{
    public class CasamentoAppService : BaseAppService<Casamento>, ICasamentoAppService
    {
        #region Variaveis
        private readonly ICasamentoRepository _casamentoService;
        #endregion

        #region Construtor
        public CasamentoAppService(ICasamentoRepository casamentoService)
            : base(casamentoService)
        {
            _casamentoService = casamentoService;
        }
        #endregion

        public Casamento VerificarCasamentoCongregacao(Casamento casamento)
        {
            return _casamentoService.VerificarCasamentoCongregacao(casamento);
        }
        public IEnumerable<Casamento> ListarCasamentoPaginado(int pageSize, int rowStart, string sorting, string campo, string valor, long usuarioID, out int rowCount)
        {
            return _casamentoService.ListarCasamentoPaginado(pageSize, rowStart, sorting, campo, valor, usuarioID, out rowCount);
        }

        public Casamento GetById(long id)
        {
            return _casamentoService.GetById(id);
        }
    }
}
