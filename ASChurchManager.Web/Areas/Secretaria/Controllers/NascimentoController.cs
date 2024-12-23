using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Nascimento;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Filters.Menu;
using ASChurchManager.Web.Lib;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ASChurchManager.Web.Areas.Secretaria.Controllers
{
    [Rotina(Area.Secretaria), ControllerAuthorize("Nascimento")]
    public class NascimentoController : BaseController
    {
        private INascimentoAppService _appService;
        private ICongregacaoAppService _appCongregacao;
        private readonly IMapper _mapper;

        public NascimentoController(INascimentoAppService appService
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

        // GET: Secretaria/Nascimento
        [ActionAttribute(Menu.Nascimento)]
        public ActionResult Index()
        {
            return View(new IndexNascVM());
        }

        [HttpPost]
        public JsonResult GetList([FromServices] ILogger<NascimentoController> logger, string filtro = "", string conteudo = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var nasc = _appService.ListarNascimentoPaginado(jtPageSize, jtStartIndex, out int qtdRows, jtSorting, filtro, conteudo, UserAppContext.Current.Usuario.Id).ToList();
                return Json(new { Result = "OK", Records = nasc, TotalRecordCount = qtdRows });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { Result = "ERROR", ex.Message });
            }

        }

        // GET: Secretaria/Nascimento/Details/5
        public ActionResult Details(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Nascimento_Details&valor=0");

            var nascimento = _appService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (nascimento == null || nascimento.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var nascimentoVm = _mapper.Map<NascimentoViewModel>(nascimento);
            var congr = _appCongregacao.GetById(nascimento.CongregacaoId);
            if (nascimentoVm.PastorId > 0 && nascimentoVm.PastorId == congr.PastorResponsavelId)
                nascimentoVm.PastorMembro = true;
            nascimentoVm.PastorId = nascimentoVm.PastorId == 0 ? null : nascimentoVm.PastorId;
            nascimentoVm.IdMembroPai = nascimentoVm.IdMembroPai == 0 ? null : nascimentoVm.IdMembroPai;
            nascimentoVm.IdMembroMae = nascimentoVm.IdMembroMae == 0 ? null : nascimentoVm.IdMembroMae;
            nascimentoVm.IsReadOnly = true;

            return View(nascimentoVm);
        }

        // GET: Secretaria/Nascimento/Create
        public ActionResult Create()
        {
            var nascimentoVm = new NascimentoViewModel();
            if (UserAppContext.Current.Usuario.Congregacao.Id > 0)
            {
                nascimentoVm.CongregacaoId = UserAppContext.Current.Usuario.Congregacao.Id;
                nascimentoVm.CongregacaoNome = UserAppContext.Current.Usuario.Congregacao.Nome;

                var congr = _appCongregacao.GetById((long)nascimentoVm.CongregacaoId, UserAppContext.Current.Usuario.Id);
                if (congr != null)
                {
                    nascimentoVm.PastorMembro = true;
                    nascimentoVm.PastorId = congr.PastorResponsavelId;
                    nascimentoVm.Pastor = congr.PastorResponsavelNome;
                }
                nascimentoVm.PaiMembro = true;
                nascimentoVm.MaeMembro = true;
            }
            return View(nascimentoVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Create([FromServices] ILogger<NascimentoController> logger, NascimentoViewModel nascimentoVm)
        {
            try
            {
                var nascimento = _mapper.Map<Nascimento>(nascimentoVm);
                _appService.Add(nascimento);
                this.ShowMessage("Sucesso", "Nascimento incluído com sucesso!", AlertType.Success);
                return Json(new { status = "OK", msg = "Nascimento incluído com sucesso!", url = Url.Action("Index", "Nascimento", new { Area = "Secretaria" }) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                var msgAlert = $"Falha ao incluir o Nascimento - Erro: {ex.Message}";
                return Json(new { status = "Erro", msg = msgAlert });
            }
        }

        // GET: Secretaria/Nascimento/Edit/5
        public ActionResult Edit(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Nascimento_Edit&valor=0");

            var nascimento = _appService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (nascimento == null || nascimento.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var nascimentoVm = _mapper.Map<NascimentoViewModel>(nascimento);
            var congr = _appCongregacao.GetById(nascimento.CongregacaoId);
            if (nascimentoVm.PastorId > 0 && nascimentoVm.PastorId == congr.PastorResponsavelId)
                nascimentoVm.PastorMembro = true;
            nascimentoVm.PastorId = nascimentoVm.PastorId == 0 ? null : nascimentoVm.PastorId;
            nascimentoVm.IdMembroPai = nascimentoVm.IdMembroPai == 0 ? null : nascimentoVm.IdMembroPai;
            nascimentoVm.IdMembroMae = nascimentoVm.IdMembroMae == 0 ? null : nascimentoVm.IdMembroMae;
            nascimentoVm.IsReadOnly = false;
            return View(nascimentoVm);
        }

        // POST: Secretaria/Nascimento/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Edit([FromServices] ILogger<NascimentoController> logger, NascimentoViewModel nascimentoVm)
        {
            try
            {
                var nascimento = _mapper.Map<Nascimento>(nascimentoVm);
                _appService.Update(nascimento);


                this.ShowMessage("Sucesso", "Nascimento atualizado com sucesso!", AlertType.Success);
                return Json(new { status = "OK", msg = "Nascimento atualizado com sucesso!", url = Url.Action("Index", "Nascimento", new { Area = "Secretaria" }) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                var msgAlert = $"Falha ao atualizar o Nascimento - Erro: {ex.Message}";
                return Json(new { status = "Erro", msg = msgAlert });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Delete([FromServices] ILogger<NascimentoController> logger, long id)
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
            this.ShowMessage("Sucesso", "Nascimento excluído com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Nascimento excluído com sucesso!", url = Url.Action("Index", "Nascimento") });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _appService = null;
            }
            base.Dispose(disposing);
        }
    }
}
