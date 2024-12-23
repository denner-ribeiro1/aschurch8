using ASChurchManager.Domain.Lib;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASChurchManager.Web.Filters
{
    public class SiteExceptionFilter : ExceptionFilterAttribute
    {
        private ILogger<SiteExceptionFilter> _Logger;

        public SiteExceptionFilter(ILogger<SiteExceptionFilter> logger)
        {
            _Logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            if (!(context.Exception is Erro))
            {
                _Logger.LogError(context.Exception, $"Usuario: {context.HttpContext.User.Identity.Name}");
            }
            base.OnException(context);
        }
    }
}
