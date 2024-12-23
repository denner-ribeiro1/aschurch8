using ASChurchManager.Domain.Entities;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface IDashboardRepository : IRepositoryDAO<Dashboard>
    {
        Dashboard RetornaDadosDashboard(long usuarioId);
    }
}
