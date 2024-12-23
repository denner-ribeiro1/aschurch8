using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities.Relatorios.API.In;
using ASChurchManager.Domain.Entities.Relatorios.API.Out;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.WebApi.Oauth.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;

namespace ASChurchManager.Application.AppServices
{
    public class PrinterAPIAppService : IPrinterAPIAppService
    {
        private readonly IUsuarioRepository _usuarioService;
        private readonly IConfiguration _configuration;

        private string UrlAPI => _configuration["UrlAPI"];
        public PrinterAPIAppService(IUsuarioRepository usuarioService, 
            IConfiguration configuration)
        {
            _configuration = configuration;
            _usuarioService = usuarioService;
        }

        private string RequisicaoWebApi(string metodoAPI, TipoRequisicaoWebApi tipoRequisicaoWebApi, string parametros, int usuarioId)
        {
            var usuario = _usuarioService.GetById(usuarioId, 0);
            var url = UrlAPI;

            ManagerWebApi t = new ManagerWebApi(
                new AutenticacaoApiFITokenGeraToken()
                {
                    ClienteId = "1",
                    Usuario = usuario.Username
                }, url
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

        
        public bool Printer(long templateId, string targetModel, long modelId, int usuarioId, out byte[] relatorio, out string mimeType)
        {
            var param = new InPrint()
            {
                TemplateId = templateId,
                TargetModel = targetModel,
                ModelId = modelId
            };

            var retorno =
                JsonConvert.DeserializeObject<OutRelatorio>(RequisicaoWebApi("Printer/Print", TipoRequisicaoWebApi.Post, JsonConvert.SerializeObject(param), usuarioId));

            if (retorno.Erros.Count > 0)
            {
                string erro = "";
                retorno.Erros.ForEach(e => erro += e + Environment.NewLine);
                throw new Exception($"Erro ao gerar a Carta de Transferência. {erro}");
            }
            else
            {
                relatorio = retorno.Relatorio;
                mimeType = retorno.MimeType;
                return true;
            }
        }
    }
}
