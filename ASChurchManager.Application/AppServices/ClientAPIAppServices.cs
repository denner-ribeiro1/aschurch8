using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.WebApi.Oauth.Client;
using Microsoft.Extensions.Configuration;

namespace ASChurchManager.Application.AppServices
{
    public class ClientAPIAppServices : IClientAPIAppServices
    {
        private readonly IUsuarioRepository _usuarioService;
        private readonly IConfiguration _configuration;

        private string UrlAPI => _configuration["UrlAPI"];
        public ClientAPIAppServices(IUsuarioRepository usuarioService,
            IConfiguration configuration)
        {
            _usuarioService = usuarioService;
            _configuration = configuration;
        }

        public string RequisicaoWebApi(string metodoAPI, TipoRequisicaoWebApi tipoRequisicaoWebApi, string parametros, int usuarioId)
        {
            ManagerWebApi t = new ManagerWebApi(
                new AutenticacaoApiFITokenGeraToken()
                {
                    ClienteId = "1",
                    Usuario = _usuarioService.GetById(usuarioId, 0).Username
                }, UrlAPI
                );

            switch (tipoRequisicaoWebApi)
            {
                case TipoRequisicaoWebApi.Get:
                    return t.Get<string>(metodoAPI);
                case TipoRequisicaoWebApi.Post:
                    return t.Post<string, string>(metodoAPI, parametros);
                default:
                    return string.Empty;
            }
        }
    }
}
