using Microsoft.AspNetCore.Mvc;

namespace ASChurchManager.Web.Lib
{
    public enum AlertType
    {
        Information = 1,
        Success = 2,
        Warning = 3,
        Error = 4
    }

    public class AlertMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public AlertType Type { get; set; }
    }

    internal static class Messenger
    {

        public static void ShowMessage(this ControllerBase controller, string title, string message, AlertType type = AlertType.Information)
        {
            var alertMessage = new AlertMessage
            {
                Title = title,
                Message = message,
                Type = type
            };

            const string key = "AlertMessage";
            if (((Microsoft.AspNetCore.Mvc.Controller)controller).TempData.ContainsKey(key))
            {
                ((Microsoft.AspNetCore.Mvc.Controller)controller).TempData.Remove(key);
            }

            var msg = Newtonsoft.Json.JsonConvert.SerializeObject(alertMessage);
            ((Microsoft.AspNetCore.Mvc.Controller)controller).TempData.Add(key, msg);
        }
    }
}