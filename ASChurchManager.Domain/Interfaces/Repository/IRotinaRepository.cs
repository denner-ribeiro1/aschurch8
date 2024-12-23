using ASChurchManager.Domain.Entities;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface IRotinaRepository : IRepositoryDAO<Rotina>
    {
        Rotina ConsultarRotinaPorAreaController(string area, string controller);

        IEnumerable<Rotina> ConsultarRotinasPorUsuario(long usuarioID);

    }
}