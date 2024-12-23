using System;
using System.Collections.Generic;
using System.Linq.Expressions;
namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface IBasicOperations<TEntity>
        where TEntity : class
    {
        long Add(TEntity entity, long usuarioID = 0);
        long Update(TEntity entity, long usuarioID = 0);
        int Delete(TEntity entity, long usuarioID = 0);
        int Delete(long id, long usuarioID = 0);

        TEntity GetById(long id, long usuarioID);
        IEnumerable<TEntity> GetAll(long usuarioID);
    }
}