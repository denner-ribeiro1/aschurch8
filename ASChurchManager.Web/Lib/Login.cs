using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASChurchManager.Web.Lib
{
    public static class Login
    {
        public async static void Logar(HttpContext HttpContext, Usuario usuario, IConfiguration _configuration)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", usuario.Id.ToString()),
                new Claim("Nome", usuario.Nome),
                new Claim("Login", usuario.Username),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim("CongregacaoNome", usuario.CongregacaoNome),
                new Claim("CongregacaoId", usuario.Congregacao.Id.ToString()),
                new Claim("Usuario", Newtonsoft.Json.JsonConvert.SerializeObject(usuario))
            };

            ClaimsIdentity userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);

            var expireMinutes = _configuration["UserSessionExpirationMinutes"].TryConvertTo<int>();
            var propriedadesDeAutenticacao = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTime.Now.ToLocalTime().AddMinutes(expireMinutes),
                IsPersistent = false
            };
            await HttpContext.SignInAsync(principal, propriedadesDeAutenticacao);
        }

        public async static Task<bool> LogOut(HttpContext HttpContext, IUsuarioLogado usuario, IMemoryCache cache)
        {
            cache.Remove($"Dashboard_{usuario.Nome}");
            cache.Remove($"Rotinas_{usuario.Login}");

            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();

            return true;
        }
    }
}
