using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Admin.ViewModels.Perfil;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Filters.Menu;
using ASChurchManager.Web.Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASChurchManager.Web.Areas.Admin.Controllers
{
    [Rotina(Area.Admin)]
    [ControllerAuthorize("Perfil")]
    public class PerfilController : BaseController
    {
        private IPerfilAppService _perfilAppService;
        private readonly IRotinaAppService _rotinaAppService;

        public PerfilController(IPerfilAppService perfilAppService
                                , IRotinaAppService rotinaAppService
                                , IMemoryCache cache
                                , IUsuarioLogado usuLog
                                , IConfiguration _configuration)
            : base(cache, usuLog, _configuration, rotinaAppService)
        {
            _perfilAppService = perfilAppService;
            _rotinaAppService = rotinaAppService;
        }

        // GET: Admin/Perfil
        [ActionAttribute(Menu.Acesso, Menu.Perfil)]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var perfis = _perfilAppService.GetAll(UserAppContext.Current.Usuario.Id);
                var lperfis = perfis.Skip(jtStartIndex).Take(jtPageSize);
                var perfilVM = new List<GridPerfilViewModel>();

                foreach (var perfil in lperfis)
                {
                    perfilVM.Add(new GridPerfilViewModel()
                    {
                        Id = perfil.Id,
                        Nome = perfil.Nome,
                        TipoPerfil = perfil.TipoPerfil
                    });
                }

                return Json(new { Result = "OK", Records = perfilVM, TotalRecordCount = perfis.Count() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        // GET: Admin/Perfil/Details/5
        public ActionResult Details(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Perfil_Details&valor=0");

            Perfil perfil = _perfilAppService.GetById(id, UserAppContext.Current.Usuario.Id);

            if (perfil == null || perfil.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var perfilRotinas = _perfilAppService.GetRotinasPerfil(perfil);

            var perfilVm = new PerfilViewModel()
            {
                Id = perfil.Id,
                Nome = perfil.Nome,
                TipoPerfil = perfil.TipoPerfil,
                DataCriacao = perfil.DataCriacao,
                DataAlteracao = perfil.DataAlteracao,
                Rotinas = perfilRotinas.Where(a => a.Value == true)
            };

            return View(perfilVm);
        }

        // GET: Admin/Perfil/Create
        public ActionResult Create()
        {
            var perfilRotinas = _perfilAppService.GetRotinasPerfil(null);
            var perfilVm = new PerfilViewModel()
            {
                Rotinas = perfilRotinas
            };

            return View(perfilVm);
        }

        // POST: Admin/Perfil/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult Create(PerfilViewModel perfilViewModel, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var rotinasSelecionadas = this.GetRotinasSelecionadas(form);

                    if (perfilViewModel.TipoPerfil == TipoPerfil.Usuario
                        && (rotinasSelecionadas == null || rotinasSelecionadas.Count == 0))
                    {
                        throw new InvalidOperationException("Selecione no mínimo uma Rotina");
                    }

                    var perfil = new Perfil()
                    {
                        Id = perfilViewModel.Id,
                        Nome = perfilViewModel.Nome,
                        TipoPerfil = (TipoPerfil)perfilViewModel.TipoPerfil,
                        Rotinas = rotinasSelecionadas,
                        Status = true,
                        DataCriacao = DateTime.Now
                    };

                    _perfilAppService.Add(perfil);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("PerfilError", ex.Message);
                }
            }

            // Recarrega as rotinas
            var perfilRotinas = _perfilAppService.GetRotinasPerfil(null);
            perfilViewModel.Rotinas = perfilRotinas;

            return View(perfilViewModel);
        }

        // GET: Admin/Perfil/Edit/5
        public ActionResult Edit(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Perfil_Details&valor=0");

            Perfil perfil = _perfilAppService.GetById(id, UserAppContext.Current.Usuario.Id);

            if (perfil == null || perfil.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var perfilRotinas = _perfilAppService.GetRotinasPerfil(perfil);

            var perfilVm = new PerfilViewModel()
            {
                Id = perfil.Id,
                Nome = perfil.Nome,
                TipoPerfil = perfil.TipoPerfil,
                DataCriacao = perfil.DataCriacao,
                DataAlteracao = perfil.DataAlteracao,
                Rotinas = perfilRotinas
            };

            return View(perfilVm);
        }

        // POST: Admin/Perfil/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult Edit(PerfilViewModel perfilViewModel, IFormCollection form)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var rotinasSelecionadas = this.GetRotinasSelecionadas(form);

                    if (perfilViewModel.TipoPerfil == TipoPerfil.Usuario
                        && (rotinasSelecionadas == null || rotinasSelecionadas.Count == 0))
                    {
                        throw new InvalidOperationException("Selecione no mínimo uma Rotina");
                    }

                    var perfil = new Perfil()
                    {
                        Id = perfilViewModel.Id,
                        Nome = perfilViewModel.Nome,
                        TipoPerfil = (TipoPerfil)perfilViewModel.TipoPerfil,
                        Rotinas = rotinasSelecionadas,
                        Status = true,
                        DataCriacao = DateTime.Now
                    };

                    _perfilAppService.Add(perfil);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("PerfilError", ex.Message);
                }
            }

            // Recarrega as rotinas
            var perfilRotinas = _perfilAppService.GetRotinasPerfil(null);
            perfilViewModel.Rotinas = perfilRotinas;
            return View(perfilViewModel);
        }
        
        // POST: Admin/Perfil/Delete/5
        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult Delete(long id)
        {
            try
            {
                _perfilAppService.Delete(id);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint") ||
                    ex.Message.Contains("A instrução DELETE conflitou com a restrição do REFERENCE "))
                    message = "Não foi possível excluir este Perfil. Ele está sendo utilizado em outros registros.";
                return Json(new { status = "Erro", mensagem = message, url = "" });
            }
            this.ShowMessage("Sucesso", "Perfil excluído com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Perfil excluído com sucesso!", url = Url.Action("Index", "Perfil") });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _perfilAppService = null;
            }
            base.Dispose(disposing);
        }

        private List<Rotina> GetRotinasSelecionadas(IFormCollection form)
        {
            var lstRotinas = _rotinaAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList();
            var rotinasSelecionadas = new List<Rotina>();
            var keys = form.Keys.Where(a => a.Contains("chkRotina_")).ToList();

            foreach (var key in keys)
            {
                form.TryGetValue(key, out Microsoft.Extensions.Primitives.StringValues valor);
                if (valor[0] == "true")
                {
                    var idRotina = key.Replace("chkRotina_", "");
                    var rotina = lstRotinas.FirstOrDefault(a => a.Id.ToString() == idRotina);
                    if (rotina != null && rotina.Id > 0)
                    {
                        rotinasSelecionadas.Add(rotina);
                    }
                }
            }
            return rotinasSelecionadas;
        }
    }
}
