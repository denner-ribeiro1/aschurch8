using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace ASChurchManager.Web.Lib
{
    [ServiceFilter(typeof(SiteExceptionFilter))]
    public class BaseController : Microsoft.AspNetCore.Mvc.Controller
    {
        public readonly IMemoryCache cache;
        public readonly IUsuarioLogado usuario;
        public readonly IConfiguration configuration;
        public readonly IRotinaAppService rotinaAppService;

        public BaseController(IMemoryCache _cache,
            IUsuarioLogado _usuario,
            IConfiguration _configuration,
            IRotinaAppService _rotinaAppService)
        {
            cache = _cache;
            usuario = _usuario;
            configuration = _configuration;
            rotinaAppService = _rotinaAppService;

        }
    }
}
