using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using System.Collections.Generic;

namespace ASChurchManager.Application.Interfaces
{
    public interface IPerfilAppService : IPerfilRepository
    {
        Dictionary<Rotina, bool> GetRotinasPerfil(Perfil perfil);
    }
}