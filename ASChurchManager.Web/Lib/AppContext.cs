using ASChurchManager.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;

namespace ASChurchManager.Web.Lib
{
    public class UserAppContext
    {
        private static UserAppContext _instance;
        public static IHttpContextAccessor _accessor { get; set; } = new HttpContextAccessor();

        private UserAppContext() { }

        public static UserAppContext Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserAppContext();
                }
                return _instance;
            }
        }
        public Usuario Usuario
        {
            get
            {
                var usu = _accessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Usuario").Value;
                var usuario = JsonConvert.DeserializeObject<Usuario>(usu);
                return usuario;
            }

        }
    }
}