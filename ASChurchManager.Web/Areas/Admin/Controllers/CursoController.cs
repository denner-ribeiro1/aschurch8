using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Admin.ViewModels.Curso;
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

namespace ASChurchManager.Web.Areas.Admin.ViewModels
{
    [Rotina(Area.Admin), ControllerAuthorize("Curso")]
    public class CursoController : BaseController
    {
        private readonly IMapper _mapper;
        private ICursoAppService _appService;
        public CursoController(ICursoAppService appService,
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

        [Action(Menu.Cadastros, Menu.Cursos)]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var curso = _appService.GetAll(UserAppContext.Current.Usuario.Id);
                var lcurso = curso.Skip(jtStartIndex).Take(jtPageSize);
                var cursoVM = _mapper.Map<List<CursoVM>>(lcurso);

                return Json(new { Result = "OK", Records = cursoVM, TotalRecordCount = curso.Count() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        public ActionResult Details(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Curso_Details&valor=0");

            var curso = _appService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (curso == null || curso.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var cursoVM = _mapper.Map<CursoVM>(curso);
            cursoVM.Acao = Acao.Read;
            cursoVM.IsReadOnly = true;

            return View(cursoVM);
        }

        public ActionResult Create()
        {
            var cursoVm = new CursoVM
            {
                Acao = Acao.Create
            };
            return View(cursoVm);
        }

        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        [HttpPost]
        public ActionResult Create(CursoVM cursoVM)
        {
            if (cursoVM.DataInicio > cursoVM.DataEncerramento)
                ModelState.AddModelError("DataInicio", "Data de Início deve ser menor ou igual a Data de Encerramento.");
            if (cursoVM.CargaHoraria <= 0)
                ModelState.AddModelError("CargaHoraria", "Carga Horária deve ser maior ou igual a 0.");
            if (ModelState.IsValid)
            {
                var curso = _mapper.Map<Domain.Entities.Curso>(cursoVM);
                _appService.Add(curso);

                this.ShowMessage("Sucesso", "Curso incluído com sucesso!", AlertType.Success);
                return RedirectToAction("Index");
            }

            return View(cursoVM);
        }

        public ActionResult Edit(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Curso_Details&valor=0");

            var curso = _appService.GetById(id, UserAppContext.Current.Usuario.Id);

            if (curso == null || curso.Id == 0)
                return Redirect("/Auth/NaoAutorizado");
            var cursoVM = _mapper.Map<CursoVM>(curso);

            return View(cursoVM);
        }

        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        [HttpPost]
        public ActionResult Edit(CursoVM cursoVM)
        {
            if (cursoVM.DataInicio > cursoVM.DataEncerramento)
                ModelState.AddModelError("DataInicio", "Data de Início deve ser menor ou igual a Data de Encerramento.");
            if (cursoVM.CargaHoraria <= 0)
                ModelState.AddModelError("CargaHoraria", "Carga Horária deve ser maior ou igual a 0.");
            if (ModelState.IsValid)
            {
                var curso = _mapper.Map<Domain.Entities.Curso>(cursoVM);
                _appService.Update(curso);
                this.ShowMessage("Sucesso", "Curso atualizado com sucesso!", AlertType.Success);
                return RedirectToAction("Index");
            }
            return View(cursoVM);
        }

        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        [HttpPost]
        public JsonResult Delete(long id)
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
                    message = "Não foi possível excluir este Curso. Ele está sendo utilizado em outros registros.";
                return Json(new { status = "Erro", mensagem = message, url = "" });
            }
            this.ShowMessage("Sucesso", "Curso excluído com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Curso excluído com sucesso!", url = Url.Action("Index", "Curso") });
        }
    }
}