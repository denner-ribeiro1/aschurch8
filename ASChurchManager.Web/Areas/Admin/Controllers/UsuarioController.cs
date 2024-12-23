using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Web.Areas.Admin.ViewModels.Usuario;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Filters.Menu;
using ASChurchManager.Web.Lib;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASChurchManager.Web.Areas.Admin.Controllers
{
    [Rotina(Area.Admin), ControllerAuthorize("Usuario")]
    public class UsuarioController : BaseController
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IPerfilAppService _perfilAppService;
        private readonly IMapper _mapper;
        private readonly ICongregacaoAppService _congrAppService;

        public UsuarioController(IUsuarioAppService appService
                                , IPerfilAppService perfilAppService
                                , IRotinaAppService rotinaAppService
                                , IMapper mapper
                                , ICongregacaoAppService congrAppService
                                , IMemoryCache cache
                                , IUsuarioLogado usuLog
                                , IConfiguration _configuration
                                , IRotinaAppService _rotinaAppService)
                : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _usuarioAppService = appService;
            _perfilAppService = perfilAppService;
            _mapper = mapper;
            _congrAppService = congrAppService;
        }

        [Action(Menu.Acesso, Menu.Usuario)]
        public ActionResult Index()
        {
            var usuarioVM = new UsuarioViewModel();
            var _congregacoes = new List<SelectListItem>();
            foreach (var cong in _congrAppService.GetAll(UserAppContext.Current.Usuario.Id).OrderBy(p => p.Nome))
            {
                _congregacoes.Add(new SelectListItem()
                {
                    Text = cong.Nome,
                    Value = cong.Id.ToString()
                });
            }
            usuarioVM.ListaCongregacoes = _congregacoes;
            return View(usuarioVM);
        }

        public JsonResult GetList(string filtro = "", string conteudo = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                jtSorting = jtSorting == "Congregacao" ? jtSorting : jtSorting.Replace("Congregacao", "CongregacaoId");
                var usuario = _usuarioAppService.ListarUsuariosPaginada(jtPageSize, jtStartIndex, out int qtdRows, jtSorting, filtro, conteudo, UserAppContext.Current.Usuario.Id).ToList();
                var usuarioVM = _mapper.Map<List<GridUsuarioViewModel>>(usuario);

                return Json(new { Result = "OK", Records = usuarioVM, TotalRecordCount = qtdRows });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        public ActionResult Details(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Usuario_Details&valor=0");

            var usuario = _usuarioAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (usuario == null || usuario.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var usuarioVM = _mapper.Map<UsuarioViewModel>(usuario);
            usuarioVM.PerfilId = usuario.Perfil.Id;
            usuarioVM.SelectPerfil = _perfilAppService.GetAll(UserAppContext.Current.Usuario.Id)
                .Select(c => new SelectListItem() { Text = c.Nome, Value = c.Id.ToString() }).ToList();

            usuarioVM.IsReadOnly = true;

            return View(usuarioVM);
        }

        public ActionResult Create()
        {
            var usuarioVM = new UsuarioViewModel()
            {
                SelectPerfil = _perfilAppService.GetAll(UserAppContext.Current.Usuario.Id)
                    .Select(c => new SelectListItem() { Text = c.Nome, Value = c.Id.ToString() }).ToList()

            };
            return View(usuarioVM);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult Create(UsuarioViewModel usuarioVM)
        {
            if (_usuarioAppService.VerificaUsuarioDuplicado(usuarioVM.Username))
                ModelState.AddModelError("Username", "Usuário já existe");

            if (ModelState.IsValid)
            {
                var usuario = _mapper.Map<Usuario>(usuarioVM);
                usuario.Congregacao.Id = usuarioVM.CongregacaoId;
                usuario.Perfil.Id = usuarioVM.PerfilId;

                _usuarioAppService.Add(usuario);
                this.ShowMessage("Sucesso", "Usuário incluído com sucesso!", AlertType.Success);
                return RedirectToAction("Index");
            }

            return View(usuarioVM);
        }

        public ActionResult Edit(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Usuario_Edit&valor=0");

            var usuario = _usuarioAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (usuario == null || usuario.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var usuarioVM = _mapper.Map<UsuarioViewModel>(usuario);
            usuarioVM.PerfilId = usuario.Perfil.Id;
            usuarioVM.SelectPerfil = _perfilAppService.GetAll(UserAppContext.Current.Usuario.Id)
                .Select(c => new SelectListItem() { Text = c.Nome, Value = c.Id.ToString() }).ToList();

            usuarioVM.IsReadOnly = true;

            return View(usuarioVM);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult Edit(UsuarioViewModel usuarioVM)
        {
            ModelState.Remove("Senha");
            ModelState.Remove("RedigiteSenha");

            if (ModelState.IsValid)
            {
                var usuario = _mapper.Map<Usuario>(usuarioVM);
                usuario.Congregacao.Id = usuarioVM.CongregacaoId;
                usuario.Perfil.Id = usuarioVM.PerfilId;

                _usuarioAppService.Update(usuario);
                this.ShowMessage("Sucesso", "Usuário atualizado com sucesso!", AlertType.Success);
                return RedirectToAction("Index");
            }
            return View(usuarioVM);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult Delete(long id)
        {
            try
            {
                _usuarioAppService.Delete(id);
            }
            catch (Exception ex)
            {
                return Json(new { status = "Erro", mensagem = ex.Message, url = "" });
            }
            this.ShowMessage("Sucesso", "Usuário excluído com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Usuário excluído com sucesso!", url = Url.Action("Index", "Usuario") });
        }
        [AllowAnonymous]
        public ActionResult NovaSenha()
        {
            var usuario = _usuarioAppService.GetById(UserAppContext.Current.Usuario.Id, UserAppContext.Current.Usuario.Id);
            if (usuario == null || usuario.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var alterarSenhaVm = _mapper.Map<AlteracaoSenhaUsuarioViewModel>(usuario);
            alterarSenhaVm.AlterarSenhaProxLogin = false;
            ViewBag.ExibirVoltar = true;

            return View("AlterarSenha", alterarSenhaVm);
        }


        [AllowAnonymous]
        public ActionResult AlterarSenha(long id, bool exibirVoltar = false)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Usuario_AlterarSenha&valor=0");

            var usuario = _usuarioAppService.GetById(id, id);
            if (usuario == null || usuario.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var alterarSenhaVm = _mapper.Map<AlteracaoSenhaUsuarioViewModel>(usuario);
            alterarSenhaVm.AlterarSenhaProxLogin = true;
            ViewBag.ExibirVoltar = exibirVoltar;

            return View(alterarSenhaVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        [AllowAnonymous]
        public ActionResult AlterarSenha(AlteracaoSenhaUsuarioViewModel alteracaoSenhaVm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = _mapper.Map<Usuario>(alteracaoSenhaVm);
                    usuario.AlterarSenhaProxLogin = alteracaoSenhaVm.AlterarSenhaProxLogin;

                    if (_usuarioAppService.AlterarSenha(usuario, UserAppContext.Current.Usuario.Id) > 0)
                    {
                        this.ShowMessage("Sucesso", "Alteração de senha realizada com sucesso!", AlertType.Success);

                        if (alteracaoSenhaVm.AlterarSenhaProxLogin)
                            return RedirectToAction("Index");
                        return RedirectToAction("Index", "Home", new { Area = "" });
                    }
                }
                catch (Exception ex)
                {
                    this.ShowMessage("Erro", ex.Message, AlertType.Error);
                    ModelState.AddModelError("Erro", ex.Message);
                }
            }
            return View(alteracaoSenhaVm);
        }

        public ActionResult VerificaUsuarioDuplicado(string username, long id = 0)
        {
            bool usuarioDuplicado = false;
            if (id == 0)
                usuarioDuplicado = string.IsNullOrWhiteSpace(username) || _usuarioAppService.VerificaUsuarioDuplicado(username);
            return Json(usuarioDuplicado);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult AlterarSkinUsuario(string novoSkin)
        {
            _usuarioAppService.AlterarSkinUsuario(novoSkin, UserAppContext.Current.Usuario.Id);
            var usuarioAtual = _usuarioAppService.GetById(UserAppContext.Current.Usuario.Id, 0);

            Login.Logar(HttpContext, usuarioAtual, configuration);
            return Json(new { status = "OK" });
        }

    }
}
