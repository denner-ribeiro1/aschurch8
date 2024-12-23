using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Lib;
using ASChurchManager.Web.Models.Log;
using ASChurchManager.Web.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASChurchManager.Web.Controllers
{
    [ControllerAuthorize("Home")]
    public class HomeController : BaseController
    {
        private readonly IDashboardAppService _dashboardApp;
        private readonly IEventosAppService _eventoAppService;
        private readonly IMemoryCache _cache;

        public HomeController(
            IDashboardAppService dashboardApp,
            IEventosAppService eventoAppService,
            IMemoryCache cache,
            IUsuarioLogado usuLog
            , IConfiguration _configuration
            , IRotinaAppService _rotinaAppService)
            : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _dashboardApp = dashboardApp;
            _eventoAppService = eventoAppService;
            _cache = cache;
        }

        public ActionResult Index()
        {
            /*Carregando o cache com o Dashboard*/
            var usuCacheBD = _cache.GetOrCreate($"Dashboard_{UserAppContext.Current.Usuario.Username}", item =>
            {
                item.SlidingExpiration = TimeSpan.FromMinutes(10);

                var dash = new EventosDashBoardVM
                {
                    Dashboard = _dashboardApp.RetornaDadosDashboard((int)UserAppContext.Current.Usuario.Id),
                    Eventos = _eventoAppService.ListarEventosPorData(DateTime.Now.Date, DateTime.Now.AddDays(30).Date, out List<Feriado> feriados),
                    Feriados = feriados
                };

                return dash;
            });
            return View();
        }

        public ActionResult _Menu()
        {
            return View(UserAppContext.Current.Usuario.Perfil.Rotinas);
        }

        public ActionResult Log()
        {
            return View(new LogVM());
        }

        public async System.Threading.Tasks.Task<JsonResult> PesquisarLog([FromServices] IConfiguration _configuration,
            int page, int pageSize, string data, string path, string exception)
        {
            try
            {
                var conStr = _configuration["ConnectionStrings:BaseLog"];
                var databaseName = _configuration["ParametrosSistema:DatabaseMongoDB"];
                var collectionName = _configuration["ParametrosSistema:CollectionLog"];

                IMongoClient client = new MongoClient(conStr);
                IMongoDatabase database = client.GetDatabase(databaseName);
                IMongoCollection<Root> collection = database.GetCollection<Root>(collectionName);

                var countFacet = AggregateFacet.Create("countFacet",
                PipelineDefinition<Root, AggregateCountResult>.Create(new[]
                {
                PipelineStageDefinitionBuilder.Count<Root>()
                }));

                var dataFacet = AggregateFacet.Create("dataFacet",
                      PipelineDefinition<Root, Root>.Create(new[]
                      {
                        PipelineStageDefinitionBuilder.Sort(Builders<Root>.Sort.Ascending(x => x.Timestamp)),
                        PipelineStageDefinitionBuilder.Skip<Root>((page - 1) * pageSize),
                        PipelineStageDefinitionBuilder.Limit<Root>(pageSize),
                      }));

                var filtros = new List<FilterDefinition<Root>>();
                if (!string.IsNullOrEmpty(exception))
                    filtros.Add(Builders<Root>.Filter.Where(p => p.Exception.ToLower().Contains(exception.ToLower())));
                if (!string.IsNullOrEmpty(path))
                    filtros.Add(Builders<Root>.Filter.Where(p => p.Properties.RequestPath.ToLower().Contains(path.ToLower())));
                if (!string.IsNullOrEmpty(data))
                    filtros.Add(Builders<Root>.Filter.Gte(p => p.Timestamp, new BsonDateTime(Convert.ToDateTime(data.Substring(0, data.IndexOf('T'))))));
                var filter = filtros.Count > 0 ? Builders<Root>.Filter.Or(filtros.ToArray()) : Builders<Root>.Filter.Empty;

                var aggregation = await collection.Aggregate()
                    .Match(filter)
                    .Facet(countFacet, dataFacet)
                    .ToListAsync();

                var count = aggregation.First()
                    .Facets.First(x => x.Name == "countFacet")
                    .Output<AggregateCountResult>()
                    ?.FirstOrDefault()
                    ?.Count ?? 0;

                var dados = aggregation.First()
                    .Facets.First(x => x.Name == "dataFacet")
                    .Output<Root>();

                var ret = new RetornoLogVM()
                {
                    Count = (int)count / pageSize,
                    Size = pageSize,
                    Page = page,
                    Items = dados,
                    Status = "OK"
                };

                return Json(ret);
            }
            catch (Exception ex)
            {
                return Json(new RetornoLogVM()
                {
                    Status = "Erro",
                    Erro = ex.Message
                });
            }
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> ExcluirLog([FromServices] IConfiguration _configuration,
            int qtdRegistroExcluir, bool? excluirTodosRegistros)
        {
            try
            {
                var conStr = _configuration["ConnectionStrings:BaseLog"];
                var databaseName = _configuration["ParametrosSistema:DatabaseMongoDB"];
                var collectionName = _configuration["ParametrosSistema:CollectionLog"];

                IMongoClient client = new MongoClient(conStr);
                IMongoDatabase database = client.GetDatabase(databaseName);
                IMongoCollection<Root> collection = database.GetCollection<Root>(collectionName);

                if (excluirTodosRegistros.GetValueOrDefault(false))
                {
                    collection.DeleteMany(Builders<Root>.Filter.Empty);
                }
                else
                {
                    var countFacet = AggregateFacet.Create("countFacet",
                           PipelineDefinition<Root, AggregateCountResult>.Create(new[]
                           {
                        PipelineStageDefinitionBuilder.Count<Root>()
                           }));

                    var dataFacet = AggregateFacet.Create("dataFacet",
                          PipelineDefinition<Root, Root>.Create(new[]
                          {
                        PipelineStageDefinitionBuilder.Sort(Builders<Root>.Sort.Descending(x => x.Timestamp)),
                        PipelineStageDefinitionBuilder.Limit<Root>(qtdRegistroExcluir),
                          }));

                    var filter = Builders<Root>.Filter.Empty;
                    var aggregation = await collection.Aggregate()
                        .Match(filter)
                        .Facet(countFacet, dataFacet)
                        .ToListAsync();

                    var count = aggregation.First()
                        .Facets.First(x => x.Name == "countFacet")
                        .Output<AggregateCountResult>()
                        ?.FirstOrDefault()
                        ?.Count ?? 0;

                    var dados = aggregation.First()
                        .Facets.First(x => x.Name == "dataFacet")
                        .Output<Root>();

                    foreach (var item in dados)
                    {
                        collection.DeleteOne(a => a.Timestamp == item.Timestamp);
                    }
                }
                return Json(new RetornoLogVM()
                {
                    Status = "OK"
                });
            }
            catch (Exception ex)
            {
                return Json(new RetornoLogVM()
                {
                    Status = "Erro",
                    Erro = ex.Message
                });
            }
        }
    }
}