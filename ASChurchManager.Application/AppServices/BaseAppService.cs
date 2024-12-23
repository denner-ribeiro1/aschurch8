using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Intefaces.Repository;

namespace ASChurchManager.Application.AppServices
{
    public class BaseAppService<TEntity> : IBaseAppService<TEntity> where TEntity : class
    {
        #region Variaveis
        private readonly IBasicOperations<TEntity> _baseService; 
        #endregion

        #region Construtor
        protected BaseAppService(IBasicOperations<TEntity> baseService)
        {
            _baseService = baseService;
        } 
        #endregion

        #region Publicos
        public virtual long Add(TEntity entity, long usuarioID = 0)
        {
            return _baseService.Add(entity);
        }

        public virtual long Update(TEntity entity, long usuarioID = 0)
        {
            return _baseService.Update(entity);
        }

        public virtual int Delete(TEntity entity, long usuarioID = 0)
        {
            return _baseService.Delete(entity);
        }

        public virtual int Delete(long id, long usuarioID = 0)
        {
            return _baseService.Delete(id);
        }

        public virtual TEntity GetById(long id, long usuarioID)
        {
            return _baseService.GetById(id, usuarioID);
        }

        public virtual System.Collections.Generic.IEnumerable<TEntity> GetAll(long usuarioID)
        {
            return _baseService.GetAll(usuarioID);
        } 
        #endregion
    }
}