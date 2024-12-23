using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;

namespace ASChurchManager.Application.AppServices
{
    public class DashboardAppService : BaseAppService<Dashboard>, IDashboardAppService
    {
        private readonly IDashboardRepository _dash;

        public DashboardAppService(IDashboardRepository dashService)
           : base(dashService)
        {
            _dash = dashService;
        }

        public Dashboard RetornaDadosDashboard(long usuarioId)
        {
            return _dash.RetornaDadosDashboard(usuarioId);
        }
    }
}
