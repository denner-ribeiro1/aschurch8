using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;

namespace ASChurchManager.Application.AppServices
{
    public class CursoAppService : BaseAppService<Curso>, ICursoAppService
    {
        #region Variaveis
        private readonly ICursoRepository _cursoService;
        #endregion

        #region Construtor
        public CursoAppService(ICursoRepository cursoService)
            : base(cursoService)
        {
            _cursoService = cursoService;
        }
        #endregion
    }
}
