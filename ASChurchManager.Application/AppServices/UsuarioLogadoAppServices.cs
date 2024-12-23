using ASChurchManager.Application.AppServices;
using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ASChurchManager.Domain.Entities
{
    public class UsuarioLogadoAppServices : BaseAppService<Usuario>, IUsuarioLogado
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IRotinaAppService _rotinaAppService;
        private readonly IMemoryCache _cache;

        public UsuarioLogadoAppServices(IHttpContextAccessor accessor,
            IUsuarioRepository usuarioService,
            IRotinaAppService rotinaAppService,
            IMemoryCache cache)
            : base(usuarioService)
        {
            _accessor = accessor;
            _rotinaAppService = rotinaAppService;
            _cache = cache;
        }

        public string Nome => _accessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Nome").Value;
        public string Login => _accessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Login").Value;
        public int Id => int.Parse(_accessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Id").Value);

        public List<Rotina> Rotinas
        {
            get
            {
                var usuario = GetUsuarioLogado();
                if (usuario != null)
                {
                    return _cache.GetOrCreate($"Rotinas_{Login}", item =>
                    {
                        item.SlidingExpiration = TimeSpan.FromMinutes(10);
                        var lstRotinas = usuario.Perfil.TipoPerfil == TipoPerfil.Administrador
                                     ? _rotinaAppService.GetAll(Convert.ToInt32(Id))
                                     : _rotinaAppService.ConsultarRotinasPorUsuario(usuario.Id);
                        return lstRotinas.ToList();
                    });
                }
                return null;
            }
        }
        public bool IsAuthenticated()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext.User.Claims;
        }

        public Usuario GetUsuarioLogado()
        {
            if (IsAuthenticated())
            {
                var usu = _accessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Usuario").Value;
                var usuario = JsonConvert.DeserializeObject<Usuario>(usu);
                return usuario;
            }
            return null;
        }
    }
}
