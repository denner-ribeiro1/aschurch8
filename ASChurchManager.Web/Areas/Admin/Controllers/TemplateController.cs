using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Web.Areas.Admin.ViewModels.Template;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Filters.Menu;
using ASChurchManager.Web.Lib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace ASChurchManager.Web.Areas.Admin.Controllers
{
    [Rotina(Area.Admin), ControllerAuthorize("Template")]
    public class TemplateController : BaseController
    {
        private readonly ITemplateAppService _templateAppService;
        public TemplateController(ITemplateAppService templateAppService
                        , IMemoryCache cache
                        , IUsuarioLogado usuLog
                        , IConfiguration _configuration
                        , IRotinaAppService _rotinaAppService)
            : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _templateAppService = templateAppService;
        }

        // GET: Admin/Template
        [Action(Menu.Cadastros, Menu.Template)]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var templates = _templateAppService.GetAll(UserAppContext.Current.Usuario.Id);
                var ltemplates = templates.Skip(jtStartIndex).Take(jtPageSize);
                var templateVM = ltemplates.Select(template => new GridTemplateViewModel()
                {
                    Id = template.Id,
                    Nome = template.Nome,
                    DataAlteracao = template.DataAlteracao,
                    DataCriacao = template.DataCriacao
                }).ToList();

                return Json(new { Result = "OK", Records = templateVM, TotalRecordCount = templateVM.Count() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        public ActionResult Details(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Template_Details&valor=0");

            Template template = _templateAppService.GetById(id, UserAppContext.Current.Usuario.Id);

            if (template == null || template.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var templateVm = new TemplateViewModel()
            {
                Id = template.Id,
                Nome = template.Nome,
                TipoTemplate = template.Tipo,
                Conteudo = template.Conteudo,
                DataCriacao = template.DataCriacao,
                DataAlteracao = template.DataAlteracao,
                MargemAcima = template.MargemAcima,
                MargemAbaixo = template.MargemAbaixo,
                MargemDireita = template.MargemDireita,
                MargemEsquerda = template.MargemEsquerda
            };

            return View(templateVm);
        }

        public ActionResult Create()
        {
            var templateVm = new TemplateViewModel()
            {
                TagsDisponiveis = new Template().TagsDisponiveis
            };
            return View(templateVm);

        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult Create(TemplateViewModel templateVm)
        {
            try
            {
                var template = new Template()
                {
                    Id = templateVm.Id,
                    Nome = templateVm.Nome,
                    Conteudo = templateVm.Conteudo,
                    Tipo = templateVm.TipoTemplate,
                    MargemAcima = templateVm.MargemAcima,
                    MargemAbaixo = templateVm.MargemAbaixo,
                    MargemDireita = templateVm.MargemDireita,
                    MargemEsquerda = templateVm.MargemEsquerda
                };

                _templateAppService.Add(template);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("TemplateError", ex.Message);
            }
            return View(templateVm);
        }

        public ActionResult Edit(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Template_Details&valor=0");

            Template template = _templateAppService.GetById(id, UserAppContext.Current.Usuario.Id);

            if (template == null || template.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var templateVm = new TemplateViewModel()
            {
                Id = template.Id,
                Nome = template.Nome,
                TipoTemplate = template.Tipo,
                Conteudo = template.Conteudo,
                TagsDisponiveis = template.TagsDisponiveis,
                DataCriacao = template.DataCriacao,
                DataAlteracao = template.DataAlteracao,
                MargemAcima = template.MargemAcima,
                MargemAbaixo = template.MargemAbaixo,
                MargemDireita = template.MargemDireita,
                MargemEsquerda = template.MargemEsquerda
            };

            return View(templateVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult Edit(TemplateViewModel templateVm)
        {
            try
            {
                var template = new Template()
                {
                    Id = templateVm.Id,
                    Nome = templateVm.Nome,
                    Conteudo = templateVm.Conteudo,
                    Tipo = templateVm.TipoTemplate,
                    MargemAcima = templateVm.MargemAcima,
                    MargemAbaixo = templateVm.MargemAbaixo,
                    MargemDireita = templateVm.MargemDireita,
                    MargemEsquerda = templateVm.MargemEsquerda
                };

                _templateAppService.Add(template);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("TemplateError", ex.Message);
            }
            return View(templateVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult Delete(long id)
        {
            try
            {
                _templateAppService.Delete(id);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint") ||
                    ex.Message.Contains("A instrução DELETE conflitou com a restrição do REFERENCE "))
                    message = "Não foi possível excluir este Template. Ele está sendo utilizado em outros registros.";
                return Json(new { status = "Erro", mensagem = message, url = "" });
            }
            this.ShowMessage("Sucesso", "Template excluído com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Template excluído com sucesso!", url = Url.Action("Index", "Template") });
        }

    }
}