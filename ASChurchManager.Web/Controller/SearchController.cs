using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Web.Lib;
using ASChurchManager.Web.ViewModels.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ASChurchManager.Web.Controllers
{
    public class SearchController : BaseController
    {
        private readonly IMembroAppService _membroAppService;
        private readonly ICongregacaoAppService _congregacaoAppService;
        private readonly ITipoEventoAppService _tipoEventoAppService;

        public SearchController(IMembroAppService membroAppService
                                , ICongregacaoAppService congregacaoAppService
                                , ITipoEventoAppService tipoEventoAppService
                                , IMemoryCache cache
                                , IUsuarioLogado usuLog
                                , IConfiguration _configuration
                                , IRotinaAppService _rotinaAppService)
            : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _membroAppService = membroAppService;
            _congregacaoAppService = congregacaoAppService;
            _tipoEventoAppService = tipoEventoAppService;
        }

        [HttpGet]
        public ActionResult Index(string searchModel, string target, string title = "", string parametros = "")
        {
            return View("Search", new SearchViewModel(searchModel, target, title, parametros));
        }


        [HttpPost]
        public JsonResult GetList(string property, string value, string model, string parametros,
            int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            
            try
            {
                var campo = new List<string>();
                if (!string.IsNullOrWhiteSpace(property))
                {
                    var valores = property.Split(';');
                    var cmp = valores[0].Split(':');
                    for (int i = 0; i < cmp.Length; i++)
                        campo.Add(cmp[i]);
                }
                switch (model)
                {
                    case "Membro":
                        {
                            var filtrarUsuario = "S"; //Padrão SEMPRE filtrar por Usuario
                            var tpMembro = Domain.Types.TipoMembro.NaoDefinido;
                            var status = Domain.Types.Status.NaoDefinido;
                            if (!string.IsNullOrWhiteSpace(parametros))
                            {
                                var data = (JObject)JsonConvert.DeserializeObject(parametros);
                                if (data["TipoMembro"] != null)
                                    Enum.TryParse(data["TipoMembro"].Value<string>(), out tpMembro);
                                if (data["Status"] != null)
                                    Enum.TryParse(data["Status"].Value<string>(), out status);
                                if (data["FiltrarUsuario"] != null)
                                    filtrarUsuario = data["FiltrarUsuario"].Value<string>();
                            }

                            var cmp = "Id";
                            if (campo.Count > 1)
                                cmp = campo[1];

                            var membros = _membroAppService.ListarMembroPaginado(jtPageSize, jtStartIndex, out int qtdRows, jtSorting, cmp, value,
                                (filtrarUsuario == "S" ? UserAppContext.Current.Usuario.Id : 0), tpMembro, status);
                            return Json(new { Result = "OK", Records = membros, TotalRecordCount = qtdRows });
                        }
                    case "Congregacao":
                        {
                            var cmp = "Id";
                            if (campo.Count > 1)
                                cmp = campo[1];

                            var filtrarUsuario = "N"; //Padrão NUNCA filtrar por Usuario
                            if (!string.IsNullOrWhiteSpace(parametros))
                            {
                                var data = (JObject)JsonConvert.DeserializeObject(parametros);
                                if (data["FiltrarUsuario"] != null)
                                    filtrarUsuario = data["FiltrarUsuario"].Value<string>();
                            }

                            var congr = _congregacaoAppService.BuscarCongregacao(jtPageSize, jtStartIndex, jtSorting, out int mem, cmp, value,
                                (filtrarUsuario == "S" ? UserAppContext.Current.Usuario.Id : 0));
                            return Json(new { Result = "OK", Records = congr, TotalRecordCount = mem });
                        }
                    case "MembroObreiros":
                        {
                            var cmp = "Id";
                            if (campo.Count > 1)
                                cmp = campo[1];

                            int congrId = 0;
                            if (!string.IsNullOrWhiteSpace(parametros))
                            {
                                var data = (JObject)JsonConvert.DeserializeObject(parametros);
                                if (data["CongregacaoId"] != null)
                                    int.TryParse(data["CongregacaoId"].ToString(), out congrId);
                            }
                            var membros = _membroAppService.ListarMembroObreiroPaginado(jtPageSize, jtStartIndex, out int qtdRows, jtSorting, cmp, value,
                                congrId, UserAppContext.Current.Usuario.Id);
                            return Json(new { Result = "OK", Records = membros, TotalRecordCount = qtdRows });
                        }
                    default:
                        return Json(new { Result = "ERROR", Message = "Não foi implementado a pesquisa para o Modelo" });
                };
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}