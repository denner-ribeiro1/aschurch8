using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Batismo;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Casamento;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Eventos;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.PastorCelebrante;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Filters.Menu;
using ASChurchManager.Web.Lib;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ASChurchManager.Web.Areas.Secretaria.Controllers
{
    [Rotina(Area.Secretaria), ControllerAuthorize("Eventos")]
    public class EventosController : BaseController
    {
        private readonly IEventosAppService _appService;
        private readonly ITipoEventoAppService _tipoEventoAppService;
        private readonly ICongregacaoAppService _congregacaoAppService;
        private readonly ICasamentoAppService _casamentoAppService;
        private readonly IBatismoAppService _batismoAppService;
        private readonly IPresencaAppService _presencaAppService;
        private readonly IMapper _mapper;

        public EventosController(IEventosAppService appService
                         , ITipoEventoAppService tipoEventoAppService
                         , ICongregacaoAppService congregacaoAppService
                         , ICasamentoAppService casamentoAppService
                         , IBatismoAppService batismoAppService
                         , IMapper mapper
                         , IMemoryCache cache
                         , IUsuarioLogado usuLog
                         , IConfiguration _configuration
                         , IRotinaAppService _rotinaAppService
                         , IPresencaAppService presencaAppService)
            : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _appService = appService;
            _tipoEventoAppService = tipoEventoAppService;
            _congregacaoAppService = congregacaoAppService;
            _casamentoAppService = casamentoAppService;
            _batismoAppService = batismoAppService;
            _mapper = mapper;
            _presencaAppService = presencaAppService;
        }

        [Action(Menu.Eventos)]
        public ActionResult Index()
        {
            return View("CalendarioEventos");
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
                    TipoEventoDescr = evento.Tipo == Evento.TipoEvento.Evento ? evento.DescrTipoEventoId :
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

        public JsonResult Eventos(string start, string end)
        {
            var dataInicial = string.IsNullOrWhiteSpace(start) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) : DateTime.Parse(start);
            var dataFinal = string.IsNullOrWhiteSpace(end) ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)) : DateTime.Parse(end);
            var lstEventos = _appService.ListarEventosPorData(dataInicial, dataFinal, out List<Feriado> feriados).ToList();
            var eventos = RetornaEventos(lstEventos, feriados);
            return Json(new { eventos, feriados });
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Persistir([FromServices] ILogger<NascimentoController> logger, EventosViewModel eventoVm)
        {
            try
            {
                var evento = new Evento()
                {
                    Id = eventoVm.Id,
                    CongregacaoId = (int)eventoVm.CongregacaoId,
                    TipoEventoId = (int)eventoVm.TipoEventoSelecionado,
                    Descricao = eventoVm.Descricao,
                    Observacoes = eventoVm.Observacoes,
                    DataHoraInicio = eventoVm.DataInicio.Value.Add(eventoVm.HoraInicio),
                    DataHoraFim = eventoVm.DataInicio.Value.Add(eventoVm.HoraFinal),
                    Frequencia = (Evento.TipoFrequencia)Enum.Parse(typeof(Evento.TipoFrequencia), eventoVm.TipoFrequenciaSelecionado.ToString()),
                    Quantidade = eventoVm.Quantidade,
                    AlertarEventoMesmoDia = eventoVm.AlertarEventoMesmoDia
                };
                evento.Id = _appService.Add(evento);
                return Json(new { status = "OK", msg = $"Evento salvo com sucesso.", id = evento.Id });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "Erro", msg = $"Falha ao salvar o Evento. Erro: {ex.Message}" });
            }
        }

        public JsonResult GetEventos(string ano, string mes, string tipoPesquisa)
        {
            int.TryParse(tipoPesquisa, out int tpPesq);
            int codigoCongr = (int)UserAppContext.Current.Usuario.Congregacao.Id;
            var lstEventos = _appService.GetEventos(int.Parse(ano), int.Parse(mes), tpPesq, codigoCongr, out List<Feriado> feriados).ToList();
            var eventos = RetornaEventos(lstEventos, feriados);
            return Json(new { eventos, feriados });
        }

        public ActionResult Create(string clickDate)
        {
            var congrSede = _congregacaoAppService.GetSede();
            var _lstTipoEventos = _tipoEventoAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList();
            if (clickDate == "add")
            {
                var evento = new EventosViewModel()
                {
                    Acao = Acao.Create,
                    Tipo = Evento.TipoEvento.Evento,
                    Titulo = $"Inclusão de Eventos",
                    Id = 0,
                    CongregacaoId = UserAppContext.Current.Usuario.Congregacao.Id,
                    CongregacaoNome = UserAppContext.Current.Usuario.Congregacao.Nome,
                    DataInicio = null,
                    SelectTiposEvento = _lstTipoEventos.ToSelectList("Id", "Descricao").ToList(),
                    CodigoCongregacaoSede = (int)congrSede.Id
                };
                return View("Evento", evento);
            }
            else
            {
                var dt = Convert.ToDateTime(clickDate);
                var evento = new EventosViewModel()
                {
                    Acao = Acao.Create,
                    Tipo = Evento.TipoEvento.Evento,
                    Titulo = $"Inclusão de Eventos - Data: {dt.Date.ToShortDateString()}",
                    Id = 0,
                    CongregacaoId = UserAppContext.Current.Usuario.Congregacao.Id,
                    CongregacaoNome = UserAppContext.Current.Usuario.Congregacao.Nome,
                    DataInicio = dt,
                    SelectTiposEvento = _lstTipoEventos.ToSelectList("Id", "Descricao").ToList(),
                    CodigoCongregacaoSede = (int)congrSede.Id
                };
                return View("Evento", evento);
            }

        }

        public ActionResult Edit(int id, string tipoEvento)
        {
            var tipoEvnt = (Evento.TipoEvento)Enum.Parse(typeof(Evento.TipoEvento), tipoEvento);
            if (tipoEvnt == Evento.TipoEvento.Evento)
            {
                var evento = _appService.GetById(id, UserAppContext.Current.Usuario.Id);
                var _lstTipoEventos = _tipoEventoAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList();
                var congrSede = _congregacaoAppService.GetSede();

                var editar = !(evento.DataHoraInicio.Date >= DateTime.Now.Date &&
                                ((UserAppContext.Current.Usuario.Congregacao.Sede) || (evento.CongregacaoId == UserAppContext.Current.Usuario.Congregacao.Id)));

                var eventoVm = new EventosViewModel()
                {
                    Acao = Acao.Update,
                    IsReadOnly = editar,
                    Tipo = Evento.TipoEvento.Evento,
                    AlertarEventoMesmoDia = evento.AlertarEventoMesmoDia,
                    Id = evento.Id,
                    TipoEventoSelecionado = evento.TipoEventoId,
                    Descricao = evento.Descricao,
                    CongregacaoId = evento.CongregacaoId,
                    CongregacaoNome = evento.Congregacao.Nome,
                    DataInicio = evento.DataHoraInicio.Date,
                    HoraInicio = evento.DataHoraInicio.TimeOfDay,
                    HoraFinal = evento.DataHoraFim.TimeOfDay,
                    Observacoes = evento.Observacoes,
                    DataCriacao = evento.DataCriacao,
                    TipoFrequenciaSelecionado = (int)evento.Frequencia,
                    Quantidade = evento.Quantidade,
                    SelectTiposEvento = _lstTipoEventos.ToSelectList("Id", "Descricao").ToList(),
                    IdEventoOriginal = evento.IdEventoOriginal,
                    CodigoCongregacaoSede = (int)congrSede.Id
                };
                eventoVm.Titulo = $"Evento - Data: {evento.DataHoraInicio.Date.ToShortDateString()}";
                if ((UserAppContext.Current.Usuario.Congregacao.Id == congrSede.Id || evento.CongregacaoId == UserAppContext.Current.Usuario.Congregacao.Id))
                    eventoVm.Titulo = $"Alteração de Evento - Data: {evento.DataHoraInicio.Date.ToShortDateString()}";

                return View("Evento", eventoVm);
            }
            else if (tipoEvnt == Evento.TipoEvento.Casamento)
            {
                var casamento = _casamentoAppService.GetById(id);
                var casamentoVM = new CasamentoViewModel()
                {
                    CasamentoId = casamento.Id,
                    CongregacaoId = casamento.CongregacaoId,
                    CongregacaoNome = casamento.Congregacao.Nome,
                    PastorMembro = casamento.Congregacao.CongregacaoResponsavelId == casamento.PastorId,
                    PastorId = casamento.PastorId,
                    PastorNome = casamento.PastorNome,
                    DataCasamento = casamento.DataHoraInicio,
                    HoraInicio = casamento.DataHoraInicio.TimeOfDay,
                    HoraFim = casamento.DataHoraFinal.TimeOfDay,
                    NoivoMembro = casamento.NoivoId > 0,
                    NoivoId = casamento.NoivoId,
                    NoivoNome = casamento.NoivoNome,
                    PaiNoivoMembro = casamento.PaiNoivoId > 0,
                    PaiNoivoId = casamento.PaiNoivoId,
                    PaiNoivoNome = casamento.PaiNoivoNome,
                    MaeNoivoMembro = casamento.MaeNoivoId > 0,
                    MaeNoivoId = casamento.MaeNoivoId,
                    MaeNoivoNome = casamento.MaeNoivoNome,
                    NoivaMembro = casamento.NoivaId > 0,
                    NoivaId = casamento.NoivaId,
                    NoivaNome = casamento.NoivaNome,
                    PaiNoivaMembro = casamento.PaiNoivaId > 0,
                    PaiNoivaId = casamento.PaiNoivaId,
                    PaiNoivaNome = casamento.PaiNoivaNome,
                    MaeNoivaMembro = casamento.MaeNoivaId > 0,
                    MaeNoivaId = casamento.MaeNoivaId,
                    MaeNoivaNome = casamento.MaeNoivaNome
                };


                //var casamentoVm = _mapper.Map<CasamentoViewModel>(casamento);
                return View("EventoCasamento", casamentoVM);
            }
            else if (tipoEvnt == Evento.TipoEvento.Batismo)
            {
                var PastorCelebrante = new List<GridPastorCelebranteViewModel>();
                var batismo = _batismoAppService.GetById(id, UserAppContext.Current.Usuario.Id);
                var pastorCelebrante = _batismoAppService.ListarPastorCelebrante(id);

                int i = 1;
                pastorCelebrante.ToList().ForEach(p => PastorCelebrante.Add(
                   new GridPastorCelebranteViewModel()
                   {
                       Id = i++,
                       MembroId = p.Id,
                       Nome = p.Nome
                   }));

                var batismoVm = _mapper.Map<ConfiguracaoBatismoViewModel>(batismo);
                batismoVm.PastoresCelebrantes = PastorCelebrante;
                return View("EventoBatismo", batismoVm);
            }
            else if (tipoEvnt == Evento.TipoEvento.Curso)
            {
                var _lstTipoEventos = _tipoEventoAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList();
                var congrSede = _congregacaoAppService.GetSede();

                var evento = _presencaAppService.ConsultarPresencaIdData(id).FirstOrDefault();

                var eventoVm = new EventosViewModel()
                {
                    Acao = Acao.Read,
                    IsReadOnly = true,
                    Tipo = Evento.TipoEvento.Curso,
                    Id = evento.Id,
                    TipoEventoSelecionado = evento.TipoEventoId,
                    Descricao = evento.Descricao,
                    CongregacaoId = evento.CongregacaoId,
                    CongregacaoNome = evento.Congregacao.Nome,
                    DataInicio = evento.Datas.FirstOrDefault().DataHoraInicio.Date,
                    HoraInicio = evento.Datas.FirstOrDefault().DataHoraInicio.TimeOfDay,
                    HoraFinal = evento.Datas.FirstOrDefault().DataHoraFim.TimeOfDay,
                    Observacoes = $"A data máxima para inscrição no Curso/Evento é {evento.DataMaxima.ToShortDateString()}",
                    DataCriacao = evento.DataCriacao,
                    SelectTiposEvento = _lstTipoEventos.ToSelectList("Id", "Descricao").ToList(),
                    CodigoCongregacaoSede = (int)congrSede.Id
                };
                eventoVm.Titulo = $"Curso/Evento - Data: {evento.Datas.FirstOrDefault().DataHoraInicio.Date.ToShortDateString()}";
                return View("Evento", eventoVm);
            }
            return null;
        }

        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Delete([FromServices] ILogger<EventosController> logger, int id, bool excluirVinc = false)
        {
            try
            {
                _appService.Delete(id, excluirVinc);
                return Json(new { status = "OK", msg = $"Evento excluído com sucesso." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "Erro", msg = $"Falha ao Excluir o Evento. Erro: {ex.Message}" });
            }
        }

        public JsonResult ValidarEventosObrigatorio([FromServices] ILogger<EventosController> logger,
            string dataInicial, string dataFinal, int congregacao = 0,
            string frequencia = "", int quantidade = 0)
        {
            try
            {
                if (congregacao == 0)
                    throw new Erro("Congregação é de preenchimento obrigatório.");

                if (string.IsNullOrEmpty(dataInicial))
                    throw new Erro("Hora do Início do Evento é de preenchimento obrigatório.");
                if (string.IsNullOrEmpty(dataFinal))
                    throw new Erro("Hora do Término do Evento é de preenchimento obrigatório.");
                if (!DateTimeOffset.TryParse(dataInicial, out var dtIni))
                    throw new Erro("Hora do Início do Evento inválida.");
                if (!DateTimeOffset.TryParse(dataFinal, out var dtFin))
                    throw new Erro("Hora do Início do Evento inválida.");
                if (dtIni > dtFin)
                    throw new Erro("Hora do Início deve ser menor que a Hora Final do Evento.");

                var Frequencia = Evento.TipoFrequencia.Unico;
                if (!string.IsNullOrWhiteSpace(frequencia))
                {
                    Frequencia = (Evento.TipoFrequencia)Enum.Parse(typeof(Evento.TipoFrequencia), frequencia.ToString());
                }

                var evento = _appService.ListarEventosObrigatorio(dtIni, dtFin, congregacao, Frequencia, quantidade);
                if (evento.Any(p => p.Tipo == Evento.TipoEvento.Batismo))
                {
                    var msg = $"Batismo agendado para a Data: <br>" +
                              $"Inicio: {evento.FirstOrDefault().DataHoraInicio.Date.ToShortDateString()} às {evento.FirstOrDefault().DataHoraInicio:HH:mm}<br><br>" +
                              $"<b><font color='red'>Deseja realmente continuar com o cadastro?</font></b>";
                    return Json(new { status = "OK", msg });
                }

                if (evento.Any(p => p.Tipo == Evento.TipoEvento.Evento))
                {
                    var e = evento.FirstOrDefault();
                    var msg = $"A Sede cadastrou o Evento abaixo como presença obrigatória: <br><br>" +
                              $"Evento: {e.Descricao} <br>" +
                              $"Inicio: {e.DataHoraInicio.Date.ToShortDateString()} {e.DataHoraInicio:HH:mm}<br>" +
                              $"Fim: {e.DataHoraFim.Date.ToShortDateString()} {e.DataHoraFim:HH:mm}<br><br>" +
                              $"<b><font color='red'>Deseja realmente continuar com o cadastro?</font></b>";
                    return Json(new { status = "OK", msg });
                }
                return Json(new { status = "OK", msg = "" });
            }
            catch (Erro ex)
            {
                return Json(new { status = "Erro", msg = $"Falha ao Validar o Evento. Erro: {ex.Message}" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "Erro", msg = $"Falha ao Validar o Evento. Erro: {ex.Message}" });
            }
        }
    }
}
