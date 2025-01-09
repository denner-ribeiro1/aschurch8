using ASChurchManager.Domain.Lib;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASChurchManager.API.Membro.Infra;

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
            _Logger.LogError(context.Exception, context.Exception.Message);
        }
        base.OnException(context);
    }
}

