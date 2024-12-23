using ASChurchManager.WebApi.Oauth.Client;

namespace ASChurchManager.Application.Interfaces
{
    public interface IClientAPIAppServices
    {
        string RequisicaoWebApi(string metodoAPI, TipoRequisicaoWebApi tipoRequisicaoWebApi, string parametros, int usuarioId);
    }
}
