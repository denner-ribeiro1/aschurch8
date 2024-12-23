using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;

namespace ASChurchManager.Application.AppServices
{
    public class TipoEventoAppService : BaseAppService<TipoEvento>, ITipoEventoAppService
    {
        #region Variaveis
        private readonly ITipoEventoRepository _TipoEventoService;
        #endregion

        #region Construtor
        public TipoEventoAppService(ITipoEventoRepository TipoEventoService)
            : base(TipoEventoService)
        {
            _TipoEventoService = TipoEventoService;
        }
        #endregion
    }
}
