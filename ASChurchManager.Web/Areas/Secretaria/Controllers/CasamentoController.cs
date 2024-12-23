using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Casamento;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Filters.Menu;
using ASChurchManager.Web.Lib;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASChurchManager.Web.Areas.Secretaria.Controllers
{
    [Rotina(Area.Secretaria), ControllerAuthorize("Casamento")]
    public class CasamentoController : BaseController
    {
        private ICasamentoAppService _appService;
        private ICongregacaoAppService _appCongregacao;
        private readonly IMapper _mapper;

        public CasamentoController(ICasamentoAppService appService
                                   , ICongregacaoAppService appCongregacao
                                   , IMapper mapper
                                   , IMemoryCache cache
                                   , IUsuarioLogado usuLog
                                   , IConfiguration _configuration
                                   , IRotinaAppService _rotinaAppService)
            : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _appService = appService;
            _appCongregacao = appCongregacao;
            _mapper = mapper;
        }

        [Action(Menu.Casamento)]
        public ActionResult Index()
        {
            var casamentoVM = new IndexCasamentoVM();
            var _congregacoes = new List<SelectListItem>();
            _appCongregacao.GetAll(UserAppContext.Current.Usuario.Id).OrderBy(p => p.Nome).ToList()
                .ForEach(cong =>
                    casamentoVM.ListaCongregacoes.Add(new SelectListItem()
                    {
                        Text = cong.Nome,
                        Value = cong.Id.ToString()
                    }));

            return View(casamentoVM);
        }

        [HttpPost]
        public JsonResult GetList([FromServices] ILogger<CasamentoController> logger,
            string filtro = "", string conteudo = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var membros = _appService.ListarCasamentoPaginado(jtPageSize, jtStartIndex, jtSorting, filtro, conteudo, UserAppContext.Current.Usuario.Id, out int qtdRows).ToList();
                var membrosVM = new List<GridCasamentoViewModel>();
                membros.ForEach(p => membrosVM.Add(new GridCasamentoViewModel()
                {
                    Id = p.Id,
                    CongregacaoNome = p.Congregacao.Nome,
                    DataCasamento = p.DataHoraInicio.Date,
                    HoraInicio = p.DataHoraInicio.TimeOfDay.ToString(),
                    NoivaNome = p.NoivaNome,
                    NoivoNome = p.NoivoNome
                }));

                return Json(new { Result = "OK", Records = membrosVM, TotalRecordCount = qtdRows });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        // GET: Secretaria/Casamento/Details/5
        public ActionResult Details(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Casamento_Details&valor=0");

            var casamento = _appService.GetById(id, UserAppContext.Current.Usuario.Id);

            if (casamento == null || casamento.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var casamentoVm = _mapper.Map<CasamentoViewModel>(casamento);
            var congr = _appCongregacao.GetById(casamento.CongregacaoId);
            if (casamentoVm.PastorId > 0 && casamentoVm.PastorId == congr.PastorResponsavelId)
                casamentoVm.PastorMembro = true;
            casamentoVm.PastorNome = casamento.PastorNome;
            casamentoVm.PastorId = casamentoVm.PastorId == 0 ? null : casamentoVm.PastorId;
            casamentoVm.NoivoId = casamentoVm.NoivoId == 0 ? null : casamentoVm.NoivoId;
            casamentoVm.PaiNoivoId = casamentoVm.PaiNoivoId == 0 ? null : casamentoVm.PaiNoivoId;
            casamentoVm.MaeNoivoId = casamentoVm.MaeNoivoId == 0 ? null : casamentoVm.MaeNoivoId;
            casamentoVm.NoivaId = casamentoVm.NoivaId == 0 ? null : casamentoVm.NoivaId;
            casamentoVm.PaiNoivaId = casamentoVm.PaiNoivaId == 0 ? null : casamentoVm.PaiNoivaId;
            casamentoVm.MaeNoivaId = casamentoVm.MaeNoivaId == 0 ? null : casamentoVm.MaeNoivaId;
            casamentoVm.IsReadOnly = true;
            return View(casamentoVm);
        }

        // GET: Secretaria/Casamento/Create
        public ActionResult Create()
        {
            // Inicializa o ViewModel para não dar erro na view por causa do titulo da VM
            var membroVm = new CasamentoViewModel();
            if (UserAppContext.Current.Usuario.Congregacao.Id > 0)
            {
                membroVm.CongregacaoId = UserAppContext.Current.Usuario.Congregacao.Id;
                membroVm.CongregacaoNome = UserAppContext.Current.Usuario.Congregacao.Nome;

                var congr = _appCongregacao.GetById((long)membroVm.CongregacaoId, UserAppContext.Current.Usuario.Id);
                if (congr != null)
                {
                    membroVm.PastorMembro = true;
                    membroVm.PastorId = congr.PastorResponsavelId;
                    membroVm.PastorNome = congr.PastorResponsavelNome;
                }

                membroVm.NoivoMembro = true;
                membroVm.PaiNoivoMembro = true;
                membroVm.MaeNoivoMembro = true;

                membroVm.NoivaMembro = true;
                membroVm.PaiNoivaMembro = true;
                membroVm.MaeNoivaMembro = true;
            }
            return View(membroVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Create([FromServices] ILogger<CasamentoController> logger, CasamentoViewModel casamentoVm)
        {
            try
            {
                ValidarCasamento(casamentoVm);
                var casamento = _mapper.Map<Casamento>(casamentoVm);
                casamento.Congregacao.Id = (long)casamentoVm.CongregacaoId;
                casamento.Congregacao.Nome = casamentoVm.CongregacaoNome;
                casamento.Id = (long)casamentoVm.CasamentoId;
                _appService.Add(casamento);

                this.ShowMessage("Sucesso", "Casamento incluído com sucesso!", AlertType.Success);
                return Json(new { status = "OK", msg = "Casamento incluído com sucesso!", url = Url.Action("Index", "Casamento", new { Area = "Secretaria" }) });
            }
            catch (Erro ex)
            {
                var msgAlert = $"Falha ao incluir o Casamento - Erro: {ex.Message}";
                return Json(new { status = "Erro", msg = msgAlert, url = Url.Action("Index", "Casamento", new { Area = "Secretaria" }) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                var msgAlert = $"Falha ao incluir o Casamento - Erro: {ex.Message}";
                return Json(new { status = "Erro", msg = msgAlert, url = Url.Action("Index", "Casamento", new { Area = "Secretaria" }) });
            }
        }

        // GET: Secretaria/Casamento/Edit/5
        public ActionResult Edit(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Casamento_Edit&valor=0");

            var casamento = _appService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (casamento == null || casamento.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var casamentoVm = _mapper.Map<CasamentoViewModel>(casamento);
            casamentoVm.NoivoId = casamentoVm.NoivoId == 0 ? null : casamentoVm.NoivoId;
            casamentoVm.PaiNoivoId = casamentoVm.PaiNoivoId == 0 ? null : casamentoVm.PaiNoivoId;
            casamentoVm.MaeNoivoId = casamentoVm.MaeNoivoId == 0 ? null : casamentoVm.MaeNoivoId;
            casamentoVm.NoivaId = casamentoVm.NoivaId == 0 ? null : casamentoVm.NoivaId;
            casamentoVm.PaiNoivaId = casamentoVm.PaiNoivaId == 0 ? null : casamentoVm.PaiNoivaId;
            casamentoVm.MaeNoivaId = casamentoVm.MaeNoivaId == 0 ? null : casamentoVm.MaeNoivaId;

            var congr = _appCongregacao.GetById(casamento.CongregacaoId);
            if (casamentoVm.PastorId > 0 && casamentoVm.PastorId == congr.PastorResponsavelId)
                casamentoVm.PastorMembro = true;
            casamentoVm.PastorId = casamentoVm.PastorId == 0 ? null : casamentoVm.PastorId;
            casamentoVm.IsReadOnly = false;

            return View(casamentoVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Edit([FromServices] ILogger<CasamentoController> logger, CasamentoViewModel casamentoVm)
        {
            try
            {
                ValidarCasamento(casamentoVm);
                var casamento = _mapper.Map<Casamento>(casamentoVm);
                casamento.Congregacao.Id = (long)casamentoVm.CongregacaoId;
                casamento.Congregacao.Nome = casamentoVm.CongregacaoNome;
                casamento.PastorNome = casamentoVm.PastorNome;
                _appService.Update(casamento);

                this.ShowMessage("Sucesso", "Casamento atualizado com sucesso!", AlertType.Success);
                return Json(new { status = "OK", msg = "Casamento atualizado com sucesso!", url = Url.Action("Index", "Casamento", new { Area = "Secretaria" }) });
            }
            catch (Erro ex)
            {
                var msgAlert = $"Falha ao atualizar o Casamento - Erro: {ex.Message}";
                return Json(new { status = "Erro", msg = msgAlert, url = Url.Action("Index", "Casamento", new { Area = "Secretaria" }) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                var msgAlert = $"Falha ao atualizar o Casamento - Erro: {ex.Message}";
                return Json(new { status = "Erro", msg = msgAlert, url = Url.Action("Index", "Casamento", new { Area = "Secretaria" }) });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Delete([FromServices] ILogger<CasamentoController> logger, long id)
        {
            try
            {
                _appService.Delete(id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "Erro", mensagem = ex.Message, url = "" });
            }
            this.ShowMessage("Sucesso", "Casamento excluído com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Casamento excluído com sucesso!", url = Url.Action("Index", "Casamento") });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _appService = null;
            }
            base.Dispose(disposing);
        }

        public JsonResult VerificarCasamento(CasamentoViewModel casamentoVm)
        {
            try
            {
                ValidarCasamento(casamentoVm);

                var casamento = _mapper.Map<Casamento>(casamentoVm);
                casamento.Id = casamentoVm.CasamentoId;
                casamento.Congregacao.Id = (long)casamentoVm.CongregacaoId;
                casamento.Congregacao.Nome = casamentoVm.CongregacaoNome;

                var res = _appService.VerificarCasamentoCongregacao(casamento);
                casamentoVm.CasamentoId = res.Id;

                return Json(new { status = "OK", dados = casamentoVm });
            }
            catch (Exception ex)
            {
                return Json(new { status = "ERRO", mensagem = ex.Message });
            }
        }

        private bool ValidarCasamento(CasamentoViewModel casamento)
        {
            if (string.IsNullOrWhiteSpace(casamento.CongregacaoNome))
                throw new Erro("Congregação é de preenchimento obrigatório.");
            if (casamento.DataCasamento == null || casamento.DataCasamento == DateTime.MinValue)
                throw new Erro("Data de Casamento é de preenchimento obrigatório.");
            if (casamento.HoraInicio == null)
                throw new Erro("Hora do Início do Casamento é de preenchimento obrigatório.");
            if (casamento.HoraFim == null)
                throw new Erro("Hora do Término do Casamento é de preenchimento obrigatório.");

            var dateIni = casamento.DataCasamento + casamento.HoraInicio;
            var dateFim = casamento.DataCasamento + casamento.HoraFim;
            if (dateIni > dateFim)
                throw new Erro("Hora do Início é maior que a Hora Final do Casamento.");

            return true;

        }
    }
}
