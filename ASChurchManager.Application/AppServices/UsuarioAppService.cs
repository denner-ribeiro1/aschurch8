using ASBaseLib.Security.Cryptography.Providers;
using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using System;
using System.Collections.Generic;

namespace ASChurchManager.Application.AppServices
{
    public class UsuarioAppService : BaseAppService<Usuario>, IUsuarioAppService
    {
        #region Variaveis
        private readonly IUsuarioRepository _usuarioService;

        #endregion

        #region Construtor
        public UsuarioAppService(IUsuarioRepository usuarioService)
            : base(usuarioService)
        {
            _usuarioService = usuarioService;
        }
        #endregion
        private void AlterarSenhaUsuario(Usuario usuario)
        {
            usuario.Senha = Hash.GetHash(usuario.Senha, CryptoProviders.HashProvider.MD5);
        }

        #region Publicos
        public bool ValidarLogin(ref Usuario usuario)
        {


            if (_usuarioService.ValidarLogin(ref usuario))
                return true;
            throw new InvalidOperationException("Usuário ou Senha inválidos");
        }

        public long AlterarSenha(Usuario usuario, long usuarioAlteracao)
        {
            AlterarSenhaUsuario(usuario);
            return _usuarioService.AlterarSenha(usuario, usuarioAlteracao);
        }
        #endregion
        public bool VerificaUsuarioDuplicado(string username)
        {
            return _usuarioService.VerificaUsuarioDuplicado(username);
        }

        public bool AlterarSkinUsuario(string skin, long id)
        {
            return _usuarioService.AlterarSkinUsuario(skin, id);
        }

        public Usuario GetUsuarioByUsername(string userName)
        {
            return _usuarioService.GetUsuarioByUsername(userName);
        }

        public IEnumerable<Usuario> ListarUsuariosPaginada(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor, long usuarioID)
        {
            return _usuarioService.ListarUsuariosPaginada(pageSize, rowStart, out rowCount, sorting, campo, valor, usuarioID);
        }
    }
}