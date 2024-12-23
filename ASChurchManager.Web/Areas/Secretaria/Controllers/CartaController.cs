using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Carta;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Filters.Menu;
using ASChurchManager.Web.Lib;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Wkhtmltopdf.NetCore;

namespace ASChurchManager.Web.Areas.Secretaria.Controllers
{
    [Rotina(Area.Secretaria), ControllerAuthorize("Carta")]
    public class CartaController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IMembroAppService _membroAppService;
        private readonly ICartaAppService _cartaAppService;
        private readonly ITemplateAppService _templateAppService;
        private readonly ICongregacaoAppService _congregacaoAppService;
        private readonly IGeneratePdf _generatePdf;
        private readonly IPrinterAppService _printerAppService;

        public CartaController(ICartaAppService cartaAppService
                            , IMembroAppService membroAppService
                            , ITemplateAppService templateAppService
                            , ICongregacaoAppService congregacaoAppService
                            , IMapper mapper
                            , IGeneratePdf generatePdf
                            , IPrinterAppService printerAppService
                            , IMemoryCache cache
                            , IUsuarioLogado usuLog
                            , IConfiguration _configuration
                            , IRotinaAppService _rotinaAppService)
            : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _cartaAppService = cartaAppService;
            _membroAppService = membroAppService;
            _templateAppService = templateAppService;
            _congregacaoAppService = congregacaoAppService;
            _mapper = mapper;
            _generatePdf = generatePdf;
            _printerAppService = printerAppService;
        }

        // GET: Secretaria/carta
        [Action(Menu.Carta, Menu.EmissaoReceb)]
        public ActionResult Index()
        {
            var cartaVM = new IndexCartaVM();
            _congregacaoAppService.GetAll(UserAppContext.Current.Usuario.Id).OrderBy(p => p.Nome).ToList()
               .ForEach(cong =>
                   cartaVM.ListaCongregacoes.Add(new SelectListItem()
                   {
                       Text = cong.Nome,
                       Value = cong.Id.ToString()
                   }));
            return View(cartaVM);
        }

        public JsonResult GetList([FromServices] ILogger<CartaController> logger,
            string filtro = "", string conteudo = "", string status = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                StatusCarta? _status = null;

                if (!string.IsNullOrEmpty(status))
                    _status = status.ToEnum<StatusCarta>();
                if (filtro == "TipoCarta_Status")
                    filtro = "TipoCarta";

                var cartas = _cartaAppService.ListarCartaPaginado(jtPageSize, jtStartIndex, jtSorting, filtro, conteudo, _status, UserAppContext.Current.Usuario.Id, out int qtdRows).ToList();

                var cartaVM = new List<GridCartaItem>();
                cartas.ForEach(p => cartaVM.Add(new GridCartaItem()
                {
                    Id = p.Id,
                    CongregacaoDestino = p.CongregacaoDest,
                    CongregacaoOrigem = p.CongregacaoOrigem,
                    Nome = p.Nome,
                    StatusCarta = p.StatusCarta.GetDisplayAttributeValue(),
                    TipoCarta = p.TipoCarta.GetDisplayAttributeValue(),
                    DataValidade = p.DataValidade,
                    TemplateId = p.TemplateId,
                    AprovarCarta = p.StatusCarta == StatusCarta.AguardandoRecebimento &&
                        (UserAppContext.Current.Usuario.Congregacao.Sede || UserAppContext.Current.Usuario.Congregacao.Id == p.CongregacaoDestId)
                }));
                return Json(new { Result = "OK", Records = cartaVM, TotalRecordCount = qtdRows });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        public ActionResult Create()
        {
            // Inicializa o ViewModel para não dar erro na view por causa do titulo da VM
            var cartaVm = new CartaViewModel()
            {
                IsReadOnly = false,
                acao = Acao.Create
            };
            cartaVm.SelectTemplates = new List<SelectListItem>();
            _templateAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList()
             .ForEach(t =>
                 cartaVm.SelectTemplates.Add(new SelectListItem()
                 {
                     Text = t.Nome,
                     Value = t.Id.ToString()
                 }));

            return View(cartaVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult SalvarCarta([FromServices] ILogger<CartaController> logger, CartaViewModel cartaVm)
        {
            try
            {
                var carta = _mapper.Map<Carta>(cartaVm);
                if (cartaVm.acao == Acao.Create)
                    carta.Id = _cartaAppService.Add(carta, Convert.ToInt16(UserAppContext.Current.Usuario.Id));
                else if (cartaVm.acao == Acao.Update)
                    _cartaAppService.Update(carta, Convert.ToInt16(UserAppContext.Current.Usuario.Id));

                if (carta.StatusRetorno == TipoStatusRetorno.OK)
                {
                    return Json(new
                    {
                        status = "OK",
                        url = Url.Action("Index", "Carta", new { Area = "Secretaria" }),
                        mensagem = $"Carta de Transferência salva com sucesso! Código: {carta.Id}<br>Deseja imprimi-la?",
                        id = carta.Id,
                        templateid = carta.TemplateId
                    });
                }
                else
                    throw new Erro(string.Join(Environment.NewLine, carta.ErrosRetorno.Select(e => e.Mensagem)));
            }
            catch (Erro ex)
            {
                return Json(new
                {
                    status = "ERRO",
                    mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new
                {
                    status = "ERRO",
                    mensagem = ex.Message
                });
            }
        }

        public ActionResult Edit(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Carta_Edit&valor=0");

            var carta = _cartaAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            carta.Observacao = !string.IsNullOrWhiteSpace(carta.Observacao) ? carta.Observacao.Replace("<br>", "\r\n") : string.Empty;
            if (carta == null || carta.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var cartaVm = _mapper.Map<CartaViewModel>(carta);
            cartaVm.IsReadOnly = false;
            cartaVm.acao = Acao.Update;
            _templateAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList()
             .ForEach(t =>
                 cartaVm.SelectTemplates.Add(new SelectListItem()
                 {
                     Text = t.Nome,
                     Value = t.Id.ToString()
                 }));
            return View(cartaVm);
        }


        public ActionResult Details(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Carta_Details&valor=0");

            var carta = _cartaAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            carta.Observacao = !string.IsNullOrWhiteSpace(carta.Observacao) ? carta.Observacao.Replace("<br>", "\r\n") : string.Empty;
            if (carta == null || carta.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var cartaVm = _mapper.Map<CartaViewModel>(carta);
            cartaVm.IsReadOnly = true;
            cartaVm.acao = Acao.Read;
            _templateAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList()
             .ForEach(t =>
                 cartaVm.SelectTemplates.Add(new SelectListItem()
                 {
                     Text = t.Nome,
                     Value = t.Id.ToString()
                 }));
            return View(cartaVm);
        }

        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Delete([FromServices] ILogger<CartaController> logger, long id)
        {
            try
            {
                if (_cartaAppService.CancelarCarta(id, UserAppContext.Current.Usuario.Id, out string erro))
                {
                    this.ShowMessage("Carta de Transferência ", "Carta de Transferência cancelada com sucesso!", AlertType.Success);
                    return Json(new
                    {
                        status = "OK",
                        url = Url.Action("Index", "Carta", new { Area = "Secretaria" })
                    });
                }
                else
                {
                    return Json(new
                    {
                        status = "ERRO",
                        mensagem = erro
                    });
                }
                

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new
                {
                    status = "ERRO",
                    mensagem = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult ValidarMembroCarta(long MembroId = 0)
        {
            var situacoes = _membroAppService.ListarSituacoesMembro(MembroId);
            var ultSit = situacoes.FirstOrDefault(p => p.Id == situacoes.Max(m => m.Id));

            if (ultSit != null && ultSit.Situacao != MembroSituacao.Comunhao)
                return Json(new
                {
                    Erro = "Membro não está em Comunhão."
                });

            var membro = _membroAppService.GetById(MembroId, UserAppContext.Current.Usuario.Id);
            if (membro.TipoMembro != TipoMembro.Membro)
                return Json(new
                {
                    Erro = "Código de Registro não pertence a um Membro."
                });

            var cartas = _cartaAppService.VerificaCartaAguardandoRecebimento(MembroId);

            if (cartas.Count() > 0)
                return Json(new
                {
                    Erro = "Existe uma Carta em aberto para o Membro"
                });

            return Json(new
            {
                Erro = ""
            });
        }

        public ActionResult AprovarCarta(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Carta_AprovaCarta&valor=0");

            var carta = _cartaAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            carta.Observacao = !string.IsNullOrWhiteSpace(carta.Observacao) ? carta.Observacao.Replace("<br>", "\r\n") : string.Empty;
            if (carta == null || carta.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var cartaVm = _mapper.Map<CartaViewModel>(carta);
            cartaVm.IsReadOnly = true;
            cartaVm.acao = Acao.Read;
            _templateAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList()
             .ForEach(t =>
                 cartaVm.SelectTemplates.Add(new SelectListItem()
                 {
                     Text = t.Nome,
                     Value = t.Id.ToString()
                 }));
            return View(cartaVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult AprovarCarta([FromServices] ILogger<CartaController> logger, 
            long id, string codigoRecebimento)
        {
            try
            {
                //Validações da Carta
                if (id == 0)
                    throw new Erro("Id da Carta igual a 0");
                if (string.IsNullOrWhiteSpace(codigoRecebimento))
                    throw new Erro("Código de Recebimento não preenchido.");

                //Aprovação da carta
                List<ErroRetorno> erros = new List<ErroRetorno>();
                if (_cartaAppService.AprovarCarta(id, codigoRecebimento, UserAppContext.Current.Usuario.Id, out erros) == TipoStatusRetorno.OK)
                {
                    this.ShowMessage("Sucesso", "Carta de Transferência aprovada com sucesso!", AlertType.Success);
                    return Json(new
                    {
                        status = "OK",
                        url = Url.Action("Index", "Carta", new { Area = "Secretaria" })
                    });
                }
                else
                    throw new Erro(string.Join(Environment.NewLine, erros.Select(e => e.Mensagem)));
            }
            catch (Erro ex)
            {
                return Json(new
                {
                    status = "ERRO",
                    mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new
                {
                    status = "ERRO",
                    mensagem = ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult ValidarCodigoRecebCarta(long id = 0, string codigoEmissao = "")
        {
            var carta = _cartaAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (codigoEmissao != carta.CodigoRecebimento)
                return Json(new
                {
                    Erro = "Código de Recebimento não encontrado ou inválido!"
                });

            return Json(new
            {
                Erro = ""
            });
        }

        [Action(Menu.Carta, Menu.TransferenciaSemCarta)]
        public ActionResult TransferirSemCarta()
        {
            return View(new TransferirViewModel());
        }

        public JsonResult BuscarMembro([FromServices] ILogger<CartaController> logger, int MembroId = 0)
        {
            try
            {
                var situacoes = _membroAppService.ListarSituacoesMembro(MembroId);
                var ultSit = situacoes.FirstOrDefault(p => p.Id == situacoes.Max(m => m.Id));

                if (ultSit != null && ultSit.Situacao != MembroSituacao.Comunhao)
                    throw new Erro("Membro não está em Comunhão.");

                var membro = _membroAppService.GetById(MembroId, UserAppContext.Current.Usuario.Id);
                if (membro.TipoMembro != TipoMembro.Membro)
                    throw new Erro("Código de Registro não pertence a um Membro.");

                var cartas = _cartaAppService.VerificaCartaAguardandoRecebimento(MembroId);
                if (cartas.Count() > 0)
                    throw new Erro("Existe uma Carta em aberto para o Membro");

                return Json(new { status = "OK", membro });
            }
            catch (Erro ex)
            {
                return Json(new { status = "ERRO", msg = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "ERRO", msg = ex.Message });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult TransferirSemCarta([FromServices] ILogger<CartaController> logger, TransferirViewModel transf)
        {
            try
            {
                var membros = new List<Membro>();
                transf.Membros.ToList().ForEach(i => membros.Add(new Membro { Id = i.MembroId.Value }));
                _cartaAppService.TransferirSemCarta(membros, transf.CongregacaoDestId.GetValueOrDefault(0), UserAppContext.Current.Usuario.Id);

                this.ShowMessage("Sucesso", "Membros transferidos com sucesso!", AlertType.Success);
                return Json(new { status = "OK", msg = "Membros transferidos com sucesso!", url = Url.Action("Index", "Home", new { Area = "" }) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                var msgAlert = $"Erro ao Transferir os Membros sem cartas: {ex.Message}";
                return Json(new { status = "Erro", membroid = transf?.MembroId, msg = msgAlert, url = "" });
            }
        }

        private byte[] PdfConvert(string html, int MargemAcima, int MargemAbaixo,
            int MargemEsquerda, int MargemDireita)
        {
            _generatePdf.SetConvertOptions(new ConvertOptions()
            {
                PageSize = Wkhtmltopdf.NetCore.Options.Size.A4,
                PageMargins = new Wkhtmltopdf.NetCore.Options.Margins()
                {
                    Bottom = MargemAbaixo * 10,
                    Top = MargemAcima * 10,
                    Left = MargemEsquerda * 10,
                    Right = MargemDireita * 10
                }
            });
            var pdf = _generatePdf.GetPDF(html);

            var pdfStream = new System.IO.MemoryStream();
            pdfStream.Write(pdf, 0, pdf.Length);
            pdfStream.Position = 0;
            return pdfStream.ToArray();
        }

        public FileStreamResult Imprimir([FromServices] ILogger<CartaController> logger, 
            long templateId, string targetModel, long modelId)
        {
            try
            {
                if (templateId == 0 || string.IsNullOrWhiteSpace(targetModel))
                    throw new Erro("Não encontrado o template");


                var templ = _templateAppService.GetById(templateId, UserAppContext.Current.Usuario.Id);
                var conteudo = _printerAppService.GetHtmlToPrint(templateId, targetModel, modelId, UserAppContext.Current.Usuario.Id);
                var pdf = PdfConvert(conteudo, templ.MargemAcima, templ.MargemAbaixo, templ.MargemEsquerda, templ.MargemDireita);

                var filename = $"{targetModel}_{modelId}.pdf";
                var stream = new MemoryStream(pdf);
                var fileStreamResult = new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = filename
                };

                Response.StatusCode = StatusCodes.Status200OK;
                return fileStreamResult;
            }
            catch (Erro ex)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                string messages = JsonSerializer.Serialize(new
                {
                    data = string.Join(Environment.NewLine, ex.FromHierarchy<Exception>(ex1 => ex1.InnerException).Select(ex1 => ex1.Message))
                });

                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(messages);
                writer.Flush();
                stream.Position = 0;
                return new FileStreamResult(stream, "application/json");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
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