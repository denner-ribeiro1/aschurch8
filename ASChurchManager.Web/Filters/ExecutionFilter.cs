using ASChurchManager.Web.Controllers;
using ASChurchManager.Web.Lib;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;


namespace ASChurchManager.Web.Filters
{
    public class ExecutionFilter : ActionFilterAttribute
    {
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    base.OnActionExecuting(filterContext);

        //    ValidateSessionState(filterContext);
        //}

        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    base.OnActionExecuted(filterContext);

        //    ValidateErrors(filterContext);
        //}

        //private void ValidateSessionState(ActionExecutingContext filterContext)
        //{
        //    if (filterContext.Controller.GetType() != typeof(AuthController))
        //    {
        //        if (UserAppContext.Current == null
        //            || UserAppContext.Current.Usuario == null
        //            || string.IsNullOrWhiteSpace(UserAppContext.Current.Usuario.Username))
        //        {
        //            filterContext.HttpContext.Session.RemoveAll();
        //            filterContext.HttpContext.Session.Clear();
        //            filterContext.HttpContext.Session.Abandon();
        //            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
        //                { "area", "" }, { "controller", "Auth" }, { "action", "Login" }
        //            });
        //            return;
        //        }
        //    }
        //}

        //private void ValidateErrors(ActionExecutedContext filterContext)
        //{
        //    var modelStateErrors = ((Controller)filterContext.Controller).ModelState.Values.Where(a => a.Errors.Count > 0);

        //    if (modelStateErrors.Any())
        //    {
        //        string msgError = string.Empty;
        //        modelStateErrors.ToList().ForEach(a =>
        //            a.Errors.ToList().ForEach(b =>
        //                msgError = b.Exception == null ? b.ErrorMessage : b.Exception.Message)
        //        );
        //        filterContext.Controller.ShowMessage("Erro", msgError, AlertType.Error);
        //    }

        //    if (filterContext.Exception != null)
        //    {
        //        // Mostar mensagem de erro na tela não funciona aqui

        //        // TODO: Implementar Log de Erros
        //    }
        //}
    }
}