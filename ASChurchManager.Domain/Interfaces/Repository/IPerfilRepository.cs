using ASChurchManager.Domain.Entities;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface IPerfilRepository : IRepositoryDAO<Perfil>
    {
        long AddRotinaPerfil(long perfilId, long rotinaId);
    }

}