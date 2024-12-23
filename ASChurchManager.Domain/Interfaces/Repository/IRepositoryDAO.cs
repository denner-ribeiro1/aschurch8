using System;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface IRepositoryDAO<TEntity> : IBasicOperations<TEntity>
        where TEntity : class
    {
        //string GetActiveConnectionString(string activeConnectionString);
        //string ConnectionString { get; }
    }
}