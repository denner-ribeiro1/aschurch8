using ASChurchManager.Domain.Entities;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface ICursoMembroRepository : IRepositoryDAO<CursoMembro>
    {
        void BeginTran();
        void Commit();
        void RollBack();
    }
}