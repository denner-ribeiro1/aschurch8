using ASChurchManager.Domain.Entities;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface IUsuarioRepository : IRepositoryDAO<Usuario>
    {
        Usuario GetUsuarioByUsername(string userName);

        bool ValidarLogin(ref Usuario usuario);

        long AlterarSenha(Usuario usuario, long usuarioAlteracao);
        bool VerificaUsuarioDuplicado(string username);

        bool AlterarSkinUsuario(string skin, long id);
        IEnumerable<Usuario> ListarUsuariosPaginada(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor, long usuarioID);
    }
}