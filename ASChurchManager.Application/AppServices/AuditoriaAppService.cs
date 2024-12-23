using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;

namespace ASChurchManager.Application.AppServices
{
    public class AuditoriaAppService : BaseAppService<Auditoria>, IAuditoriaAppService
    {
        #region Variaveis
        private readonly IAuditoriaRepository _auditoriaService;
        #endregion

        #region Construtor
        public AuditoriaAppService(IAuditoriaRepository auditoriaService)
            : base(auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }
        #endregion

        #region Publicos

        #endregion
    }
}
