using ASChurchManager.Domain.Intefaces.Repository;

namespace ASChurchManager.Infra.Data.Repository.EnterpriseLibrary
{
    public abstract class RepositoryDAO<TEntity> : IRepositoryDAO<TEntity> where TEntity : class
    {
        #region Metodos
        public abstract long Add(TEntity entity, long usuarioID = 0);
        public abstract long Update(TEntity entity, long usuarioID = 0);
        public abstract int Delete(TEntity entity, long usuarioID = 0);
        public abstract int Delete(long id, long usuarioID = 0);
        public abstract TEntity GetById(long id, long usuarioID);
        public abstract System.Collections.Generic.IEnumerable<TEntity> GetAll(long usuarioID);
        #endregion
    }
}