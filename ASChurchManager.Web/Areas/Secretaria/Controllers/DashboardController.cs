using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Carta;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Dashboard;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Eventos;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro;
using ASChurchManager.Web.Lib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ASChurchManager.Web.Areas.Secretaria.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly ICartaAppService _cartaAppService;
        private readonly IMembroAppService _membroAppService;
        private readonly IDashboardAppService _dashboardApp;
        private readonly IEventosAppService _eventoAppService;

        public DashboardController(ICartaAppService cartaAppService,
                                   IMembroAppService membroAppService,
                                   IDashboardAppService dashboardApp,
                                   IEventosAppService eventoAppService,
                                   IMemoryCache cache,
                                   IUsuarioLogado usuLog
                                   , IConfiguration _configuration
                                   , IRotinaAppService _rotinaAppService)
            : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _cartaAppService = cartaAppService;
            _membroAppService = membroAppService;
            _dashboardApp = dashboardApp;
            _eventoAppService = eventoAppService;
        }

        public ActionResult RetornaGrafico([FromServices] IMemoryCache _cache)
        {
            var dashCacheBD = _cache.GetOrCreate($"Dashboard_{UserAppContext.Current.Usuario.Username}", item =>
            {
                item.SlidingExpiration = TimeSpan.FromMinutes(10);

                var dash = new Web.Models.Shared.EventosDashBoardVM
                {
                    Dashboard = _dashboardApp.RetornaDadosDashboard((int)UserAppContext.Current.Usuario.Id),
                    Eventos = _eventoAppService.ListarEventosPorData(DateTime.Now.Date, DateTime.Now.AddDays(30).Date, out List<Feriado> feriados),
                    Feriados = feriados
                };

                return dash;
            });

            var dash = dashCacheBD.Dashboard;
            var itemPend = dash.SituacaoMembro.FirstOrDefault(i => i.Status == Status.PendenteAprovacao);

            var qtdPend = 0;
            if (itemPend != null)
                qtdPend = itemPend.Quantidade;

            var itemNaoApr = dash.SituacaoMembro.FirstOrDefault(i => i.Status == Status.NaoAprovado);

            var qtdNaoApr = 0;
            if (itemNaoApr != null)
                qtdNaoApr = itemNaoApr.Quantidade;

            var dashVm = new DashboardVM()
            {
                MembrosPendentes = qtdPend,
                MembrosReprovados = qtdNaoApr,
                QuantidadeCartasPendentes = dash.QuantidadeCartasPendentes,
                QuantidadeCongregados = dash.QuantidadeCongregados,
            };
            dash.SituacaoMembro.ForEach(p => dashVm.SituacaoMembro.Add(new DashboardItemVM() { Quantidade = p.Quantidade, Situacao = (int)p.Status }));
            return PartialView("_Graficos", dashVm);
        }

        public JsonResult CartasPendentes([FromServices] ILogger<DashboardController> logger,
            int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var cartas = _cartaAppService.ListarCartaPaginado(jtPageSize, jtStartIndex, jtSorting, "", "", StatusCarta.AguardandoRecebimento, UserAppContext.Current.Usuario.Id, out int qtdRows).ToList();

                var cartaVM = new List<GridCartaItem>();
                cartas.ForEach(p => cartaVM.Add(new GridCartaItem()
                {
                    Id = p.Id,
                    CongregacaoDestino = p.CongregacaoDest,
                    CongregacaoOrigem = p.CongregacaoOrigem,
                    Nome = p.Nome,
                    StatusCarta = p.StatusCarta.GetDisplayAttributeValue(),
                    TipoCarta = p.TipoCarta.GetDisplayAttributeValue(),
                    DataValidade = p.DataValidade,
                    TemplateId = p.TemplateId,
                    AprovarCarta = p.StatusCarta == StatusCarta.AguardandoRecebimento &&
                        (UserAppContext.Current.Usuario.Congregacao.Sede || UserAppContext.Current.Usuario.Congregacao.Id == p.CongregacaoDestId)
                }));
                return Json(new { Result = "OK", Records = cartaVM, TotalRecordCount = qtdRows });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        public JsonResult MembrosPendentes([FromServices] ILogger<DashboardController> logger,
            int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(jtSorting))
                    jtSorting = jtSorting == "Congregacao" ? jtSorting : jtSorting.Replace("Congregacao", "CongregacaoId");
                var membros = _membroAppService.ListarMembrosPendencias(jtPageSize, jtStartIndex, out int qtdRows, jtSorting, UserAppContext.Current.Usuario.Id).ToList();
                var membrosVM = new List<GridMembroItem>();
                membros.ForEach(p => membrosVM.Add(new GridMembroItem()
                {
                    Id = p.Id,
                    Congregacao = p.Congregacao.Nome,
                    Nome = p.Nome,
                    NomeMae = p.NomeMae,
                    Cpf = p.Cpf,
                    Status = p.Status.GetDisplayAttributeValue()
                }));

                return Json(new { Result = "OK", Records = membrosVM, TotalRecordCount = qtdRows });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        private List<EventosViewModel> RetornaEventos(List<Evento> eventos, List<Feriado> feriados)
        {
            var eventoVM = new List<EventosViewModel>();
            foreach (var evento in eventos)
            {
                var bgColor = "";
                switch (evento.Tipo)
                {
                    case Evento.TipoEvento.Evento:
                    case Evento.TipoEvento.Curso:
                        {
                            if (evento.Congregacao.Sede)
                                bgColor = "#780c1e";//"#FF0000";
                            else if (evento.CongregacaoId == UserAppContext.Current.Usuario.Congregacao.Id)
                                bgColor = "#005380";
                            else
                                bgColor = "#91590a";//"#530080";
                            break;
                        }
                    case Evento.TipoEvento.Batismo:
                        {
                            bgColor = "#42420b";
                            break;
                        }
                    case Evento.TipoEvento.Casamento:
                        {
                            bgColor = "#10164f";
                            break;
                        }
                    default:
                        break;
                }

                eventoVM.Add(new EventosViewModel()
                {
                    Tipo = evento.Tipo,
                    Id = evento.Id,
                    Descricao = evento.Descricao,
                    CongregacaoId = evento.CongregacaoId,
                    CongregacaoNome = evento.Congregacao.Nome,
                    TipoEventoSelecionado = evento.TipoEventoId,
                    TipoEventoDescr = evento.Tipo == Evento.TipoEvento.Evento || evento.Tipo == Evento.TipoEvento.Curso ? evento.DescrTipoEventoId :
                                              evento.Tipo == Evento.TipoEvento.Batismo ? "Batismo" :
                                              evento.Tipo == Evento.TipoEvento.Casamento ? "Casamento" : "",
                    DataCriacao = evento.DataCriacao,
                    DataInicio = evento.DataHoraInicio.Date,
                    DataFim = evento.Tipo == Evento.TipoEvento.Batismo ? evento.DataHoraInicio.Date : evento.DataHoraFim.Date,
                    HoraInicio = evento.DataHoraInicio.TimeOfDay,
                    HoraFinal = evento.DataHoraFim.TimeOfDay,
                    BgColor = bgColor,
                    IdEventoOriginal = evento.IdEventoOriginal,
                    AlertarEventoMesmoDia = evento.AlertarEventoMesmoDia,
                    IsFeriado = feriados.Any(f => f.DataFeriado == evento.DataHoraInicio.Date)
                });
            }
            return eventoVM;
        }

        public JsonResult Eventos([FromServices] IMemoryCache _cache, string tipoPesq = "")
        {
            IEnumerable<Evento> eventosSel;

            var dashCacheBD = _cache.GetOrCreate($"Dashboard_{UserAppContext.Current.Usuario.Username}", item =>
            {
                item.SlidingExpiration = TimeSpan.FromMinutes(10);

                var dash = new Web.Models.Shared.EventosDashBoardVM
                {
                    Dashboard = _dashboardApp.RetornaDadosDashboard((int)UserAppContext.Current.Usuario.Id),
                    Eventos = _eventoAppService.ListarEventosPorData(DateTime.Now.Date, DateTime.Now.AddDays(30).Date, out List<Feriado> feriados),
                    Feriados = feriados
                };

                return dash;
            });
            var listaEventos = dashCacheBD.Eventos;
            var feriados = dashCacheBD.Feriados;

            if (tipoPesq == "S")
                eventosSel = listaEventos.Where(e => e.CongregacaoId == 1);
            else if (tipoPesq == "N")
            {
                if (UserAppContext.Current.Usuario.Congregacao.Sede)
                    eventosSel = listaEventos.Where(e => e.CongregacaoId != 1);
                else
                    eventosSel = listaEventos.Where(e => e.CongregacaoId == UserAppContext.Current.Usuario.Congregacao.Id);
            }
            else
                eventosSel = listaEventos;

            var eventos = RetornaEventos(eventosSel.ToList(), feriados);
            return Json(new { Result = "OK", Records = eventos, TotalRecordCount = eventos.Count() });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}