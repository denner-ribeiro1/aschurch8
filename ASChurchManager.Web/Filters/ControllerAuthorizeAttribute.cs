using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Web.Filters.Menu;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ASChurchManager.Web.Filters
{
    public class ControllerAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _authorizedControllers;

        public ControllerAuthorizeAttribute(params string[] authorizedControllers)
        {
            _authorizedControllers = authorizedControllers;
        }

        private bool IsProtectedController(string[] authorizedControllers, List<Rotina> rotinas)
        {
            return rotinas != null &&
                   authorizedControllers.Any(a => a == "Home" ||
                                                        rotinas.Any(r => r.Controller.Contains(a)));
        }

        private bool IsProtectedAction(AuthorizationFilterContext context, List<Rotina> rotinas)
        {
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
                return true;

            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var actionMethodInfo = controllerActionDescriptor.MethodInfo;

            var authorizeAttribute = actionMethodInfo.GetCustomAttribute<ActionAttribute>();
            if (authorizeAttribute != null)
            {
                var action = context.RouteData.Values["action"].ToString();
                var controler = context.RouteData.Values["controller"].ToString();
                var area = context.RouteData.Values["area"].ToString();

                return rotinas != null &&
                       rotinas.Any(r => r.Area.Contains(area) &&
                                        r.Controller.Contains(controler) &&
                                        r.Action.Contains(action));
            }
            return true;
        }

        private bool IsUserAuthenticated(AuthorizationFilterContext context)
        {
            return context.HttpContext.User.Identity.IsAuthenticated;
        }

#pragma warning disable 1998
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Verifica se o usuário está autenticado. 
            if (!IsUserAuthenticated(context))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            //Caso o item tenha a Action/Controller permitir anonimo
            if (context.ActionDescriptor.EndpointMetadata.Any(item => item is AllowAnonymousAttribute))
                return;

            //recupera os serviços de cache, configuração e usuario.
            var cache = context.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;
            var configuration = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var usuLog = context.HttpContext.RequestServices.GetService(typeof(IUsuarioLogado)) as IUsuarioLogado;

            //Recupera as informações de cache
            var usuario = usuLog.GetUsuarioLogado();

            //Caso não encontre o Usuário no cache, redireciona para o Login.
            if (usuario == null || usuario.Id == 0)
            {
                await context.HttpContext.SignOutAsync();
                context.HttpContext.Session.Clear();
                context.HttpContext.Response.Redirect("/Auth/Login", true);
                return;
            }

            //Rotinas que o usuario tem acesso.
            usuario.Perfil.Rotinas = usuLog.Rotinas;

            //Validando a Autorização sobre o controller
            //Em seguida, verificando se a Action possui o Atributo de Autorização e validando o acesso ao mesmo.
            if (IsProtectedController(_authorizedControllers, usuario.Perfil.Rotinas) && IsProtectedAction(context, usuario.Perfil.Rotinas))
                return;

            context.Result = new ForbidResult();
        }
#pragma warning restore 1998
    }
}