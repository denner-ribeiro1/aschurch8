using ASBaseLib.Core.Helpers;
using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Filters.Menu;
using ASChurchManager.Web.Lib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ASChurchManager.Web.Areas.Admin.Controllers
{
    [Rotina(Area.Admin), ControllerAuthorize("Rotina")]
    public class RotinaController : BaseController
    {
        private readonly IRotinaAppService _appService;

        public RotinaController(IRotinaAppService appService
                    , IMemoryCache cache
                    , IUsuarioLogado usuLog
                    , IConfiguration _configuration
                    , IRotinaAppService _rotinaAppService)
            : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _appService = appService;
        }

        // GET: Admin/Acesso
        [ActionAttribute(Menu.Acesso, Menu.Rotina)]
        public ActionResult Index()
        {
            var assemblies = Assembly.Load("ASChurchManager.Web")
                .GetTypes()
                .Where(t => t.GetCustomAttribute<RotinaAttribute>() != null);

            var controllerTypes = assemblies
                .Where(t => t != null
                    && t.IsPublic // public controllers only
                    && !t.IsAbstract // no abstract controllers
                    && typeof(ControllerBase).IsAssignableFrom(t)
                    && t.GetCustomAttributes(typeof(RotinaAttribute), false).Any()); // should implement IController (happens automatically when you extend Controller)

            var controllerMethods = controllerTypes.ToDictionary(
                controllerType => controllerType,
                controllerType => controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance).
                    Where(m => typeof(ActionResult).IsAssignableFrom(m.ReturnType) &&
                          m.GetCustomAttributes(typeof(ActionAttribute), false).Any()));

            var lRotinas = new List<Rotina>();
            foreach (var cntrl in controllerMethods)
            {
                var rotAtrib = (RotinaAttribute)cntrl.Key.GetCustomAttributes(typeof(RotinaAttribute), false)[0];

                /*Area*/
                string area = rotAtrib.Area.ToString();
                string descricaoArea = ReflectionHelper.GetValueFromCustomAttribute<Enum, MenuAttribute, String>(rotAtrib.Area, "Description");
                string iconeArea = ReflectionHelper.GetValueFromCustomAttribute<Enum, MenuAttribute, String>(rotAtrib.Area, "Icon");
                if (iconeArea == "NaoDefinido")
                    iconeArea = "";
                /*Controller*/
                string controller = cntrl.Key.Name.Replace("Controller", "");

                foreach (var act in cntrl.Value)
                {
                    var atrib = (ActionAttribute)act.GetCustomAttributes(typeof(ActionAttribute), false)[0];
                    string action = string.Empty;
                    string descricaoMenu = string.Empty;
                    string iconeMenu = string.Empty;
                    string descricaoSubMenu = string.Empty;
                    string iconeSubMenu = string.Empty;

                    /*Action*/
                    action = act.Name;
                    descricaoMenu = ReflectionHelper.GetValueFromCustomAttribute<Enum, MenuAttribute, String>(atrib.Menu, "Description");
                    iconeMenu = ReflectionHelper.GetValueFromCustomAttribute<Enum, MenuAttribute, String>(atrib.Menu, "Icon");

                    if (iconeMenu == "NaoDefinido")
                        iconeMenu = "";

                    if (atrib.SubMenu != Menu.NaoDefinido)
                    {
                        descricaoSubMenu = ReflectionHelper.GetValueFromCustomAttribute<Enum, MenuAttribute, String>(atrib.SubMenu, "Description");
                        iconeSubMenu = ReflectionHelper.GetValueFromCustomAttribute<Enum, MenuAttribute, String>(atrib.SubMenu, "Icon");

                        if (iconeSubMenu == "NaoDefinido")
                            iconeSubMenu = "";
                    }

                    var rot = new Rotina
                    {
                        Area = area,
                        AreaDescricao = descricaoArea,
                        AreaIcone = iconeArea,
                        Controller = controller,
                        Action = action,
                        MenuDescricao = descricaoMenu,
                        MenuIcone = iconeMenu,
                        SubMenuDescricao = descricaoSubMenu,
                        SubMenuIcone = iconeSubMenu
                    };

                    lRotinas.Add(rot);
                }
            }
            lRotinas.ToList().ForEach(r => _appService.Add(r));

            this.ShowMessage("Sucesso", "Cadastro de Rotinas disponíveis realizado com Sucesso", AlertType.Success);

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}