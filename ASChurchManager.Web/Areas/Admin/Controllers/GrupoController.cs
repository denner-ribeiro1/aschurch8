using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Web.Areas.Admin.ViewModels.Grupo;
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
    [Rotina(Area.Admin)]
    [ControllerAuthorize("Grupo")]
    public class GrupoController : BaseController
    {
        private readonly IMapper _mapper;
        private IGrupoAppService _appService;

        public GrupoController(IGrupoAppService appService,
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

        [Action(Menu.Cadastros, Menu.Grupo)]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var grupo = _appService.GetAll(UserAppContext.Current.Usuario.Id);
                var lgrupo = grupo.Skip(jtStartIndex).Take(jtPageSize);
                var grupoVM = _mapper.Map<List<GridGrupoViewModel>>(lgrupo);

                return Json(new { Result = "OK", Records = grupoVM, TotalRecordCount = grupo.Count() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }
        public ActionResult Details(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Grupo_Details&valor=0");

            var grupo = _appService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (grupo == null || grupo.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var cargoVm = _mapper.Map<GrupoViewModel>(grupo);
            return View(cargoVm);
        }

        public ActionResult Create()
        {
            var grupoVm = new GrupoViewModel();
            return View(grupoVm);
        }

        [HttpPost]

        public ActionResult Create(GrupoViewModel GrupoVm)
        {
            if (ModelState.IsValid)
            {
                var grupo = _mapper.Map<Grupo>(GrupoVm);
                _appService.Add(grupo);
                return RedirectToAction("Index");
            }

            return View(GrupoVm);
        }

        public ActionResult Edit(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Grupo_Details&valor=0");

            var grupo = _appService.GetById(id, UserAppContext.Current.Usuario.Id);

            if (grupo == null || grupo.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var grupoVm = _mapper.Map<GrupoViewModel>(grupo);
            return View(grupoVm);
        }

        [HttpPost]

        public ActionResult Edit(GrupoViewModel grupoVm)
        {
            if (ModelState.IsValid)
            {
                var grupo = _mapper.Map<Grupo>(grupoVm);
                _appService.Update(grupo);
                return RedirectToAction("Index");
            }
            return View(grupoVm);
        }

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

                if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    message = "Não é possível excluir este Grupo. Ele está sendo usada em outros registros.";
                }
                this.ShowMessage("Erro", message, AlertType.Error);
            }
            this.ShowMessage("Sucesso", "Grupo excluído com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Grupo excluído com sucesso!", url = Url.Action("Index", "Grupo") });
        }

    }
}