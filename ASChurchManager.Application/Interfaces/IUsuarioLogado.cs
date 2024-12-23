using ASChurchManager.Domain.Entities;
using System.Collections.Generic;
using System.Security.Claims;

namespace ASChurchManager.Domain.Interfaces
{
    public interface IUsuarioLogado
    {
        string Nome { get; }
        string Login { get; }
        int Id { get; }
        bool IsAuthenticated();
        IEnumerable<Claim> GetClaimsIdentity();
        Usuario GetUsuarioLogado();
        List<Rotina> Rotinas { get; }
    }
}
