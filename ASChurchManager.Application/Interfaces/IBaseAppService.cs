using ASChurchManager.Domain.Intefaces.Repository;

namespace ASChurchManager.Application.Interfaces
{
    public interface IBaseAppService<TEntity> : IBasicOperations<TEntity> where TEntity : class
    {


    }
}