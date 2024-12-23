using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Web.Areas.Admin.ViewModels.Usuario;
using ASChurchManager.Web.Lib;
using ASChurchManager.Web.ViewModels.Usuario;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace ASChurchManager.Web.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IUsuarioAppService _usuarioAppService;
        private readonly IRotinaAppService _rotinaAppService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _Logger;

        public AuthController(IUsuarioAppService usuarioAppService,
            IRotinaAppService rotinaAppService,
            IMapper mapper,
            IConfiguration configuration,
            IMemoryCache cache,
            IUsuarioLogado usuLog,
            ILogger<AuthController> logger)
            : base(cache, usuLog, configuration, rotinaAppService)
        {
            _usuarioAppService = usuarioAppService;
            _rotinaAppService = rotinaAppService;
            _mapper = mapper;
            _configuration = configuration;
            _Logger = logger;
        }

        [HttpGet]
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public IActionResult Login(LoginViewModel loginVm, [FromServices] IMemoryCache _cache)
        {
            try
            {
                var secretKey = _configuration["ParametrosSistema:CaptchaChave"];
                var token = HttpContext.Request.Form["g-recaptcha-response"].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    throw new Erro("Não foi possível obter o Captcha!");
                }

                var captchaIsValid = ValidarCaptcha(token, secretKey).Result;
                if (!captchaIsValid)
                {
                    throw new Erro("Captcha inválido!");
                }
                if (ModelState.IsValid)
                {
                    var usuario = _mapper.Map<Usuario>(loginVm);

                    if (_usuarioAppService.ValidarLogin(ref usuario))
                    {
                        if (usuario.AlterarSenhaProxLogin)
                        {
                            var userName = System.Text.Encoding.UTF8.GetBytes(usuario.Username);
                            return RedirectToAction("AlterarSenhaAposLogin", "Auth", new { guid = Convert.ToBase64String(userName) });
                        }
                        else
                        {
                            Login(usuario);
                            return RedirectToAction("Index", "Home", null);
                        }
                    }
                }
            }
            catch (Erro ex)
            {
                this.ShowMessage("Erro", ex.Message, AlertType.Error);
            }
            catch (Exception ex)
            {
                this.ShowMessage("Erro", ex.Message, AlertType.Error);
            }
            return View();
        }
        private async Task<bool> ValidarCaptcha(string token, string secret)
        {
            using var client = new System.Net.Http.HttpClient();
            var result = await client.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={token}").Result.Content.ReadAsStringAsync();
            return (bool)JObject.Parse(result.ToString())["success"];
        }

        private void Login(Usuario usuario)
        {
            Lib.Login.Logar(HttpContext, usuario, _configuration);
        }

        public ActionResult AlterarSenha(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Auth_AlterarSenha&valor=0");

            var usuario = _usuarioAppService.GetById(id, id);
            if (usuario == null || usuario.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var alterarSenhaVm = _mapper.Map<AlteracaoSenhaUsuarioViewModel>(usuario);

            return View(alterarSenhaVm);
        }

        [HttpPost]
        public ActionResult AlterarSenha(AlteracaoSenhaUsuarioViewModel alteracaoSenhaVm, [FromServices] IMemoryCache cache)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = _mapper.Map<Usuario>(alteracaoSenhaVm);

                    if (_usuarioAppService.AlterarSenha(usuario, usuario.Id) > 0)
                    {
                        usuario = _usuarioAppService.GetById(usuario.Id, usuario.Id);
                        Login(usuario);
                        this.ShowMessage("Sucesso", "Alteração de senha realizada com sucesso!", AlertType.Success);
                        return RedirectToAction("Index", "Home", null);
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

        public ActionResult AlterarSenhaAposLogin(string guid)
        {
            var username = Convert.FromBase64String(guid);
            var usuario = _usuarioAppService.GetUsuarioByUsername(System.Text.Encoding.UTF8.GetString(username));
            if (usuario == null || usuario.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            return AlterarSenha(usuario.Id);
        }

        [HttpPost]
        public ActionResult AlterarSenhaAposLogin(AlteracaoSenhaUsuarioViewModel alteracaoSenhaVm, [FromServices] IMemoryCache cache)
        {
            return AlterarSenha(alteracaoSenhaVm, cache);
        }

        public ActionResult NaoAutorizado()
        {
            return View();
        }

        public ActionResult BadRequest(string url, string valor)
        {
            string msg = $"Erro ao requisitar uma Página - Usuário: {UserAppContext.Current.Usuario.Username} - Url com Erro: {url} - Valor Pesquisado: {valor}";
            _Logger.LogError(msg, null);
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LogOutAsync([FromServices] IUsuarioLogado usuario, [FromServices] IMemoryCache cache)
        {
            await Lib.Login.LogOut(HttpContext, usuario, cache);
            return RedirectToAction("Login", "Auth", null);
        }
    }
}