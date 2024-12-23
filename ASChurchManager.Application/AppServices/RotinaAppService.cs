using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using System.Collections.Generic;
using System.Linq;

namespace ASChurchManager.Application.AppServices
{
    public class RotinaAppService : BaseAppService<Rotina>, IRotinaAppService
    {
        #region Variaveis
        private readonly IRotinaRepository _rotinaService;
        #endregion

        #region Construtor
        public RotinaAppService(IRotinaRepository rotinaService)
            : base(rotinaService)
        {
            _rotinaService = rotinaService;
        }
        #endregion

        #region Publicos
        public void AddRotinas(IEnumerable<Rotina> rotinas)
        {
            /*Insiro todas as rotinas que nao pertencem a um menu)*/
            var rotinaOrd = rotinas.OrderBy(x => x.Area)
                                .ThenBy(x => x.Controller);

            foreach (var rotina in rotinaOrd)
            {
                _rotinaService.Add(rotina);
            }
        }

        public Rotina ConsultarRotinaPorAreaController(string area, string controller)
        {
            return _rotinaService.ConsultarRotinaPorAreaController(area, controller);
        }

        public IEnumerable<Rotina> ConsultarRotinasPorUsuario(long usuarioID)
        {
            return _rotinaService.ConsultarRotinasPorUsuario(usuarioID);
        }

        #endregion
    }
}