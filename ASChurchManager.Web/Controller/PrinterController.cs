using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Web.Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ASChurchManager.Web.Controllers
{
    public class PrinterController : BaseController
    {
        private readonly IPrinterAPIAppService _printerAppService;

        public PrinterController(IPrinterAPIAppService printerAppService
            , IMemoryCache cache
            , IUsuarioLogado usuLog, 
            IConfiguration _configuration
            , IRotinaAppService _rotinaAppService)
            : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _printerAppService = printerAppService;
        }

        public FileStreamResult Print(long templateId, string targetModel, long modelId)
        {
            try
            {
                if (templateId == 0 || string.IsNullOrWhiteSpace(targetModel))
                    throw new Exception("Não encontrado o template");

                _printerAppService.Printer(templateId, targetModel, modelId, Convert.ToInt32(UserAppContext.Current.Usuario.Id),
                       out byte[] bytes, out string mimeType);

                var filename = $"{targetModel}_{modelId}.pdf";
                var stream = new MemoryStream(bytes);
                var fileStreamResult = new FileStreamResult(stream, mimeType)
                {
                    FileDownloadName = filename
                };

                Response.StatusCode = StatusCodes.Status200OK;
                return fileStreamResult;
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                string messages = JsonSerializer.Serialize(new
                {
                    data = string.Join(Environment.NewLine, ex.FromHierarchy(ex1 => ex1.InnerException).Select(ex1 => ex1.Message))
                });

                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(messages);
                writer.Flush();
                stream.Position = 0;
                return new FileStreamResult(stream, "application/json");
            }
        }
    }
}