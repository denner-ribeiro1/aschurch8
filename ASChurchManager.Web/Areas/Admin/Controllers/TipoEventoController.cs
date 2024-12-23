using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Web.Areas.Admin.ViewModels.TipoDeEvento;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Filters.Menu;
using ASChurchManager.Web.Lib;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASChurchManager.Web.Areas.Admin.Controllers
{
    [Rotina(Area.Admin), ControllerAuthorize("TipoEvento")]
    public class TipoEventoController : BaseController
    {
        private ITipoEventoAppService _appService;
        private readonly IMapper _mapper;

        public TipoEventoController(ITipoEventoAppService appService, 
                    IMapper mapper
                    , IMemoryCache cache
                    , IUsuarioLogado usuLog
                    , IConfiguration _configuration
                    , IRotinaAppService _rotinaAppService)
            : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _appService = appService;
            _mapper = mapper;
        }

        // GET: Secretaria/TipoEvento
        [Action(Menu.Cadastros, Menu.TipoEvento)]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var tipoEvento = _appService.GetAll(UserAppContext.Current.Usuario.Id);
                var lperfis = tipoEvento.Skip(jtStartIndex).Take(jtPageSize);
                var tipoEventoCM = _mapper.Map<List<GridTipoEventoViewModel>>(tipoEvento);

                return Json(new { Result = "OK", Records = tipoEventoCM, TotalRecordCount = tipoEvento.Count() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        // GET: Secretaria/TipoEvento/Details/5
        public ActionResult Details(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=TipoEvento_Details&valor=0");

            var tipoEvento = _appService.GetById(id, UserAppContext.Current.Usuario.Id);

            if (tipoEvento == null || tipoEvento.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var tipoEventoVm = _mapper.Map<TipoEventoViewModel>(tipoEvento);
            return View(tipoEventoVm);
        }

        // GET: Secretaria/TipoEvento/Create
        public ActionResult Create()
        {
            var tipoEventoVm = new TipoEventoViewModel();
            return View(tipoEventoVm);
        }

        // POST: Secretaria/TipoEvento/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(TipoEventoViewModel TipoEventoVm)
        {
            if (ModelState.IsValid)
            {
                var tipoEvento = _mapper.Map<TipoEvento>(TipoEventoVm);
                _appService.Add(tipoEvento);
                return RedirectToAction("Index");
            }
            return View(TipoEventoVm);
        }

        // GET: Secretaria/TipoEvento/Edit/5
        public ActionResult Edit(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=TipoEvento_Details&valor=0");

            var tipoEvento = _appService.GetById(id, UserAppContext.Current.Usuario.Id);

            if (tipoEvento == null || tipoEvento.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var tipoEventoVm = _mapper.Map<TipoEventoViewModel>(tipoEvento);
            return View(tipoEventoVm);
        }

        // POST: Secretaria/TipoEvento/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(TipoEventoViewModel TipoEventoVm)
        {
            if (ModelState.IsValid)
            {
                var tipoEvento = _mapper.Map<TipoEvento>(TipoEventoVm);
                _appService.Update(tipoEvento);
                return RedirectToAction("Index");
            }
            return View(TipoEventoVm);
        }

        // POST: Secretaria/TipoEvento/Delete/5
        [HttpPost]
        public ActionResult Delete(long id)
        {
            try
            {
                _appService.Delete(id);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint") ||
                    ex.Message.Contains("A instrução DELETE conflitou com a restrição do REFERENCE "))
                    message = "Não foi possível excluir este Tipo de Evento. Ele está sendo utilizado em outros registros.";
                return Json(new { status = "Erro", mensagem = message, url = "" });
            }
            this.ShowMessage("Sucesso", "Tipo de Evento excluído com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Tipo de Evento excluído com sucesso!", url = Url.Action("Index", "TipoEvento") });
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
