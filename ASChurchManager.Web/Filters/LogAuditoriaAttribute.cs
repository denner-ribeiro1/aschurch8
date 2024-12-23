using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Web.Lib;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASChurchManager.Web.Filters
{
    public class LogAuditoriaAttribute : ActionFilterAttribute
    {
        private readonly IAuditoriaAppService _auditoriaAppService;
        private readonly IActionContextAccessor _actionContextAccessor;

        public LogAuditoriaAttribute(IAuditoriaAppService auditoriaAppService, IActionContextAccessor actionContextAccessor)
        {
            _auditoriaAppService = auditoriaAppService;
            _actionContextAccessor = actionContextAccessor;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                if (context != null)
                {
                    var request = context.HttpContext.Request;
                    var valores = new Dictionary<string, string>();
                    request.Form.Keys.Where(p => p != "__RequestVerificationToken")
                        .ToList()
                        .ForEach(item => valores.Add(item, request.Form[item].ToString()));
                    var json = JsonConvert.SerializeObject(valores, Formatting.None);

                    var audit = new Auditoria()
                    {
                        UsuarioId = UserAppContext.Current.Usuario.Id,
                        Controle = _actionContextAccessor.ActionContext.RouteData.Values["controller"].ToString(),
                        Acao = _actionContextAccessor.ActionContext.RouteData.Values["action"].ToString(),
                        Ip = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                        Url = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(request),
                        Parametros = json,
                        Navegador = request.Headers["User-Agent"].ToString()
                    };
                    _auditoriaAppService.Add(audit);
                }
            }
            catch (System.Exception ex)
            {
                _auditoriaAppService.Add(new Auditoria()
                {
                    UsuarioId = UserAppContext.Current.Usuario.Id,
                    Parametros = ex.Message
                });
            }
            await base.OnActionExecutionAsync(context, next);
        }
    }
}
