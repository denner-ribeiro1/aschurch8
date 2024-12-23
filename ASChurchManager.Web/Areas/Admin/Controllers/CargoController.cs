using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Web.Areas.Admin.ViewModels.Cargo;
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
    [ControllerAuthorize("Cargo")]
    public class CargoController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ICargoAppService _appService;

        public CargoController(ICargoAppService appService,
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

        // GET: Secretaria/Cargo
        [Action(Menu.Cadastros, Menu.Cargo)]
        public ActionResult Index()
        {

            return View();
        }

        public JsonResult GetList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var cargo = _appService.GetAll(UserAppContext.Current.Usuario.Id);
                var lcargos = cargo.Skip(jtStartIndex).Take(jtPageSize);
                var cargoVm = _mapper.Map<List<GridCargosViewModel>>(lcargos);

                return Json(new { Result = "OK", Records = cargoVm, TotalRecordCount = cargo.Count() });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        // GET: Secretaria/Cargo/Details/5
        public ActionResult Details(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Cargo_Details&valor=0");
            
            var cargo = _appService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (cargo == null || cargo.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var cargoVm = _mapper.Map<CargoViewModel>(cargo);
            return View(cargoVm);
        }

        // GET: Secretaria/Cargo/Create
        public ActionResult Create()
        {
            var cargoVm = new CargoViewModel();
            return View(cargoVm);
        }

        // POST: Secretaria/Cargo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        [HttpPost]
        public ActionResult Create(CargoViewModel cargoVm)
        {
            if (ModelState.IsValid)
            {
                var cargo = _mapper.Map<Cargo>(cargoVm);
                _appService.Add(cargo);

                this.ShowMessage("Sucesso", "Cargo incluído com sucesso!", AlertType.Success);
                return RedirectToAction("Index");
            }

            return View(cargoVm);
        }

        // GET: Secretaria/Cargo/Edit/5
        public ActionResult Edit(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Cargo_Details&valor=0");

            var cargo = _appService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (cargo == null || cargo.Id == 0)
                return Redirect("/Auth/NaoAutorizado");
            
            var cargoVm = _mapper.Map<CargoViewModel>(cargo);
            return View(cargoVm);
        }

        // POST: Secretaria/Cargo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        [HttpPost]
        public ActionResult Edit(CargoViewModel cargoVm)
        {
            if (ModelState.IsValid)
            {
                var cargo = _mapper.Map<Cargo>(cargoVm);
                _appService.Update(cargo);
                this.ShowMessage("Sucesso", "Cargo atualizado com sucesso!", AlertType.Success);
                return RedirectToAction("Index");
            }
            return View(cargoVm);
        }

        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        [HttpPost]
        public JsonResult Delete(string id)
        {
            try
            {
                _appService.Delete(Convert.ToInt32(id));
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint") ||
                    ex.Message.Contains("A instrução DELETE conflitou com a restrição do REFERENCE "))
                    message = "Não foi possível excluir este Cargo. Ele está sendo utilizado em outros registros.";
                return Json(new { status = "Erro", mensagem = message, url = "" });
            }
            this.ShowMessage("Sucesso", "Cargo excluído com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Cargo excluído com sucesso!", url = Url.Action("Index", "Cargo") });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
