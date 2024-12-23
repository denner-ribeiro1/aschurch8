using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using System.Collections.Generic;

namespace ASChurchManager.Application.AppServices
{
    public class GrupoAppService : BaseAppService<Grupo>, IGrupoAppService
    {
        #region Variaveis
        private readonly IGrupoRepository _grupoService;
        #endregion

        public GrupoAppService(IGrupoRepository grupoService)
            : base(grupoService)
        {
            _grupoService = grupoService;
        }

        public IEnumerable<CongregacaoGrupo> ListarGrupoCongregacao(int congregacaoId)
        {
            return _grupoService.ListarGrupoCongregacao(congregacaoId);
        }
    }
}
