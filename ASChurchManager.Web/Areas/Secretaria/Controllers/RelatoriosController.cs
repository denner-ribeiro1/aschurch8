using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities.Relatorios.Secretaria;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Secretaria.Models.Relatorio;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Relatorio;
using ASChurchManager.Web.Filters.Menu;
using ASChurchManager.Web.Lib;
using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static ASChurchManager.Domain.Entities.Evento;

namespace ASChurchManager.Web.Areas.Secretaria.Controllers
{
    [Rotina(Area.Secretaria)]
    public class RelatoriosController : BaseController
    {
        #region Variaveis
        private readonly ICongregacaoAppService _congregacaoAppService;
        private readonly IBatismoAppService _batismoAppService;
        private readonly ICursoAppService _cursoAppService;
        private readonly IRelatoriosSecretariaAppService _relatoriosAppService;
        private readonly ICursoAppService _cursoService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RelatoriosController> _logger;
        private readonly IPresencaAppService _presencaAppService;
        private readonly ICargoAppService _cargoService;
        private ITipoEventoAppService _tipoEventoService;
        #endregion

        #region Construtor
        public RelatoriosController(ICongregacaoAppService congregacaoAppService
                , IBatismoAppService batismoAppService
                , ICursoAppService cursoAppService
                , IRelatoriosSecretariaAppService relatoriosAppService
                , IConfiguration configuration
                , ICursoAppService cursoService
                , IMemoryCache cache
                , IUsuarioLogado usuLog
                , ILogger<RelatoriosController> logger
                , IRotinaAppService _rotinaAppService
                , IPresencaAppService presencaAppService
                , ICargoAppService cargoService
                , ITipoEventoAppService tipoEventoService)
            : base(cache, usuLog, configuration, _rotinaAppService)
        {
            _congregacaoAppService = congregacaoAppService;
            _batismoAppService = batismoAppService;
            _cursoAppService = cursoAppService;
            _relatoriosAppService = relatoriosAppService;
            _configuration = configuration;
            _cursoService = cursoService;
            _logger = logger;
            _presencaAppService = presencaAppService;
            _cargoService = cargoService;
            _tipoEventoService = tipoEventoService;
        }
        #endregion

        #region Privates
        private static int RelatorioCabecalho(PdfPage page,
                                              XGraphics gfx,
                                              int pagNumero,
                                              string nomeCliente,
                                              string titulo,
                                              string usuario,
                                              int comecaDados,
                                              string filtro,
                                              Dictionary<string, int> cabecalho)
        {
            var fontArial12 = new XFont("Arial", 12, XFontStyle.Bold);
            var fontArial8 = new XFont("Arial", 8, XFontStyle.Bold);

            gfx.DrawString($"Pag.: {pagNumero}", fontArial8, XBrushes.Black, new XRect(530, 30, 0, 0));
            gfx.DrawString(nomeCliente.ToUpper(), fontArial12, XBrushes.Black, new XRect(25, 40, 0, 0));
            comecaDados += 2;
            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 35, 575, comecaDados + 35);
            comecaDados += 2;
            gfx.DrawString(titulo.ToUpper(), fontArial12, XBrushes.Black, new XRect(25, 65, 0, 0));
            comecaDados += 2;
            gfx.DrawLine(new XPen(XColor.FromName("Black")), 445, 47, 445, 74);
            gfx.DrawString($"DATA: {DateTime.Now.ToShortDateString()} às {DateTime.Now:HH:mm}", fontArial8, XBrushes.Black, new XRect(450, comecaDados + 40, 0, 0));
            gfx.DrawString($"USUÁRIO: {usuario}", fontArial8, XBrushes.Black, new XRect(450, comecaDados + 53, 0, 0));
            comecaDados += 5;
            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 53, 575, comecaDados + 53);
            var fontFiltro = new XFont("Arial", 10, XFontStyle.Bold);
            gfx.DrawString(filtro, fontFiltro, XBrushes.Black, new XRect(25, comecaDados + 68, 0, 0));
            comecaDados += 2;
            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 72, 575, comecaDados + 72);
            comecaDados += 2;
            //Retangulo da Tela Toda
            gfx.DrawRectangle(new XPen(XColor.FromName("Black")), 20, 20, 555, page.Height.Point - 40);
            comecaDados += 2;
            foreach (var item in cabecalho)
                gfx.DrawString(item.Key, fontArial8, XBrushes.Black, new XRect(item.Value, comecaDados + 80, 0, 0));
            comecaDados += 12;

            return comecaDados;
        }

        private static int RelatorioCabecalhoExcel(ExcelWorksheet workSheet,
                                                   string nomeCliente,
                                                   string titulo,
                                                   string usuario,
                                                   string filtro,
                                                   Dictionary<string, int> cabecalho)
        {
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;

            var linha = 1;
            var colunas = cabecalho != null ? cabecalho.Count() : 0;

            workSheet.Row(linha).Height = 20;
            workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Row(linha).Style.Font.Bold = true;
            workSheet.Cells[linha, 1].Value = nomeCliente.ToUpper();
            if (cabecalho != null && colunas > 2)
                workSheet.Cells[linha, 1, linha, colunas].Merge = true;
            else
                workSheet.Cells[linha, 1, linha, 3].Merge = true;
            linha++;

            workSheet.Row(linha).Height = 20;
            workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Row(linha).Style.Font.Bold = true;
            workSheet.Cells[linha, 1].Value = titulo;
            if (cabecalho != null && colunas > 2)
            {

                workSheet.Cells[linha, 1, linha, colunas - 2].Merge = true;
                workSheet.Cells[linha, colunas - 1].Value = $"DATA: {DateTime.Now.ToShortDateString()} às {DateTime.Now:HH:mm}";
                workSheet.Cells[linha, colunas].Value = $"USUÁRIO: {usuario}";
            }
            else
            {
                workSheet.Cells[linha, 2].Value = $"DATA: {DateTime.Now.ToShortDateString()} às {DateTime.Now:HH:mm}";
                workSheet.Cells[linha, 3].Value = $"USUÁRIO: {usuario}";
            }
            linha++;

            workSheet.Row(linha).Height = 20;
            workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            workSheet.Row(linha).Style.Font.Bold = true;
            workSheet.Cells[linha, 1].Value = filtro;
            if (cabecalho != null && colunas > 2)
                workSheet.Cells[linha, 1, linha, colunas].Merge = true;
            else
                workSheet.Cells[linha, 1, linha, 3].Merge = true;

            if (cabecalho != null)
            {
                linha++;
                linha++;
                var cel = 1;
                foreach (var item in cabecalho)
                {
                    workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Row(linha).Style.Font.Bold = true;
                    workSheet.Cells[linha, cel++].Value = item.Key;
                }
            }
            linha++;
            return linha;
        }

        private static int ImprimirLinha(XGraphics gfx,
                                         string text,
                                         XFont font,
                                         int colIni,
                                         int colFim,
                                         int altura)
        {
            var tam = colFim - colIni;
            var qtdLinha = 1;
            if (string.IsNullOrWhiteSpace(text))
                text = "";

            XSize size = gfx.MeasureString(text, font);

            if (size.Width < tam)
            {
                gfx.DrawString(text, font, XBrushes.Black, new XRect(colIni, altura, 0, 0));
            }
            else
            {
                var strAux = text.Split(" ");
                var strRet = "";
                foreach (var item in strAux)
                {
                    XSize sizeRet = gfx.MeasureString(strRet + item, font);
                    if (sizeRet.Width <= tam)
                        strRet += item + " ";
                    else
                    {
                        gfx.DrawString(strRet.Trim(), font, XBrushes.Black, new XRect(colIni, altura, 0, 0));
                        qtdLinha++;
                        altura += 12;
                        strRet = item + " ";
                    }

                }
                if (!string.IsNullOrWhiteSpace(strRet))
                    gfx.DrawString(strRet, font, XBrushes.Black, new XRect(colIni, altura, 0, 0));
            }
            return qtdLinha;
        }

        private static void ImprimirLinhaDescrCont(XGraphics gfx,
                                            double x,
                                            double y,
                                            string descricao,
                                            string conteudo)
        {
            var fontBold = new XFont("Arial", 10, XFontStyle.Bold);
            var fontRegular = new XFont("Arial", 10, XFontStyle.Regular);

            ImprimirLinhaDescrCont(gfx, x, y, descricao, conteudo, fontBold, fontRegular);
        }

        private static void ImprimirLinhaDescrCont(XGraphics gfx,
                                                   double x,
                                                   double y,
                                                   string descricao,
                                                   string conteudo,
                                                   XFont fontBold,
                                                   XFont fontRegular)
        {
            XSize size = gfx.MeasureString($"{descricao}: ", fontBold);
            gfx.DrawString($"{descricao}: ", fontBold, XBrushes.Black, new XRect(x, y, 0, 0));
            gfx.DrawString(string.IsNullOrEmpty(conteudo) ? "" : conteudo, fontRegular, XBrushes.Black, new XRect(x + size.Width, y, 0, 0));
        }

        private static string MesExtenso(int mes)
        {
            string mesExtenso = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(mes).ToLower();
            return char.ToUpper(mesExtenso[0]) + mesExtenso[1..];
        }

        private static string SemanaExtenso(DateTime data)
        {
            string semanaExtenso = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetDayName(data.DayOfWeek).ToLower();
            return semanaExtenso.Substring(0, 3).ToUpper();
        }

        private FileStreamResult GerarRelatorio(string nomeArquivo,
                                                byte[] bytes,
                                                string mimeType,
                                                string extensao = "pdf")
        {
            var filename = $"{nomeArquivo}.{extensao}";
            var stream = new MemoryStream(bytes);
            var fileStreamResult = new FileStreamResult(stream, mimeType)
            {
                FileDownloadName = filename
            };
            Response.StatusCode = StatusCodes.Status200OK;
            return fileStreamResult;

        }

        private List<SelectListItem> ListaCongregacoes()
        {
            var _congregacoes = new List<SelectListItem>();
            foreach (var cong in _congregacaoAppService.GetAll(UserAppContext.Current.Usuario.Id).OrderBy(p => p.Nome))
            {
                _congregacoes.Add(new SelectListItem()
                {
                    Text = cong.Nome,
                    Value = cong.Id.ToString()
                });
            }
            return _congregacoes;
        }

        private FileStreamResult TratarException(Exception exc)
        {
            if (!(exc is Erro))
                _logger.LogError(exc, $"Usuário - {HttpContext.User.Identity.Name}");
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            string messages = JsonSerializer.Serialize(new
            {
                data = string.Join(Environment.NewLine, exc.FromHierarchy(ex1 => ex1.InnerException).Select(ex1 => ex1.Message))
            });

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(messages);
            writer.Flush();
            stream.Position = 0;
            return new FileStreamResult(stream, "application/json");
        }

        private static (byte[] FileContents, int Height, int Width) Resize(byte[] fileContents)
        {
            MemoryStream ms = new(fileContents);
            SKBitmap sourceBitmap = SKBitmap.Decode(ms);

            int height = sourceBitmap.Height;
            int width = sourceBitmap.Width;
            double tamMax = 68.0;

            if (height > tamMax)
            {
                double percentual = height / tamMax;
                height = (int)(height / percentual);
                width = (int)(width / percentual);
                SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.High);
                SKImage scaledImage = SKImage.FromBitmap(scaledBitmap);
                SKData data = scaledImage.Encode();

                return (data.ToArray(), height, width);
            }
            return (fileContents, height, width);
        }
        #endregion

        #region Views
        [Action(Menu.Relatorios, Menu.Aniversariantes)]
        public ActionResult Aniversariantes()
        {
            var aniver = new RelatorioViewModel();

            IEnumerable<TipoMembro> tipos = Enum.GetValues(typeof(TipoMembro))
                                                     .Cast<TipoMembro>();
            aniver.SelectTipoMembro = (from item in tipos
                                       select new SelectListItem
                                       {
                                           Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                           Value = item.ToString()
                                       }).ToList();

            var _congregacoes = ListaCongregacoes();
            aniver.SelectCongregacoes = _congregacoes;

            aniver.DataInicio = DateTimeOffset.Now.Date;
            aniver.DataFinal = DateTimeOffset.Now.Date;

            IEnumerable<Saida> formato = Enum.GetValues(typeof(Saida))
                                         .Cast<Saida>();
            aniver.SelectTipoSaida = (from item in formato
                                      select new SelectListItem
                                      {
                                          Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                          Value = item.ToString()
                                      }).ToList();
            return View(aniver);
        }

        [Action(Menu.Relatorios, Menu.CandidatosBatismo)]
        public ActionResult CandidatosBatismo()
        {
            var candBat = new CandidatosBatismoViewModel();

            var _congregacoes = ListaCongregacoes();
            candBat.SelectCongregacoes = _congregacoes;

            var datas = _batismoAppService.GetAll(UserAppContext.Current.Usuario.Id);

            var _dataBatismo = new List<SelectListItem>();

            foreach (var item in datas)
            {
                _dataBatismo.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = String.Format("{0:dd/MM/yyyy}", item.DataBatismo)
                });
            }
            candBat.SelectDatasBatismo = _dataBatismo;

            var situacao = Enum.GetValues(typeof(SituacaoCandidatoBatismo)).Cast<SituacaoCandidatoBatismo>();
            candBat.SelectSituacao = (from item in situacao
                                      select new SelectListItem
                                      {
                                          Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                          Value = item.ToString()
                                      }).ToList();

            IEnumerable<Saida> formato = Enum.GetValues(typeof(Saida))
                                        .Cast<Saida>();
            candBat.SelectTipoSaida = (from item in formato
                                       select new SelectListItem
                                       {
                                           Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                           Value = item.ToString()
                                       }).ToList();

            return View(candBat);
        }

        [Action(Menu.Relatorios, Menu.Nascimentos)]
        public ActionResult Nascimentos()
        {
            var nasc = new NascimentosViewModel();

            var _congregacoes = ListaCongregacoes();
            nasc.SelectCongregacoes = _congregacoes;

            nasc.DataInicio = DateTimeOffset.Now.Date;
            nasc.DataFinal = DateTimeOffset.Now.Date;

            IEnumerable<Saida> formato = Enum.GetValues(typeof(Saida))
                                       .Cast<Saida>();
            nasc.SelectTipoSaida = (from item in formato
                                    select new SelectListItem
                                    {
                                        Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                        Value = item.ToString()
                                    }).ToList();
            return View(nasc);
        }

        [Action(Menu.Relatorios, Menu.Casamentos)]
        public ActionResult Casamentos()
        {
            var casamento = new RelatorioViewModel();

            var _congregacoes = ListaCongregacoes();
            casamento.SelectCongregacoes = _congregacoes;

            casamento.DataInicio = DateTimeOffset.Now.Date;
            casamento.DataFinal = DateTimeOffset.Now.Date;

            IEnumerable<Saida> formato = Enum.GetValues(typeof(Saida))
                                       .Cast<Saida>();
            casamento.SelectTipoSaida = (from item in formato
                                         select new SelectListItem
                                         {
                                             Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                             Value = item.ToString()
                                         }).ToList();
            return View(casamento);
        }

        [Action(Menu.Relatorios, Menu.Transferencia)]
        public ActionResult Transferencia()
        {
            var trans = new RelatorioViewModel();

            var _congregacoes = ListaCongregacoes();
            trans.SelectCongregacoes = _congregacoes;

            trans.DataInicio = DateTimeOffset.Now.Date;
            trans.DataFinal = DateTimeOffset.Now.Date;


            IEnumerable<Saida> formato = Enum.GetValues(typeof(Saida))
                                      .Cast<Saida>();
            trans.SelectTipoSaida = (from item in formato
                                     select new SelectListItem
                                     {
                                         Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                         Value = item.ToString()
                                     }).ToList();

            return View(trans);
        }

        [Action(Menu.Relatorios, Menu.Congregacoes)]
        public ActionResult Congregacoes()
        {
            var congreg = new RelatorioViewModel();

            var _congregacoes = ListaCongregacoes();
            congreg.SelectCongregacoes = _congregacoes;

            congreg.DataInicio = DateTimeOffset.Now.Date;
            congreg.DataFinal = DateTimeOffset.Now.Date;

            IEnumerable<Saida> formato = Enum.GetValues(typeof(Saida))
                                      .Cast<Saida>();
            congreg.SelectTipoSaida = (from item in formato
                                       select new SelectListItem
                                       {
                                           Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                           Value = item.ToString()
                                       }).ToList();
            return View(congreg);
        }

        [Action(Menu.Relatorios, Menu.Obreiros)]
        public ActionResult Obreiros()
        {
            var obr = new RelatorioViewModel();

            var _congregacoes = ListaCongregacoes();
            obr.SelectCongregacoes = _congregacoes;

            IEnumerable<Saida> tipos = Enum.GetValues(typeof(Saida))
                                           .Cast<Saida>();
            obr.SelectTipoSaida = (from item in tipos
                                   select new SelectListItem
                                   {
                                       Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                       Value = item.ToString()
                                   }).ToList();

            return View(obr);
        }

        [Action(Menu.Relatorios, Menu.RelMensal)]
        public ActionResult RelMensal()
        {
            var relMensal = new RelatorioViewModel();
            var _congregacoes = ListaCongregacoes();
            relMensal.SelectCongregacoes = _congregacoes;

            relMensal.Ano = DateTimeOffset.Now.Year;

            return View(relMensal);
        }

        [Action(Menu.Relatorios, Menu.RelatorioEventos)]
        public ActionResult Eventos()
        {
            var eventos = new RelatorioViewModel();
            var _congregacoes = ListaCongregacoes();
            eventos.SelectCongregacoes = _congregacoes;

            eventos.Ano = DateTimeOffset.Now.Year;

            eventos.SelectTipoEvento = (from item in Enum.GetValues(typeof(TipoEvento)).Cast<TipoEvento>()
                                        select new SelectListItem
                                        {
                                            Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "" : item.GetDisplayAttributeValue(),
                                            Value = ((int)item).ToString()
                                        }).ToList().Where(x => !string.IsNullOrWhiteSpace(x.Text)).ToList();

            return View(eventos);
        }

        [Action(Menu.Relatorios, Menu.RelMembros)]
        public ActionResult RelMembros()
        {
            var membro = new MembroVM();

            var _congregacoes = ListaCongregacoes();
            membro.SelectCongregacoes = _congregacoes;

            IEnumerable<TipoMembro> tipos = Enum.GetValues(typeof(TipoMembro))
                                                       .Cast<TipoMembro>();
            membro.SelectTipoMembro = (from item in tipos
                                       select new SelectListItem
                                       {
                                           Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                           Value = item.ToString()
                                       }).ToList();

            IEnumerable<Status> status = Enum.GetValues(typeof(Status))
                                                       .Cast<Status>();
            membro.SelectStatus = (from item in status
                                   select new SelectListItem
                                   {
                                       Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                       Value = item.ToString()
                                   }).ToList();

            IEnumerable<EstadoCivil> estadoCivil = Enum.GetValues(typeof(EstadoCivil))
                                                       .Cast<EstadoCivil>();
            membro.SelectEstadoCivil = (from item in estadoCivil
                                        select new SelectListItem
                                        {
                                            Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                            Value = item.ToString()
                                        }).ToList();

            IEnumerable<Saida> formato = Enum.GetValues(typeof(Saida))
                                         .Cast<Saida>();
            membro.SelectTipoSaida = (from item in formato
                                      select new SelectListItem
                                      {
                                          Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                          Value = item.ToString()
                                      }).ToList();

            return View(membro);
        }

        [Action(Menu.Relatorios, Menu.CursosMembro)]
        public ActionResult CursosMembro()
        {
            var curso = new CursoMembroVM
            {
                SelectCongregacoes = ListaCongregacoes()
            };
            var listacursos = _cursoAppService.GetAll(UserAppContext.Current.Usuario.Id);
            curso.SelectCursos = new List<SelectListItem>();
            listacursos.ToList().ForEach(c => curso.SelectCursos.Add(new SelectListItem() { Text = c.Descricao, Value = c.Id.ToString() }));

            IEnumerable<Saida> formato = Enum.GetValues(typeof(Saida))
                                      .Cast<Saida>();
            curso.SelectTipoSaida = (from item in formato
                                     select new SelectListItem
                                     {
                                         Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                         Value = item.ToString()
                                     }).ToList();

            return View(curso);
        }

        [HttpPost]
        public JsonResult GetDatasPresenca(
            [FromServices] ILogger<PresencaController> logger,
            int idPresenca)
        {
            try
            {
                var presenca = _presencaAppService.ListarPresencaDatas(idPresenca);

                return Json(new
                {
                    status = "OK",
                    data = presenca.OrderBy(a => a.DataHoraInicio)
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "ERROR", ex.Message });
            }
        }

        [Action(Menu.Relatorios, Menu.PresencaInscritos)]
        public ActionResult PresencaInscritos()
        {
            var presencaVM = new PresencaVM
            {
                ListaCongregacoes = ListaCongregacoes()
            };
            if (presencaVM.ListaCongregacoes.Count == 1)
            {
                presencaVM.CongregacaoId = (int)UserAppContext.Current.Usuario.Congregacao.Id;
            };

            var _lstPresenca = _presencaAppService.GetAll(UserAppContext.Current.Usuario.Id);
            foreach (var item in _lstPresenca)
            {
                presencaVM.ListaPresenca.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = $"{item.Descricao} - Data Inicio: {item.DataHoraInicio.ToShortDateString()} - Data Máxima: {item.DataMaxima.ToShortDateString()}"
                });
            }
            return View(presencaVM);
        }

        [Action(Menu.Relatorios, Menu.PresencaFrequencia)]
        public ActionResult PresencaFrequencia()
        {
            var presencaVM = new PresencaVM
            {
                ListaCongregacoes = ListaCongregacoes()
            };

            var _lstPresenca = _presencaAppService.GetAll(UserAppContext.Current.Usuario.Id);
            foreach (var item in _lstPresenca)
            {
                presencaVM.ListaPresenca.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = $"{item.Descricao} - Data Inicio: {item.DataHoraInicio.ToShortDateString()} - Data Máxima: {item.DataMaxima.ToShortDateString()}"
                });
            }
            IEnumerable<Saida> formato = Enum.GetValues(typeof(Saida))
                                     .Cast<Saida>();
            presencaVM.SelectTipoSaida = (from item in formato
                                          select new SelectListItem
                                          {
                                              Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                              Value = item.ToString()
                                          }).ToList();

            return View(presencaVM);
        }

        [Action(Menu.Relatorios, Menu.PresencaListaData)]
        public ActionResult PresencaLista()
        {
            var presencaVM = new PresencaListaVM
            {
                ListaCongregacoes = ListaCongregacoes()
            };
            if (presencaVM.ListaCongregacoes.Count == 1)
            {
                presencaVM.CongregacaoId = (int)UserAppContext.Current.Usuario.Congregacao.Id;
            };

            var _tipoEvento = new List<SelectListItem>();
            foreach (var evt in _tipoEventoService.GetAll(UserAppContext.Current.Usuario.Id).OrderBy(p => p.Descricao))
            {
                presencaVM.ListaTipoEventos.Add(new SelectListItem()
                {
                    Text = evt.Descricao,
                    Value = evt.Id.ToString()
                });
            }

            var _cargos = new List<SelectListItem>();
            presencaVM.ListaCargos.Add(new SelectListItem()
            {
                Text = "Membro/Visitante",
                Value = "0"
            });
            foreach (var carg in _cargoService.GetAll(UserAppContext.Current.Usuario.Id).OrderBy(p => p.Descricao))
            {
                presencaVM.ListaCargos.Add(new SelectListItem()
                {
                    Text = carg.Descricao,
                    Value = carg.Id.ToString()
                });
            }

            return View(presencaVM);
        }

        [Action(Menu.Relatorios, Menu.Carteirinhas)]
        public ActionResult CarteirinhaLista()
        {
            var congr = ListaCongregacoes();
            var cartVM = new CarteirinhaVM
            {
                ListaCongregacoesBatismo = congr,
                ListaCongregacoesMembro = congr
            };

            var _cargos = new List<SelectListItem>();
            cartVM.ListaCargos.Add(new SelectListItem()
            {
                Text = "Membro",
                Value = "0"
            });
            foreach (var carg in _cargoService.GetAll(UserAppContext.Current.Usuario.Id).OrderBy(p => p.Descricao))
            {
                cartVM.ListaCargos.Add(new SelectListItem()
                {
                    Text = carg.Descricao,
                    Value = carg.Id.ToString()
                });
            }

            var datas = _batismoAppService.GetAll(UserAppContext.Current.Usuario.Id);
            var _dataBatismo = new List<SelectListItem>();
            foreach (var item in datas)
            {
                _dataBatismo.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = String.Format("{0:dd/MM/yyyy}", item.DataBatismo)
                });
            }
            cartVM.ListaDatasBatismo = _dataBatismo;

            return View(cartVM);
        }
        #endregion

        #region Relatorios

        #region Aniversariantes
        public FileStreamResult RelatorioAniversariantes(string congregacao,
                                                         string dataInicio,
                                                         string dataFinal,
                                                         string tipoMembro,
                                                         string formato)
        {
            try
            {
                DateTime.TryParse(dataInicio, out DateTime dtInicio);
                DateTime.TryParse(dataFinal, out DateTime dtFinal);
                int.TryParse(congregacao, out int congreg);
                Enum.TryParse(typeof(TipoMembro), tipoMembro, out object tpMembro);
                var laniver = _relatoriosAppService.Aniversariantes(dtInicio, dtFinal, congreg, UserAppContext.Current.Usuario.Id, (TipoMembro)tpMembro);
                var aniver = from a in laniver
                             orderby a.DataNascimento.Day, a.DataNascimento.Month, a.Congregacao, a.Nome
                             select a;

                if (!aniver.Any())
                    throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

                var filtro = $"Data Inicio: {dtInicio.Date.ToShortDateString()} - Data Final: {dtFinal.Date.ToShortDateString()}";
                if (congreg > 0)
                    filtro += $" - Congregação: {_congregacaoAppService.GetById(congreg, UserAppContext.Current.Usuario.Id).Nome}";
                else
                    filtro += " - Todas as Congregações";

                if ((TipoMembro)tpMembro != TipoMembro.NaoDefinido)
                    filtro += $" - Tipo Membro: {((TipoMembro)tpMembro).GetDisplayAttributeValue()}";
                else
                    filtro += " - Tipo Membro: Todos";

                var nomeArquivo = $"Aniversariantes_{dtInicio.Date.ToShortDateString().Replace("/", "")}_{dtFinal.Date.ToShortDateString().Replace("/", "")}";

                if (formato == "Excel")
                    return RelatorioAniversariantesExcel(aniver, filtro, nomeArquivo);

                return RelatorioAniversariantesPDF(aniver, filtro, nomeArquivo);
            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        private FileStreamResult RelatorioAniversariantesPDF(IEnumerable<Aniversariantes> aniver,
                                                             string filtro,
                                                             string nomeArquivo)
        {
            var document = new PdfDocument();
            document.Info.Title = "Relatório de Aniversariantes";
            document.Info.Author = "Architect Systems";
            document.Info.Subject = "Relatório de Aniversariantes";
            document.Info.Keywords = "PDFsharp, XGraphics";

            PdfPage page = document.AddPage();
            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
            page.Size = PdfSharpCore.PageSize.A4;
            XGraphics gfx = XGraphics.FromPdfPage(page);

            int comecaDados = 10;
            var contaPagina = 1;
            Dictionary<string, int> cabecalho = new Dictionary<string, int>
                    {
                        { "CONGREGAÇÃO", 25 },
                        { "NOME", 280 },
                        { "DATA NASC.", 520 }
                    };
            var nomecliente = _configuration["ParametrosSistema:NomeCliente"].ToString();

            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "ANIVERSARIANTES", UserAppContext.Current.Usuario.Username,
                comecaDados, filtro, cabecalho);

            foreach (var item in aniver)
            {
                var font = new XFont("Arial", 8, XFontStyle.Regular);
                gfx.DrawString(item.Congregacao, font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                gfx.DrawString(item.Nome, font, XBrushes.Black, new XRect(280, comecaDados + 80, 0, 0));
                var dtNasc = $"{item.DataNascimento.Day.ToString().PadLeft(2, '0')}/{item.DataNascimento.Month.ToString().PadLeft(2, '0')}";
                gfx.DrawString(dtNasc, font, XBrushes.Black, new XRect(535, comecaDados + 80, 0, 0));

                comecaDados += 12;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "ANIVERSARIANTES", UserAppContext.Current.Usuario.Username,
                                       comecaDados, filtro, cabecalho);
                }
            }
            comecaDados += 2;
            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 80, 575, comecaDados + 80);
            comecaDados += 10;
            var fontRodape = new XFont("Arial", 8, XFontStyle.Bold);
            gfx.DrawString($"Total de Aniversáriantes: {aniver.Count()} - Total de Paginas: {contaPagina}", fontRodape, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/pdf");
        }

        private FileStreamResult RelatorioAniversariantesExcel(IEnumerable<Aniversariantes> aniver,
                                                               string filtro,
                                                               string nomeArquivo)
        {
            if (!aniver.Any())
                throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("Aniversariantes");

            Dictionary<string, int> cabecalho = new Dictionary<string, int>
                    {
                        { "CONGREGAÇÃO", 25 },
                        { "NOME", 280 },
                        { "DATA NASC.", 520 }
                    };

            int linha = RelatorioCabecalhoExcel(workSheet, _configuration["ParametrosSistema:NomeCliente"].ToString(), "ANIVERSARIANTES",
                UserAppContext.Current.Usuario.Username, filtro, cabecalho);

            foreach (var item in aniver)
            {
                workSheet.Cells[linha, 1].Value = item.Congregacao;
                workSheet.Cells[linha, 2].Value = item.Nome;
                var dtNasc = $"{item.DataNascimento.Day.ToString().PadLeft(2, '0')}/{item.DataNascimento.Month.ToString().PadLeft(2, '0')}";
                workSheet.Cells[linha, 3].Value = dtNasc;
                linha++;
            }
            workSheet.Cells[linha, 1].Value = $"Total de Aniversáriantes: {aniver.Count()}";

            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();

            var stream = new MemoryStream(excel.GetAsByteArray());
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/octet-stream", "xlsx");

        }
        #endregion

        #region Candidatos ao Batismo
        public FileStreamResult RelatorioCandidatosBatismo(string congregacao,
                                                           long batismoId,
                                                           string dataBatismo,
                                                           string situacao,
                                                           string formato)
        {
            try
            {
                int.TryParse(congregacao, out int congreg);
                Enum.TryParse(situacao, out SituacaoCandidatoBatismo sit);
                DateTimeOffset.TryParse(dataBatismo, out DateTimeOffset dtBatismo);

                var batismo = _relatoriosAppService.CandidatosBatismo(batismoId, dtBatismo, congreg, sit, UserAppContext.Current.Usuario.Id);

                if (!batismo.Any())
                    throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

                var filtro = "";
                if (batismoId > 0)
                    filtro = $"Data: {dtBatismo.Date.ToShortDateString()}";
                else
                    filtro = "Todas as Datas";

                if (congreg > 0)
                    filtro += $" - Congregação: {_congregacaoAppService.GetById(congreg, UserAppContext.Current.Usuario.Id).Nome}";
                else
                    filtro += " - Todas as Congregações";

                if (sit != SituacaoCandidatoBatismo.Nulo)
                    filtro += $" - Situação: {sit.GetDisplayAttributeValue()}";
                else
                    filtro += " - Todas as Situações";
                var nomeArquivo = $"ListaPresenca_{(dtBatismo.Date != DateTime.MinValue ? dtBatismo.Date.ToShortDateString().Replace("/", "") : "TODOS")}";

                if (formato == "Excel")
                    return RelatorioCandidatosBatismoExcel(batismo, filtro, nomeArquivo);
                return RelatorioCandidatosBatismoPDF(batismo, filtro, nomeArquivo);
            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        private FileStreamResult RelatorioCandidatosBatismoPDF(IEnumerable<Batismo> batismo,
                                                               string filtro,
                                                               string nomeArquivo)
        {
            var document = new PdfDocument();
            document.Info.Title = "Relatório de Lista de Presença - Batismo";
            document.Info.Author = "Architect Systems";
            document.Info.Subject = "Lista de Presença - Batismo";
            document.Info.Keywords = "PDFsharp, XGraphics";

            PdfPage page = document.AddPage();
            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);

            int comecaDados = 10;
            var contaPagina = 1;
            Dictionary<string, int> cabecalho = new Dictionary<string, int>()
                {
                    { "CANDIDATO", 25 },
                    { "CAPA TAM.", 350 },
                    { "DATA NASC", 400 },
                    { "SITUAÇÃO", 470 }
                };

            var nomecliente = _configuration["ParametrosSistema:NomeCliente"].ToString();

            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA DE PRESENÇA - BATISMO", UserAppContext.Current.Usuario.Username,
                comecaDados, filtro, cabecalho);

            var grpDatas = batismo.GroupBy(x => new { x.Id, x.DataBatismo, x.Status, TotalCandidatos = x.Candidatos.Count() })
                            .Select(g => new { g.Key.Id, g.Key.DataBatismo, g.Key.Status, g.Key.TotalCandidatos, Total = g.Count() })
                            .OrderByDescending(o => o.DataBatismo);

            foreach (var itemData in grpDatas)
            {
                var font = new XFont("Arial", 10, XFontStyle.Bold);
                gfx.DrawString($"DATA: {itemData.DataBatismo.Date.ToShortDateString()} - Situação: {itemData.Status.GetDisplayAttributeValue()}", font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                comecaDados += 24;

                var batismoSel = batismo.FirstOrDefault(b => b.Id == itemData.Id);
                /*CONGREGACAO*/
                var grpCongr = batismoSel.Candidatos
                                    .GroupBy(x => new { x.CongregacaoId, x.CongregacaoNome })
                                    .Select(g => new { g.Key.CongregacaoId, g.Key.CongregacaoNome, Total = g.Count() })
                                    .OrderBy(o => o.CongregacaoNome);

                foreach (var itemCongr in grpCongr)
                {
                    font = new XFont("Arial", 10, XFontStyle.Bold);
                    gfx.DrawString($"CONGREGAÇÃO: {itemCongr.CongregacaoNome}", font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);

                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA DE PRESENÇA - BATISMO", UserAppContext.Current.Usuario.Username,
                                           comecaDados, filtro, cabecalho);
                    }

                    var candCongr = batismoSel.Candidatos.Where(c => c.CongregacaoId == itemCongr.CongregacaoId).OrderBy(o => o.Nome);
                    foreach (var itemMembro in candCongr)
                    {
                        font = new XFont("Arial", 10, XFontStyle.Regular);
                        gfx.DrawString($"{itemMembro.Id} - {itemMembro.Nome}", font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                        gfx.DrawString(itemMembro.TamanhoCapa.GetDisplayAttributeValue(), font, XBrushes.Black, new XRect(350, comecaDados + 80, 0, 0));
                        gfx.DrawString(itemMembro.DataNascimento.Date.ToShortDateString(), font, XBrushes.Black, new XRect(400, comecaDados + 80, 0, 0));
                        gfx.DrawString(itemMembro.Situacao, font, XBrushes.Black, new XRect(470, comecaDados + 80, 0, 0));
                        comecaDados += 12;
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);

                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA DE PRESENÇA - BATISMO", UserAppContext.Current.Usuario.Username,
                                               comecaDados, filtro, cabecalho);
                        }
                        if (itemMembro.Observacoes.Any())
                        {
                            font = new XFont("Arial", 10, XFontStyle.Bold);
                            gfx.DrawString("Observações:", font, XBrushes.Black, new XRect(40, comecaDados + 80, 0, 0));
                            comecaDados += 12;
                            if (comecaDados > 740)
                            {
                                page = document.AddPage();
                                page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                                page.Size = PdfSharpCore.PageSize.A4;
                                gfx = XGraphics.FromPdfPage(page);

                                contaPagina += 1;
                                comecaDados = 10;
                                comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA DE PRESENÇA - BATISMO", UserAppContext.Current.Usuario.Username,
                                                   comecaDados, filtro, cabecalho);
                            }
                            int linhaObs = 1;
                            foreach (var obs in itemMembro.Observacoes)
                            {
                                font = new XFont("Arial", 10, XFontStyle.Regular);
                                gfx.DrawString($"{linhaObs} - {obs.Observacao}", font, XBrushes.Black, new XRect(40, comecaDados + 80, 0, 0));
                                comecaDados += 12;
                                if (comecaDados > 740)
                                {
                                    page = document.AddPage();
                                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                                    page.Size = PdfSharpCore.PageSize.A4;
                                    gfx = XGraphics.FromPdfPage(page);

                                    contaPagina += 1;
                                    comecaDados = 10;
                                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA DE PRESENÇA - BATISMO", UserAppContext.Current.Usuario.Username,
                                                       comecaDados, filtro, cabecalho);
                                }
                                linhaObs++;
                            }
                        }
                    }

                    font = new XFont("Arial", 10, XFontStyle.Bold);
                    gfx.DrawString($"CONGREGAÇÃO: {itemCongr.CongregacaoId} - {itemCongr.CongregacaoNome} - TOTAL: {itemCongr.Total}", font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                    comecaDados += 24;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);

                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA DE PRESENÇA - BATISMO", UserAppContext.Current.Usuario.Username,
                                           comecaDados, filtro, cabecalho);
                    }
                }

                font = new XFont("Arial", 10, XFontStyle.Bold);
                gfx.DrawString($"DATA: {itemData.DataBatismo.Date.ToShortDateString()} - TOTAL: {itemData.TotalCandidatos}", font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                comecaDados += 12;
                if (comecaDados > 700)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);

                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA DE PRESENÇA - BATISMO", UserAppContext.Current.Usuario.Username,
                                       comecaDados, filtro, cabecalho);
                }
                comecaDados += 12;

                if (grpDatas.Count() == 1)
                {
                    font = new XFont("Arial", 10, XFontStyle.Bold);
                    comecaDados += 12;
                    if (comecaDados > 700)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);

                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA DE PRESENÇA - BATISMO", UserAppContext.Current.Usuario.Username,
                                           comecaDados, filtro, cabecalho);
                    }
                    gfx.DrawString("Pastor(es) Celebrante(s):", font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA DE PRESENÇA - BATISMO", UserAppContext.Current.Usuario.Username,
                                           comecaDados, filtro, cabecalho);
                    }

                    foreach (var itemPastor in batismoSel.PastorCelebrante)
                    {
                        font = new XFont("Arial", 10, XFontStyle.Regular);
                        gfx.DrawString(itemPastor.Nome, font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                        comecaDados += 12;
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA DE PRESENÇA - BATISMO", UserAppContext.Current.Usuario.Username,
                                               comecaDados, filtro, cabecalho);
                        }

                    }
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA DE PRESENÇA - BATISMO", UserAppContext.Current.Usuario.Username,
                                           comecaDados, filtro, cabecalho);
                    }

                    XRect rect = new XRect(0, comecaDados + 80, page.Width, 140);
                    XStringFormat format = new XStringFormat
                    {
                        Alignment = XStringAlignment.Center
                    };
                    gfx.DrawString("___________________________________________________", font, XBrushes.Black, rect, format);
                    comecaDados += 15;
                    font = new XFont("Arial", 10, XFontStyle.Bold);
                    var congrSede = _congregacaoAppService.GetSede();
                    rect = new XRect(0, comecaDados + 80, page.Width, 140);
                    gfx.DrawString(congrSede.PastorResponsavelNome, font, XBrushes.Black, rect, format);

                }
            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/pdf");
        }

        private FileStreamResult RelatorioCandidatosBatismoExcel(IEnumerable<Batismo> batismo,
                                                                 string filtro,
                                                                 string nomeArquivo)
        {
            if (!batismo.Any())
                throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("Candidatos");

            Dictionary<string, int> cabecalho = new Dictionary<string, int>()
                {
                    { "CANDIDATO", 25 },
                    { "CAPA TAM.", 350 },
                    { "DATA NASC", 400 },
                    { "SITUAÇÃO", 470 }
                };

            int linha = RelatorioCabecalhoExcel(workSheet, _configuration["ParametrosSistema:NomeCliente"].ToString(), "LISTA DE PRESENÇA - BATISMO",
                UserAppContext.Current.Usuario.Username, filtro, cabecalho);
            var grpDatas = batismo.GroupBy(x => new { x.Id, x.DataBatismo, x.Status, TotalCandidatos = x.Candidatos.Count() })
                            .Select(g => new { g.Key.Id, g.Key.DataBatismo, g.Key.Status, g.Key.TotalCandidatos, Total = g.Count() })
                            .OrderByDescending(o => o.DataBatismo);

            foreach (var itemData in grpDatas)
            {
                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 1].Value = $"DATA: {itemData.DataBatismo.Date.ToShortDateString()} - Situação: {itemData.Status.GetDisplayAttributeValue()}";
                linha++;

                var batismoSel = batismo.FirstOrDefault(b => b.Id == itemData.Id);
                /*CONGREGACAO*/
                var grpCongr = batismoSel.Candidatos
                                    .GroupBy(x => new { x.CongregacaoId, x.CongregacaoNome })
                                    .Select(g => new { g.Key.CongregacaoId, g.Key.CongregacaoNome, Total = g.Count() })
                                    .OrderBy(o => o.CongregacaoNome);
                foreach (var itemCongr in grpCongr)
                {
                    workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Row(linha).Style.Font.Bold = true;
                    workSheet.Cells[linha, 1].Value = $"CONGREGAÇÃO: {itemCongr.CongregacaoNome}";
                    linha++;

                    var candCongr = batismoSel.Candidatos.Where(c => c.CongregacaoId == itemCongr.CongregacaoId).OrderBy(o => o.Nome);
                    foreach (var itemMembro in candCongr)
                    {
                        workSheet.Cells[linha, 1].Value = $"{itemMembro.Id} - {itemMembro.Nome}";
                        workSheet.Cells[linha, 2].Value = itemMembro.TamanhoCapa.GetDisplayAttributeValue();
                        workSheet.Cells[linha, 3].Value = itemMembro.DataNascimento.Date.ToShortDateString();
                        workSheet.Cells[linha, 4].Value = itemMembro.Situacao;
                        linha++;
                        if (itemMembro.Observacoes.Any())
                        {
                            workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            workSheet.Row(linha).Style.Font.Bold = true;
                            workSheet.Cells[linha, 1].Value = "Observações:";
                            linha++;

                            int linhaObs = 1;
                            foreach (var obs in itemMembro.Observacoes)
                            {
                                workSheet.Cells[linha, 1].Value = $"{linhaObs} - {obs.Observacao}";
                                linhaObs++;
                                linha++;
                            }
                        }
                    }
                    workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Row(linha).Style.Font.Bold = true;
                    workSheet.Cells[linha, 1].Value = $"CONGREGAÇÃO: {itemCongr.CongregacaoId} - {itemCongr.CongregacaoNome} - TOTAL: {itemCongr.Total}";
                    linha++;
                }
                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 1].Value = $"DATA: {itemData.DataBatismo.Date.ToShortDateString()} - TOTAL: {itemData.TotalCandidatos}";
                linha++;
                linha++;
                if (grpDatas.Count() == 1)
                {
                    workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Row(linha).Style.Font.Bold = true;
                    workSheet.Cells[linha, 1].Value = "Pastor(es) Celebrante(s):";
                    linha++;

                    foreach (var itemPastor in batismoSel.PastorCelebrante)
                    {
                        workSheet.Cells[linha, 1].Value = itemPastor.Nome;
                        linha++;
                    }
                    linha++;
                    workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[linha, 1, linha, 4].Merge = true;
                    workSheet.Cells[linha, 1].Value = "___________________________________________________";
                    linha++;

                    var congrSede = _congregacaoAppService.GetSede();
                    workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(linha).Style.Font.Bold = true;
                    workSheet.Cells[linha, 1, linha, 4].Merge = true;
                    workSheet.Cells[linha, 1].Value = congrSede.PastorResponsavelNome;
                    linha++;
                }
            }

            for (int i = 1; i <= cabecalho.Count(); i++)
                workSheet.Column(i).AutoFit();

            var stream = new MemoryStream(excel.GetAsByteArray());
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/octet-stream", "xlsx");

        }
        #endregion

        #region Casamentos
        public FileStreamResult RelatorioCasamentos(string congregacao,
                                                    string dataInicio,
                                                    string dataFinal,
                                                    string formato)
        {
            try
            {
                DateTimeOffset.TryParse(dataInicio, out DateTimeOffset dtInicio);
                DateTimeOffset.TryParse(dataFinal, out DateTimeOffset dtFinal);
                int.TryParse(congregacao, out int congr);

                var casam = _relatoriosAppService.Casamento(dtInicio, dtFinal, congr, UserAppContext.Current.Usuario.Id);
                if (!casam.Any())
                    throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

                var filtro = $"Data Inicio: {dtInicio.Date.ToShortDateString()} - Data Final: {dtFinal.Date.ToShortDateString()}";
                if (congr > 0)
                    filtro += $" - Congregação: {_congregacaoAppService.GetById(congr, UserAppContext.Current.Usuario.Id).Nome}";
                else
                    filtro += " - Todas as Congregações";
                var nomeArquivo = $"Casamentos_{dtInicio.Date.ToShortDateString().Replace("/", "")}_{dtFinal.Date.ToShortDateString().Replace("/", "")}";

                if (formato == "Excel")
                    return RelatorioCasamentosExcel(casam, filtro, nomeArquivo);

                return RelatorioCasamentosPDF(casam, filtro, nomeArquivo);
            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        private FileStreamResult RelatorioCasamentosPDF(IEnumerable<Casamento> casam,
                                                        string filtro,
                                                        string nomeArquivo)
        {
            var document = new PdfDocument();
            document.Info.Title = "Relatório de Membros - Curso";
            document.Info.Author = "Architect Systems";
            document.Info.Subject = "Membros - Curso";
            document.Info.Keywords = "PDFsharp, XGraphics";

            PdfPage page = document.AddPage();
            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
            page.Size = PdfSharpCore.PageSize.A4;
            XGraphics gfx = XGraphics.FromPdfPage(page);

            int comecaDados = 10;
            var contaPagina = 1;
            Dictionary<string, int> cabecalho = new Dictionary<string, int>();

            var nomecliente = _configuration["ParametrosSistema:NomeCliente"].ToString();

            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "CASAMENTOS", UserAppContext.Current.Usuario.Username,
                comecaDados, filtro, cabecalho);

            var grpCongr = casam.GroupBy(x => new { x.CongregacaoNome })
                            .Select(g => new { Congregacao = g.Key.CongregacaoNome, Total = g.Count() })
                            .OrderBy(o => o.Congregacao);

            foreach (var itemCongr in grpCongr)
            {
                var fontBold = new XFont("Arial", 10, XFontStyle.Bold);
                var fontRegular = new XFont("Arial", 10, XFontStyle.Regular);
                gfx.DrawString($"Congregação: {itemCongr.Congregacao}", fontBold, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));

                var grpCasam = casam.Where(c => c.CongregacaoNome == itemCongr.Congregacao);
                foreach (var itemCasam in grpCasam)
                {
                    comecaDados += 24;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);

                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "CASAMENTOS", UserAppContext.Current.Usuario.Username,
                                           comecaDados, filtro, cabecalho);
                    }
                    ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Pastor", itemCasam.PastorNome);

                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);

                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "CASAMENTOS", UserAppContext.Current.Usuario.Username,
                                           comecaDados, filtro, cabecalho);
                    }
                    ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Data do Casamento", itemCasam.DataHora);

                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);

                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "CASAMENTOS", UserAppContext.Current.Usuario.Username,
                                           comecaDados, filtro, cabecalho);
                    }
                    ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Noivo", itemCasam.NoivoNome);

                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);

                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "CASAMENTOS", UserAppContext.Current.Usuario.Username,
                                           comecaDados, filtro, cabecalho);
                    }
                    ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Pai Noivo", itemCasam.PaiNoivoNome);

                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);

                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "CASAMENTOS", UserAppContext.Current.Usuario.Username,
                                           comecaDados, filtro, cabecalho);
                    }
                    ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Mãe Noivo", itemCasam.MaeNoivoNome);

                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);

                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "CASAMENTOS", UserAppContext.Current.Usuario.Username,
                                           comecaDados, filtro, cabecalho);
                    }
                    ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Noiva", itemCasam.NoivaNome);

                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);

                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "CASAMENTOS", UserAppContext.Current.Usuario.Username,
                                           comecaDados, filtro, cabecalho);
                    }
                    ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Pai Noiva", itemCasam.PaiNoivaNome);

                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);

                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "CASAMENTOS", UserAppContext.Current.Usuario.Username,
                                           comecaDados, filtro, cabecalho);
                    }
                    ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Mãe Noiva", itemCasam.MaeNoivaNome);
                }

                comecaDados += 24;
                fontBold = new XFont("Arial", 10, XFontStyle.Bold);
                gfx.DrawString($"Total de Casamentos: {itemCongr.Total} - Congregação: {itemCongr.Congregacao}", fontBold, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                comecaDados += 24;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);

                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "CASAMENTOS", UserAppContext.Current.Usuario.Username,
                                       comecaDados, filtro, cabecalho);
                }
            }
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/pdf");
        }

        private FileStreamResult RelatorioCasamentosExcel(IEnumerable<Casamento> casam,
                                                          string filtro,
                                                          string nomeArquivo)
        {
            if (!casam.Any())
                throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("Casamentos");

            Dictionary<string, int> cabecalho = new Dictionary<string, int>
                    {
                        {"Pastor", 1},
                        {"Data do Casamento", 2},
                        {"Noivo", 3},
                        {"Pai Noivo", 4},
                        {"Mãe Noivo", 5},
                        {"Noiva", 6},
                        {"Pai Noiva", 7},
                        {"Mãe Noiva", 8}
                    };
            int linha = RelatorioCabecalhoExcel(workSheet, _configuration["ParametrosSistema:NomeCliente"].ToString(), "CASAMENTOS",
                UserAppContext.Current.Usuario.Username, filtro, cabecalho);

            var grpCongr = casam.GroupBy(x => new { x.CongregacaoNome })
                            .Select(g => new { Congregacao = g.Key.CongregacaoNome, Total = g.Count() })
                            .OrderBy(o => o.Congregacao);

            foreach (var itemCongr in grpCongr)
            {
                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 1].Value = $"CONGREGAÇÃO: {itemCongr.Congregacao}";
                linha++;

                var grpCasam = casam.Where(c => c.CongregacaoNome == itemCongr.Congregacao);
                foreach (var itemCasam in grpCasam)
                {
                    workSheet.Cells[linha, 1].Value = itemCasam.PastorNome;
                    workSheet.Cells[linha, 2].Value = itemCasam.DataHora;
                    workSheet.Cells[linha, 3].Value = itemCasam.NoivoNome;
                    workSheet.Cells[linha, 4].Value = itemCasam.PaiNoivoNome;
                    workSheet.Cells[linha, 5].Value = itemCasam.MaeNoivoNome;
                    workSheet.Cells[linha, 6].Value = itemCasam.NoivaNome;
                    workSheet.Cells[linha, 7].Value = itemCasam.PaiNoivaNome;
                    workSheet.Cells[linha, 8].Value = itemCasam.MaeNoivaNome;
                    linha++;
                }

                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 1].Value = $"Congregação: {itemCongr.Congregacao}";

                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 2].Value = $"Total de Casamentos: {itemCongr.Total}";
                linha++;
                linha++;
            }

            for (int i = 1; i <= cabecalho.Count(); i++)
                workSheet.Column(i).AutoFit();

            var stream = new MemoryStream(excel.GetAsByteArray());
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/octet-stream", "xlsx");

        }
        #endregion

        #region Congregações
        public FileStreamResult RelatorioCongregacoes(string congregacao, string formato)
        {
            try
            {
                int.TryParse(congregacao, out int congreg);

                var cong = _relatoriosAppService.Congregacao(congreg, UserAppContext.Current.Usuario.Id);
                if (!cong.Any())
                    throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

                var filtro = "";
                if (congreg > 0)
                    filtro += $"Congregação: {_congregacaoAppService.GetById(congreg, UserAppContext.Current.Usuario.Id).Nome}";
                else
                    filtro += "Todas as Congregações";
                var nomeArquivo = $"RelatorioCongregacoes_{congreg}";

                if (formato == "Excel")
                    return RelatorioCongregacoesExcel(cong, filtro, nomeArquivo);

                return RelatorioCongregacoesPDF(cong, filtro, nomeArquivo);
            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        private FileStreamResult RelatorioCongregacoesPDF(IEnumerable<Congregacoes> congr,
                                                          string filtro,
                                                          string nomeArquivo)
        {
            var document = new PdfDocument();
            document.Info.Title = "Relatório de Congregações";
            document.Info.Author = "Architect Systems";
            document.Info.Subject = "Congregações";
            document.Info.Keywords = "PDFsharp, XGraphics";

            PdfPage page = document.AddPage();
            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);

            int comecaDados = 10;
            var contaPagina = 1;

            var nomecliente = _configuration["ParametrosSistema:NomeCliente"].ToString();

            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "Congregações", UserAppContext.Current.Usuario.Username,
                comecaDados, filtro, new Dictionary<string, int>());

            foreach (var item in congr)
            {
                var font = new XFont("Arial", 8, XFontStyle.Regular);
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Congregação", $"{item.Id} - {item.Congregacao}");
                comecaDados += 12;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);

                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "Congregações", UserAppContext.Current.Usuario.Username,
                                       comecaDados, filtro, new Dictionary<string, int>());
                }
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "CNPJ", item.CNPJ);
                ImprimirLinhaDescrCont(gfx, 200, comecaDados + 70, "Dirigente", item.Dirigente);
                comecaDados += 12;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);

                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "Congregações", UserAppContext.Current.Usuario.Username,
                                       comecaDados, filtro, new Dictionary<string, int>());
                }
                var endereco = item.Logradouro;
                if (!string.IsNullOrWhiteSpace(item.Numero))
                    endereco += $", {item.Numero}";
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Logradouro", endereco);
                ImprimirLinhaDescrCont(gfx, 350, comecaDados + 70, "Bairro", item.Bairro);
                comecaDados += 12;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);

                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "Congregações", UserAppContext.Current.Usuario.Username,
                                       comecaDados, filtro, new Dictionary<string, int>());
                }
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Cidade", item.Cidade);
                ImprimirLinhaDescrCont(gfx, 300, comecaDados + 70, "Estado", item.Estado);
                ImprimirLinhaDescrCont(gfx, 400, comecaDados + 70, "País", item.Pais);
                comecaDados += 12;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);

                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "Congregações", UserAppContext.Current.Usuario.Username,
                                       comecaDados, filtro, new Dictionary<string, int>());
                }
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Qtd. de Membros Ativos", item.QtdMembrosAtivos.ToString());
                ImprimirLinhaDescrCont(gfx, 350, comecaDados + 70, "Qtd. de Obreiros", item.QtdObreiros.ToString());
                comecaDados += 24;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);

                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "Congregações", UserAppContext.Current.Usuario.Username,
                                       comecaDados, filtro, new Dictionary<string, int>());
                }
            }
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/pdf");
        }

        private FileStreamResult RelatorioCongregacoesExcel(IEnumerable<Congregacoes> congr,
                                                            string filtro,
                                                            string nomeArquivo)
        {
            if (!congr.Any())
                throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("Congregações");

            Dictionary<string, int> cabecalho = new Dictionary<string, int>
                    {
                        { "Código", 1 },
                        { "Congregação", 2 },
                        { "CNPJ", 3 },
                        { "Dirigente", 4 },
                        { "Logradouro", 5 },
                        { "Bairro", 6 },
                        { "Cidade", 7 },
                        { "Estado", 8 },
                        { "País", 9 },
                        { "Qtd.de Membros Ativos", 10 },
                        { "Qtd.de Obreiros", 11 },
                    };

            int linha = RelatorioCabecalhoExcel(workSheet, _configuration["ParametrosSistema:NomeCliente"].ToString(), "CONGREGAÇÕES",
                UserAppContext.Current.Usuario.Username, filtro, cabecalho);

            foreach (var item in congr)
            {
                workSheet.Cells[linha, 1].Value = item.Id;
                workSheet.Cells[linha, 2].Value = item.Congregacao;
                workSheet.Cells[linha, 3].Value = item.CNPJ;
                workSheet.Cells[linha, 4].Value = item.Dirigente;
                workSheet.Cells[linha, 5].Value = item.Logradouro;
                workSheet.Cells[linha, 6].Value = item.Bairro;
                workSheet.Cells[linha, 7].Value = item.Cidade;
                workSheet.Cells[linha, 8].Value = item.Estado;
                workSheet.Cells[linha, 9].Value = item.Pais;
                workSheet.Cells[linha, 10].Value = item.QtdMembrosAtivos;
                workSheet.Cells[linha, 11].Value = item.QtdObreiros;
                linha++;
            }
            for (int i = 1; i <= cabecalho.Count(); i++)
                workSheet.Column(i).AutoFit();

            var stream = new MemoryStream(excel.GetAsByteArray());
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/octet-stream", "xlsx");
        }
        #endregion

        #region Transferências
        public FileStreamResult RelatorioTransferencia(string congregacao,
                                                       string dataInicio,
                                                       string dataFinal,
                                                       string formato)
        {
            try
            {
                DateTimeOffset.TryParse(dataInicio, out DateTimeOffset dtInicio);
                DateTimeOffset.TryParse(dataFinal, out DateTimeOffset dtFinal);
                int.TryParse(congregacao, out int congr);
                var trans = _relatoriosAppService.Transferencia(dtInicio, dtFinal, congr, UserAppContext.Current.Usuario.Id);
                if (!trans.Any())
                    throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

                var filtro = $"Data Inicio: {dtInicio.Date.ToShortDateString()} - Data Final: {dtFinal.Date.ToShortDateString()}";
                if (congr > 0)
                    filtro += $" - Congregação: {_congregacaoAppService.GetById(congr, UserAppContext.Current.Usuario.Id).Nome}";
                else
                    filtro += " - Todas as Congregações";
                var nomeArquivo = $"Transferencias_{dtInicio.Date.ToShortDateString().Replace("/", "")}_{dtFinal.Date.ToShortDateString().Replace("/", "")}";

                if (formato == "Excel")
                    return RelatorioTransferenciaExcel(trans, filtro, nomeArquivo);
                return RelatorioTransferenciaPDF(trans, filtro, nomeArquivo);

            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        private FileStreamResult RelatorioTransferenciaPDF(IEnumerable<Transferencia> trans,
                                                           string filtro,
                                                           string nomeArquivo)
        {
            var document = new PdfDocument();
            document.Info.Title = "Relatório de Tranferências";
            document.Info.Author = "Architect Systems";
            document.Info.Subject = "Transferências";
            document.Info.Keywords = "PDFsharp, XGraphics";

            PdfPage page = document.AddPage();
            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);

            int comecaDados = 10;
            var contaPagina = 1;
            Dictionary<string, int> cabecalho = new Dictionary<string, int>
                    {
                        { "DATA", 25 },
                        { "TIPO", 70 },
                        { "CONGREGAÇÃO ORIGEM", 130 },
                        { "CONGREGAÇÃO DESTINO", 250 },
                        { "MEMBRO", 380 },
                        { "STATUS", 525 },
                    };

            var nomecliente = _configuration["ParametrosSistema:NomeCliente"].ToString();

            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "TRANSFERÊNCIAS", UserAppContext.Current.Usuario.Username,
                comecaDados, filtro, cabecalho);

            foreach (var item in trans.OrderBy(t => t.DataDaTransferencia))
            {
                var qtdLinhasPular = new List<int>();

                var font = new XFont("Arial", 8, XFontStyle.Regular);
                gfx.DrawString(item.DataDaTransferencia.Date.ToShortDateString(), font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                gfx.DrawString(item.TipoCarta, font, XBrushes.Black, new XRect(70, comecaDados + 80, 0, 0));
                qtdLinhasPular.Add(ImprimirLinha(gfx, item.CongregacaoOrigem, font, 130, 249, comecaDados + 80));
                qtdLinhasPular.Add(ImprimirLinha(gfx, item.CongregacaoDestino, font, 250, 379, comecaDados + 80));
                qtdLinhasPular.Add(ImprimirLinha(gfx, item.Nome, font, 380, 529, comecaDados + 80));
                qtdLinhasPular.Add(ImprimirLinha(gfx, item.StatusCarta, font, 525, 580, comecaDados + 80));
                comecaDados += (12 * qtdLinhasPular.Max());
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "TRANSFERÊNCIAS", UserAppContext.Current.Usuario.Username,
                                       comecaDados, filtro, cabecalho);
                }
            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/pdf");
        }

        private FileStreamResult RelatorioTransferenciaExcel(IEnumerable<Transferencia> trans,
                                                             string filtro,
                                                             string nomeArquivo)
        {
            if (!trans.Any())
                throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("Transferências");

            Dictionary<string, int> cabecalho = new Dictionary<string, int>
                    {
                        { "DATA", 1 },
                        { "TIPO", 2 },
                        { "CONGREGAÇÃO ORIGEM", 3 },
                        { "CONGREGAÇÃO DESTINO", 4 },
                        { "MEMBRO", 5 },
                        { "STATUS", 6 },
                    };

            int linha = RelatorioCabecalhoExcel(workSheet, _configuration["ParametrosSistema:NomeCliente"].ToString(), "TRANSFERÊNCIAS",
                UserAppContext.Current.Usuario.Username, filtro, cabecalho);


            foreach (var item in trans.OrderBy(t => t.DataDaTransferencia))
            {
                workSheet.Cells[linha, 1].Value = item.DataDaTransferencia.Date.ToShortDateString();
                workSheet.Cells[linha, 2].Value = item.TipoCarta;
                workSheet.Cells[linha, 3].Value = item.CongregacaoOrigem;
                workSheet.Cells[linha, 4].Value = item.CongregacaoDestino;
                workSheet.Cells[linha, 5].Value = item.Nome;
                workSheet.Cells[linha, 6].Value = item.StatusCarta;
                linha++;
            }

            for (int i = 1; i <= cabecalho.Count(); i++)
                workSheet.Column(i).AutoFit();

            var stream = new MemoryStream(excel.GetAsByteArray());
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/octet-stream", "xlsx");
        }
        #endregion

        #region Nascimentos
        public FileStreamResult RelatorioNascimentos(string congregacao,
                                                     string dataInicio,
                                                     string dataFinal,
                                                     string formato)
        {
            try
            {
                DateTimeOffset.TryParse(dataInicio, out DateTimeOffset dtInicio);
                DateTimeOffset.TryParse(dataFinal, out DateTimeOffset dtFinal);
                int.TryParse(congregacao, out int congr);
                var nasc = _relatoriosAppService.Nascimento(dtInicio, dtFinal, congr, UserAppContext.Current.Usuario.Id);
                if (!nasc.Any())
                    throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

                var filtro = $"Data Inicio: {dtInicio.Date.ToShortDateString()} - Data Final: {dtFinal.Date.ToShortDateString()}";
                if (congr > 0)
                    filtro += $" - Congregação: {_congregacaoAppService.GetById(congr, UserAppContext.Current.Usuario.Id).Nome}";
                else
                    filtro += " - Todas as Congregações";

                var nomeArquivo = $"Nascimentos_{dtInicio.Date.ToShortDateString().Replace("/", "")}_{dtFinal.Date.ToShortDateString().Replace("/", "")}";

                if (formato == "Excel")
                    return RelatorioNascimentosExcel(nasc, filtro, nomeArquivo);
                return RelatorioNascimentosPDF(nasc, filtro, nomeArquivo);
            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        private FileStreamResult RelatorioNascimentosPDF(IEnumerable<Nascimentos> nasc,
                                                         string filtro,
                                                         string nomeArquivo)
        {
            int comecaDados = 10;
            var contaPagina = 1;

            var document = new PdfDocument();
            document.Info.Title = "Relatório de Nascimentos";
            document.Info.Author = "Architect Systems";
            document.Info.Subject = "Nascimentos";
            document.Info.Keywords = "PDFsharp, XGraphics";

            PdfPage page = document.AddPage();
            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);

            var grpData = nasc.GroupBy(x => x.DataApresentacao.Date)
                        .Select(g => new { DataApresentacao = g.Key, Total = g.Count() })
                        .OrderBy(o => o.DataApresentacao);

            var nomecliente = _configuration["ParametrosSistema:NomeCliente"].ToString();
            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "NASCIMENTOS", UserAppContext.Current.Usuario.Username,
                comecaDados, filtro, new Dictionary<string, int>());
            foreach (var grp in grpData)
            {
                var font = new XFont("Arial", 10, XFontStyle.Bold);
                gfx.DrawString($"DATA APRESENTAÇÃO: {grp.DataApresentacao.ToShortDateString()}", font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                comecaDados += 12;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"NASCIMENTOS", UserAppContext.Current.Usuario.Username,
                                                     comecaDados, filtro, new Dictionary<string, int>());
                }

                var memCongr = nasc.Where(o => o.DataApresentacao.Date == grp.DataApresentacao).OrderBy(o => o.Crianca).ToList();

                font = new XFont("Arial", 10, XFontStyle.Regular);
                foreach (var item in memCongr)
                {
                    comecaDados += 12;
                    ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Congregação", item.CongregacaoNome);
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "NASCIMENTOS", UserAppContext.Current.Usuario.Username,
                            comecaDados, filtro, new Dictionary<string, int>());
                    }

                    ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Pastor", item.Pastor);
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "NASCIMENTOS", UserAppContext.Current.Usuario.Username,
                            comecaDados, filtro, new Dictionary<string, int>());
                    }

                    ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Criança", item.Crianca);
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "NASCIMENTOS", UserAppContext.Current.Usuario.Username,
                            comecaDados, filtro, new Dictionary<string, int>());
                    }

                    ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Data Nascimento", item.DataNascimento.Date.ToShortDateString());
                    ImprimirLinhaDescrCont(gfx, 300, comecaDados + 70, "Sexo", item.Sexo);
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - COMPLETO", UserAppContext.Current.Usuario.Username,
                                                         comecaDados, filtro, new Dictionary<string, int>());
                    }
                    ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Pai", item.NomePai);
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "NASCIMENTOS", UserAppContext.Current.Usuario.Username,
                            comecaDados, filtro, new Dictionary<string, int>());
                    }

                    ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Mãe", item.NomeMae);
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "NASCIMENTOS", UserAppContext.Current.Usuario.Username,
                                                        comecaDados, filtro, new Dictionary<string, int>());
                    }
                }
                comecaDados += 12;
                font = new XFont("Arial", 10, XFontStyle.Bold);
                gfx.DrawString($"DATA APRESENTAÇÃO: {grp.DataApresentacao.ToShortDateString()} - TOTAL: {grp.Total}", font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                comecaDados += 24;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - COMPLETO", UserAppContext.Current.Usuario.Username,
                                                     comecaDados, filtro, new Dictionary<string, int>());
                }
            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/pdf");
        }

        private FileStreamResult RelatorioNascimentosExcel(IEnumerable<Nascimentos> nasc,
                                                           string filtro,
                                                           string nomeArquivo)
        {
            if (!nasc.Any())
                throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("Nascimentos");

            Dictionary<string, int> cabecalho = new Dictionary<string, int>()
                {
                    { "Congregação", 1 },
                    { "Pastor", 2 },
                    { "Criança", 3 },
                    { "Data Nascimento", 4 },
                    { "Sexo", 5 },
                    { "Pai", 6 },
                    { "Mãe", 7 }
                };

            int linha = RelatorioCabecalhoExcel(workSheet, _configuration["ParametrosSistema:NomeCliente"].ToString(), "NASCIMENTOS",
                UserAppContext.Current.Usuario.Username, filtro, cabecalho);

            var grpData = nasc.GroupBy(x => x.DataApresentacao.Date)
                        .Select(g => new { DataApresentacao = g.Key, Total = g.Count() })
                        .OrderBy(o => o.DataApresentacao);

            foreach (var grp in grpData)
            {
                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 1].Value = $"DATA APRESENTAÇÃO: {grp.DataApresentacao.ToShortDateString()}";
                linha++;

                var memCongr = nasc.Where(o => o.DataApresentacao.Date == grp.DataApresentacao).OrderBy(o => o.Crianca).ToList();


                foreach (var item in memCongr)
                {
                    workSheet.Cells[linha, 1].Value = item.CongregacaoNome;
                    workSheet.Cells[linha, 2].Value = item.Pastor;
                    workSheet.Cells[linha, 3].Value = item.Crianca;
                    workSheet.Cells[linha, 4].Value = item.DataNascimento.Date.ToShortDateString();
                    workSheet.Cells[linha, 5].Value = item.Sexo;
                    workSheet.Cells[linha, 6].Value = item.NomePai;
                    workSheet.Cells[linha, 7].Value = item.NomeMae;
                    linha++;
                }

                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 1].Value = $"DATA APRESENTAÇÃO: {grp.DataApresentacao.ToShortDateString()}";

                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 2].Value = $"TOTAL: {grp.Total}";

                linha++;
                linha++;
            }

            for (int i = 1; i <= cabecalho.Count(); i++)
                workSheet.Column(i).AutoFit();

            var stream = new MemoryStream(excel.GetAsByteArray());
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/octet-stream", "xlsx");
        }
        #endregion

        #region Obreiros
        public FileStreamResult RelatorioObreiros(string congregacao,
                                                  string formato)
        {
            try
            {
                int.TryParse(congregacao, out int congreg);
                var obr = _relatoriosAppService.Obreiros(congreg, UserAppContext.Current.Usuario.Id);

                var filtro = "";
                if (congreg > 0)
                    filtro += $"Congregação: {_congregacaoAppService.GetById(congreg, UserAppContext.Current.Usuario.Id).Nome}";
                else
                    filtro += "Todas as Congregações";

                var nomeArquivo = $"RelatorioObreiros{congreg}";
                if (congreg == 0)
                    nomeArquivo = "RelatorioObreiros_Todos";

                if (formato == "Excel")
                    return RelatorioObreirosExcel(obr, filtro, nomeArquivo);
                return RelatorioObreirosPDF(obr, filtro, nomeArquivo);
            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        private FileStreamResult RelatorioObreirosPDF(IEnumerable<Obreiros> obreiros,
                                                      string filtro,
                                                      string nomeArquivo)
        {
            if (!obreiros.Any())
                throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

            var document = new PdfDocument();
            document.Info.Title = "Relatório de Obreiros";
            document.Info.Author = "Architect Systems";
            document.Info.Subject = "Relatório de Obreiros";
            document.Info.Keywords = "PDFsharp, XGraphics";

            PdfPage page = document.AddPage();
            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);

            int comecaDados = 10;
            var contaPagina = 1;
            Dictionary<string, int> cabecalho = new Dictionary<string, int>();

            var nomecliente = _configuration["ParametrosSistema:NomeCliente"].ToString();

            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "OBREIROS", UserAppContext.Current.Usuario.Username,
                comecaDados, filtro, cabecalho);

            var grpCongr = obreiros.GroupBy(x => new { x.CongregacaoId, x.PastorResponsavel, x.CongregacaoNome })
                            .Select(g => new { g.Key.CongregacaoId, g.Key.PastorResponsavel, Congregacao = g.Key.CongregacaoNome, Total = g.Count() })
                            .OrderBy(o => o.CongregacaoId);

            foreach (var grp in grpCongr)
            {
                var font = new XFont("Arial", 10, XFontStyle.Bold);
                gfx.DrawString($"CONGREGAÇÃO: {grp.Congregacao}", font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                comecaDados += 12;
                gfx.DrawString($"PASTOR RESP.: {grp.PastorResponsavel}", font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                comecaDados += 13;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "OBREIROS", UserAppContext.Current.Usuario.Username,
                                       comecaDados, filtro, cabecalho);
                }

                gfx.DrawString("Membro:", font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                gfx.DrawString("Data Nasc.:", font, XBrushes.Black, new XRect(360, comecaDados + 70, 0, 0));
                gfx.DrawString("Cargo:", font, XBrushes.Black, new XRect(430, comecaDados + 70, 0, 0));
                comecaDados += 12;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "OBREIROS", UserAppContext.Current.Usuario.Username,
                                       comecaDados, filtro, cabecalho);
                }
                var obrCongr = obreiros.Where(o => o.CongregacaoId == grp.CongregacaoId)
                                  .OrderBy(o => o.Descricao).ToList();
                if (obrCongr.Count() > 0)
                {
                    font = new XFont("Arial", 10, XFontStyle.Regular);
                    foreach (var item in obrCongr)
                    {
                        gfx.DrawString(item.NomeMembro, font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                        gfx.DrawString(item.DataNascimento.Date.ToShortDateString(), font, XBrushes.Black, new XRect(360, comecaDados + 70, 0, 0));
                        gfx.DrawString(item.Descricao, font, XBrushes.Black, new XRect(430, comecaDados + 70, 0, 0));
                        comecaDados += 12;

                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "OBREIROS", UserAppContext.Current.Usuario.Username,
                                               comecaDados, filtro, cabecalho);
                        }
                    }
                }
                else
                {
                    font = new XFont("Arial", 10, XFontStyle.Regular);
                    gfx.DrawString("Não existem Obreiros cadastrados para a Congregação", font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    comecaDados += 12;
                }

                font = new XFont("Arial", 10, XFontStyle.Bold);
                gfx.DrawString($"CONGREGAÇÃO: {grp.Congregacao} - TOTAL: {grp.Total}", font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                comecaDados += 24;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "OBREIROS", UserAppContext.Current.Usuario.Username,
                                       comecaDados, filtro, cabecalho);
                }
            }
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/pdf");

        }

        private FileStreamResult RelatorioObreirosExcel(IEnumerable<Obreiros> obreiros,
                                                        string filtro,
                                                        string nomeArquivo)
        {
            if (!obreiros.Any())
                throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("Obreiros");

            int linha = RelatorioCabecalhoExcel(workSheet, _configuration["ParametrosSistema:NomeCliente"].ToString(), "OBREIROS",
                UserAppContext.Current.Usuario.Username, filtro, null);

            var grpCongr = obreiros.GroupBy(x => new { x.CongregacaoId, x.PastorResponsavel, x.CongregacaoNome })
                            .Select(g => new { g.Key.CongregacaoId, g.Key.PastorResponsavel, Congregacao = g.Key.CongregacaoNome, Total = g.Count() })
                            .OrderBy(o => o.CongregacaoId);
            linha++;
            foreach (var grp in grpCongr)
            {
                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 1].Value = $"CONGREGAÇÃO: {grp.Congregacao}";
                linha++;

                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 1].Value = $"PASTOR RESP.: {grp.PastorResponsavel}";
                linha++;

                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 1].Value = "Membro:";
                workSheet.Cells[linha, 2].Value = "Data Nasc.:";
                workSheet.Cells[linha, 3].Value = "Cargo:";
                linha++;

                var obrCongr = obreiros.Where(o => o.CongregacaoId == grp.CongregacaoId)
                                  .OrderBy(o => o.Descricao).ToList();
                if (obrCongr.Count() > 0)
                {
                    foreach (var item in obrCongr)
                    {
                        workSheet.Cells[linha, 1].Value = item.NomeMembro;
                        workSheet.Cells[linha, 2].Value = item.DataNascimento.Date.ToShortDateString();
                        workSheet.Cells[linha, 3].Value = item.Descricao;
                        linha++;
                    }
                }
                else
                {
                    workSheet.Cells[linha, 1].Value = "Não existem Obreiros cadastrados para a Congregação";
                    linha++;
                }
                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 1].Value = $"CONGREGAÇÃO: {grp.Congregacao} - TOTAL: {grp.Total}";
                linha++;
                linha++;
            }
            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();
            workSheet.Column(3).AutoFit();

            var stream = new MemoryStream(excel.GetAsByteArray());
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/octet-stream", "xlsx");

        }
        #endregion

        #region Cursos Membro
        public FileStreamResult RelatorioCursosMembro(string congregacaoId,
                                                      string cursoId,
                                                      string formato)
        {
            try
            {
                int.TryParse(congregacaoId, out int congr);
                int.TryParse(cursoId, out int curId);

                var mem = _relatoriosAppService.CursosMembro(congr, curId, UserAppContext.Current.Usuario.Id);
                if (!mem.Any())
                    throw new Erro("Curso não encontrado.");

                var filtro = "";
                if (congr > 0)
                    filtro += $"Congregação: {_congregacaoAppService.GetById(congr, UserAppContext.Current.Usuario.Id).Nome}";
                else
                    filtro += $"Congregação: Todas";
                if (curId > 0)
                    filtro += string.IsNullOrEmpty(filtro) ? "" : " - " + $"Cursos: {_cursoService.GetById(curId, UserAppContext.Current.Usuario.Id).Descricao}";
                else
                    filtro += string.IsNullOrEmpty(filtro) ? "" : " - " + $"Cursos: Todos";
                var nomeArquivo = $"RelatorioCursosMembro_{curId}";

                if (formato == "Excel")
                    return RelatorioCursosMembroExcel(mem, filtro, nomeArquivo);

                return RelatorioCursosMembroPDF(mem, filtro, nomeArquivo);
            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        private FileStreamResult RelatorioCursosMembroPDF(IEnumerable<CursoMembro> mem,
                                                          string filtro,
                                                          string nomeArquivo)
        {
            var document = new PdfDocument();
            document.Info.Title = "Relatório de Membros - Curso";
            document.Info.Author = "Architect Systems";
            document.Info.Subject = "Membros - Curso";
            document.Info.Keywords = "PDFsharp, XGraphics";

            PdfPage page = document.AddPage();
            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);

            int comecaDados = 10;
            var contaPagina = 1;
            Dictionary<string, int> cabecalho = new Dictionary<string, int>();

            var nomecliente = _configuration["ParametrosSistema:NomeCliente"].ToString();

            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "MEMBROS - CURSOS", UserAppContext.Current.Usuario.Username,
                comecaDados, filtro, cabecalho);

            var grpCursos = mem.GroupBy(x => new { x.Curso })
                           .Select(g => new { g.Key.Curso, Total = g.Count() });

            foreach (var grp in grpCursos)
            {

                var font = new XFont("Arial", 8, XFontStyle.Bold);
                gfx.DrawString($"CURSO: {grp.Curso}", font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                comecaDados += 12;
                gfx.DrawString("MEMBRO", font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                gfx.DrawString("CONGREGAÇÃO", font, XBrushes.Black, new XRect(300, comecaDados + 70, 0, 0));
                comecaDados += 12;

                var membrosCurso = mem.Where(o => o.Curso == grp.Curso)
                                  .OrderBy(o => o.Congregacao).ToList();
                if (membrosCurso.Count() > 0)
                {
                    font = new XFont("Arial", 8, XFontStyle.Regular);
                    foreach (var item in membrosCurso)
                    {
                        gfx.DrawString(item.Membro, font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                        gfx.DrawString(item.Congregacao, font, XBrushes.Black, new XRect(300, comecaDados + 70, 0, 0));
                        comecaDados += 12;

                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "MEMBROS - CURSOS", UserAppContext.Current.Usuario.Username,
                                               comecaDados, filtro, cabecalho);
                        }
                    }
                }
                else
                {
                    font = new XFont("Arial", 8, XFontStyle.Regular);
                    gfx.DrawString("Não existem Membros cadastrados para o Curso", font, XBrushes.Black, new XRect(360, comecaDados + 70, 0, 0));
                    comecaDados += 12;
                }

                font = new XFont("Arial", 8, XFontStyle.Bold);
                gfx.DrawString($"CURSO: {grp.Curso} - TOTAL: {grp.Total}", font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                comecaDados += 24;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "MEMBROS - CURSOS", UserAppContext.Current.Usuario.Username,
                                       comecaDados, filtro, cabecalho);
                }

            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/pdf");
        }

        private FileStreamResult RelatorioCursosMembroExcel(IEnumerable<CursoMembro> mem,
                                                            string filtro,
                                                            string nomeArquivo)
        {
            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("Cursos");

            Dictionary<string, int> cabecalho = new Dictionary<string, int>()
                {
                    { "Membro", 1 },
                    { "Congregação", 2 },
                };

            int linha = RelatorioCabecalhoExcel(workSheet, _configuration["ParametrosSistema:NomeCliente"].ToString(), "MEMBROS - CURSOS",
                UserAppContext.Current.Usuario.Username, filtro, cabecalho);

            var grpCursos = mem.GroupBy(x => new { x.Curso })
                           .Select(g => new { g.Key.Curso, Total = g.Count() });

            foreach (var grp in grpCursos)
            {
                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 1].Value = $"CURSO: {grp.Curso}";
                linha++;


                var membrosCurso = mem.Where(o => o.Curso == grp.Curso)
                                  .OrderBy(o => o.Congregacao).ToList();
                if (membrosCurso.Count() > 0)
                {

                    foreach (var item in membrosCurso)
                    {
                        workSheet.Cells[linha, 1].Value = item.Membro;
                        workSheet.Cells[linha, 2].Value = item.Congregacao;
                        linha++;
                    }
                }
                else
                {
                    workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Row(linha).Style.Font.Bold = true;
                    workSheet.Cells[linha, 1].Value = "Não existem Membros cadastrados para o Curso";
                    linha++;
                }

                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 1].Value = $"CURSO: {grp.Curso}";
                workSheet.Cells[linha, 2].Value = $"TOTAL: {grp.Total}";
                linha++;
                linha++;

            }
            for (int i = 1; i <= cabecalho.Count(); i++)
                workSheet.Column(i).AutoFit();

            var stream = new MemoryStream(excel.GetAsByteArray());
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/octet-stream", "xlsx");
        }
        #endregion

        #region Membros
        public FileStreamResult RelatorioMembros(string congregacaoId,
                                                 string status,
                                                 string tipoMembro,
                                                 string estadoCivil,
                                                 bool simplificado,
                                                 bool abedabe,
                                                 bool filtrarConf = false,
                                                 bool ativosConf = false,
                                                 string formato = "")
        {
            try
            {
                int.TryParse(congregacaoId, out int congrId);
                Enum.TryParse(status, out Status statusMem);
                Enum.TryParse(tipoMembro, out TipoMembro tpMembro);
                Enum.TryParse(estadoCivil, out EstadoCivil estCivil);

                var mem = _relatoriosAppService.RelatorioMembros(congrId, statusMem, tpMembro,
                    estCivil, abedabe, filtrarConf, ativosConf, UserAppContext.Current.Usuario.Id);
                if (!mem.Any())
                    throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

                var filtro = "";
                if (congrId > 0)
                {
                    var congrNome = _congregacaoAppService.GetById(congrId, UserAppContext.Current.Usuario.Id).Nome;
                    if (congrNome.Length > 20)
                        congrNome = $"{congrNome.Substring(0, 20)}...";
                    filtro += $"Congregação: {congrNome}";
                }

                else
                    filtro += "Todas as Congregações";

                if (tpMembro != TipoMembro.NaoDefinido)
                    filtro += $" - Tipo Membro: {tpMembro.GetDisplayAttributeValue()}";
                else
                    filtro += " - Tipo Membro: Todos";

                if (statusMem != Status.NaoDefinido)
                    filtro += $" - Status: {statusMem.GetDisplayAttributeValue()}";
                else
                    filtro += " - Status: Todos";

                if (estCivil != EstadoCivil.NaoDefinido)
                    filtro += $" - Estado Civil: {estCivil.GetDisplayAttributeValue()}";
                else
                    filtro += " - Estado Civil: Todos";

                if (abedabe)
                    filtro += $" - Associados ABEDABE - Sim";

                if (filtrarConf)
                    filtro += $" - Confirmados - {(ativosConf ? "Sim" : "Não")}";

                var nomeArquivo = $"RelatorioMembrosCompl_{congregacaoId}";
                if (simplificado)
                    nomeArquivo = $"RelatorioMembrosSimpl_{congregacaoId}";

                if (formato == "Excel")
                    return RelatorioMembrosExcel(mem, filtro, nomeArquivo, simplificado);

                return RelatorioMembrosPDF(mem, filtro, nomeArquivo, simplificado);
            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        private FileStreamResult RelatorioMembrosPDF(IEnumerable<RelatorioMembros> membro,
                                                     string filtro,
                                                     string nomeArquivo,
                                                     bool simplificado)
        {
            var document = new PdfDocument();
            document.Info.Title = "Relatório de Membros";
            document.Info.Author = "Architect Systems";
            document.Info.Subject = "Membros";
            document.Info.Keywords = "PDFsharp, XGraphics";

            PdfPage page = document.AddPage();
            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);

            int comecaDados = 10;
            var contaPagina = 1;

            Dictionary<string, int> cabecalho = new Dictionary<string, int>
                    {
                        { "MEMBRO.", 25 },
                        { "DT.NASC.", 300 },
                        { "STATUS", 355 },
                        { "SITUACAO", 470 }
                    };

            var nomecliente = _configuration["ParametrosSistema:NomeCliente"].ToString();

            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - {(!simplificado ? "COMPLETO" : "SIMPLIFICADO")}", UserAppContext.Current.Usuario.Username,
                                             comecaDados, filtro, !simplificado ? new Dictionary<string, int>() : cabecalho);

            var grpCongr = membro.GroupBy(x => new { x.CongregacaoNome })
                         .Select(g => new { Congregacao = g.Key.CongregacaoNome, Total = g.Count() })
                         .OrderBy(o => o.Congregacao);
            if (simplificado)
            {
                foreach (var grp in grpCongr)
                {
                    var font = new XFont("Arial", 10, XFontStyle.Bold);
                    gfx.DrawString($"CONGREGAÇÃO: {grp.Congregacao}", font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - SIMPLIFICADO", UserAppContext.Current.Usuario.Username,
                                                         comecaDados, filtro, cabecalho);
                    }

                    var memCongr = membro.Where(o => o.CongregacaoNome == grp.Congregacao)
                                      .OrderBy(o => o.MembroNome).ToList();

                    font = new XFont("Arial", 10, XFontStyle.Regular);
                    foreach (var item in memCongr)
                    {
                        gfx.DrawString(item.Membro, font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                        gfx.DrawString(item.DataNascimento, font, XBrushes.Black, new XRect(300, comecaDados + 80, 0, 0));
                        gfx.DrawString(item.Status, font, XBrushes.Black, new XRect(355, comecaDados + 80, 0, 0));
                        gfx.DrawString(item.Situacao, font, XBrushes.Black, new XRect(470, comecaDados + 80, 0, 0));
                        comecaDados += 12;

                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - SIMPLIFICADO", UserAppContext.Current.Usuario.Username,
                                                             comecaDados, filtro, cabecalho);
                        }
                    }

                    font = new XFont("Arial", 10, XFontStyle.Bold);
                    gfx.DrawString($"CONGREGAÇÃO: {grp.Congregacao} - TOTAL: {grp.Total}", font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                    comecaDados += 24;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - SIMPLIFICADO", UserAppContext.Current.Usuario.Username,
                                                         comecaDados, filtro, cabecalho);
                    }
                }
            }
            else
            {
                foreach (var grp in grpCongr)
                {
                    var font = new XFont("Arial", 10, XFontStyle.Bold);
                    gfx.DrawString($"CONGREGAÇÃO: {grp.Congregacao}", font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - COMPLETO", UserAppContext.Current.Usuario.Username,
                                                         comecaDados, filtro, new Dictionary<string, int>());
                    }

                    var memCongr = membro.Where(o => o.CongregacaoNome == grp.Congregacao)
                                      .OrderBy(o => o.MembroNome).ToList();

                    font = new XFont("Arial", 10, XFontStyle.Regular);
                    foreach (var item in memCongr)
                    {
                        comecaDados += 12;
                        ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Status", item.Status);
                        ImprimirLinhaDescrCont(gfx, 150, comecaDados + 70, "Tipo", item.TipoMembro);
                        ImprimirLinhaDescrCont(gfx, 400, comecaDados + 70, "Situação", item.Situacao);
                        comecaDados += 12;
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - COMPLETO", UserAppContext.Current.Usuario.Username,
                                                             comecaDados, filtro, new Dictionary<string, int>());
                        }

                        ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Membro", item.Membro);
                        comecaDados += 12;
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - COMPLETO", UserAppContext.Current.Usuario.Username,
                                                             comecaDados, filtro, new Dictionary<string, int>());
                        }

                        ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Pai", item.NomePai);
                        comecaDados += 12;
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - COMPLETO", UserAppContext.Current.Usuario.Username,
                                                             comecaDados, filtro, new Dictionary<string, int>());
                        }

                        ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Mãe", item.NomeMae);
                        comecaDados += 12;
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - COMPLETO", UserAppContext.Current.Usuario.Username,
                                                             comecaDados, filtro, new Dictionary<string, int>());
                        }

                        ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Data Nascimento", item.DataNascimento);
                        comecaDados += 12;
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - COMPLETO", UserAppContext.Current.Usuario.Username,
                                                             comecaDados, filtro, new Dictionary<string, int>());
                        }

                        ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Natural", item.Natural);
                        comecaDados += 12;
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - COMPLETO", UserAppContext.Current.Usuario.Username,
                                                             comecaDados, filtro, new Dictionary<string, int>());
                        }

                        ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Telefones", item.Telefones);
                        ImprimirLinhaDescrCont(gfx, 400, comecaDados + 70, "ABEDABE", item.MembroAbedabe);
                        comecaDados += 12;
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - COMPLETO", UserAppContext.Current.Usuario.Username,
                                                             comecaDados, filtro, new Dictionary<string, int>());
                        }
                    }
                    comecaDados += 12;
                    font = new XFont("Arial", 10, XFontStyle.Bold);
                    gfx.DrawString($"CONGREGAÇÃO: {grp.Congregacao} - TOTAL: {grp.Total}", font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    comecaDados += 24;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"MEMBROS - COMPLETO", UserAppContext.Current.Usuario.Username,
                                                         comecaDados, filtro, new Dictionary<string, int>());
                    }
                }
            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/pdf");
        }

        private FileStreamResult RelatorioMembrosExcel(IEnumerable<RelatorioMembros> membro,
                                                       string filtro,
                                                       string nomeArquivo,
                                                       bool simplificado)
        {
            if (!membro.Any())
                throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("Membros");

            Dictionary<string, int> cabecalhoSimp = new Dictionary<string, int>
                    {
                        { "MEMBRO.", 1 },
                        { "DT.NASC.", 2 },
                        { "STATUS", 3 },
                        { "SITUACAO", 4 }
                    };

            Dictionary<string, int> cabecalhoComp = new Dictionary<string, int>
                    {
                        {"STATUS", 1},
                        {"TIPO", 2},
                        {"SITUAÇÃO", 3},
                        {"MEMBRO", 4},
                        {"PAI", 5},
                        {"MÃE", 6},
                        {"DATA NASCIMENTO", 7},
                        {"NATURAL", 8},
                        {"TELEFONES", 9},
                        {"ABEDABE", 10}
                    };
            int linha = RelatorioCabecalhoExcel(workSheet, _configuration["ParametrosSistema:NomeCliente"].ToString(), $"MEMBROS - {(simplificado ? "SIMPLIFICADO" : "COMPLETO")}",
                UserAppContext.Current.Usuario.Username, filtro, (simplificado ? cabecalhoSimp : cabecalhoComp));

            var grpCongr = membro.GroupBy(x => new { x.CongregacaoNome })
                         .Select(g => new { Congregacao = g.Key.CongregacaoNome, Total = g.Count() })
                         .OrderBy(o => o.Congregacao);

            if (simplificado)
            {
                foreach (var grp in grpCongr)
                {
                    workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Row(linha).Style.Font.Bold = true;
                    workSheet.Cells[linha, 1].Value = $"CONGREGAÇÃO: {grp.Congregacao}";
                    linha++;

                    var memCongr = membro.Where(o => o.CongregacaoNome == grp.Congregacao)
                                      .OrderBy(o => o.MembroNome).ToList();

                    foreach (var item in memCongr)
                    {
                        workSheet.Cells[linha, 1].Value = item.Membro;
                        workSheet.Cells[linha, 2].Value = item.DataNascimento;
                        workSheet.Cells[linha, 3].Value = item.Status;
                        workSheet.Cells[linha, 4].Value = item.Situacao;
                        linha++;
                    }

                    workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Row(linha).Style.Font.Bold = true;
                    workSheet.Cells[linha, 1].Value = $"CONGREGAÇÃO: {grp.Congregacao} - TOTAL: {grp.Total}";
                    linha++;
                    linha++;
                }
            }
            else
            {
                foreach (var grp in grpCongr)
                {
                    workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Row(linha).Style.Font.Bold = true;
                    workSheet.Cells[linha, 1].Value = $"CONGREGAÇÃO: {grp.Congregacao}";
                    linha++;


                    var memCongr = membro.Where(o => o.CongregacaoNome == grp.Congregacao)
                                      .OrderBy(o => o.MembroNome).ToList();


                    foreach (var item in memCongr)
                    {
                        workSheet.Cells[linha, 1].Value = item.Status;
                        workSheet.Cells[linha, 2].Value = item.TipoMembro;
                        workSheet.Cells[linha, 3].Value = item.Situacao;
                        workSheet.Cells[linha, 4].Value = item.Membro;
                        workSheet.Cells[linha, 5].Value = item.NomePai;
                        workSheet.Cells[linha, 6].Value = item.NomeMae;
                        workSheet.Cells[linha, 7].Value = item.DataNascimento;
                        workSheet.Cells[linha, 8].Value = item.Natural;
                        workSheet.Cells[linha, 9].Value = item.Telefones;
                        workSheet.Cells[linha, 10].Value = item.MembroAbedabe;
                        linha++;
                    }

                    workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    workSheet.Row(linha).Style.Font.Bold = true;
                    workSheet.Cells[linha, 1].Value = $"CONGREGAÇÃO: {grp.Congregacao} - TOTAL: {grp.Total}";
                    linha++;
                    linha++;
                }
            }

            var qtdColumns = simplificado ? cabecalhoSimp.Count() : cabecalhoComp.Count();
            for (int i = 1; i <= qtdColumns; i++)
                workSheet.Column(i).AutoFit();

            var stream = new MemoryStream(excel.GetAsByteArray());
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/octet-stream", "xlsx");
        }
        #endregion

        public FileStreamResult RelatorioMensal(string congregacao,
                                                string mes,
                                                int ano)
        {
            try
            {
                int.TryParse(congregacao, out int congregacaoId);
                var relMensal = _relatoriosAppService.RelatorioMensal(mes, ano, congregacaoId, UserAppContext.Current.Usuario.Id).FirstOrDefault();
                if (relMensal == null)
                    throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

                var filtro = "";
                var cong = _congregacaoAppService.GetById(congregacaoId, UserAppContext.Current.Usuario.Id);
                filtro += $"Congregação: {cong.Nome}";

                var pastor = "Pr. " + _congregacaoAppService.GetSede().PastorResponsavelNome;

                var document = new PdfDocument();
                document.Info.Title = "Relatório Mensal";
                document.Info.Author = "Architect Systems";
                document.Info.Subject = "Mensal";
                document.Info.Keywords = "PDFsharp, XGraphics";

                PdfPage page = document.AddPage();
                page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                page.Size = PdfSharpCore.PageSize.A4;
                var gfx = XGraphics.FromPdfPage(page);

                int comecaDados = 10;
                var contaPagina = 1;

                Dictionary<string, int> cabecalho = new Dictionary<string, int>();

                var nomecliente = _configuration["ParametrosSistema:NomeCliente"].ToString();

                comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                    comecaDados, filtro, cabecalho);

                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Dirigente", cong.PastorResponsavelNome);
                comecaDados += 15;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }

                #region Totalizadores
                var totalRecebidoBatismo = relMensal.Totais.FirstOrDefault().TotalRecebidoBatismo.ToString();
                var totalRecMudanca = relMensal.Totais.FirstOrDefault().TotalRecMudanca.ToString();
                var totalRecAclamacao = relMensal.Totais.FirstOrDefault().TotalRecAclamacao.ToString();
                var totalAdmissoes = relMensal.Totais.FirstOrDefault().TotalAdmissoes.ToString();
                var totalDemissoes = relMensal.Totais.FirstOrDefault().TotalDemissoes.ToString();

                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Membros Recebido por", "");
                ImprimirLinhaDescrCont(gfx, 200, comecaDados + 70, "Batismo", totalRecebidoBatismo);
                ImprimirLinhaDescrCont(gfx, 300, comecaDados + 70, "Mudança", totalRecMudanca);
                ImprimirLinhaDescrCont(gfx, 400, comecaDados + 70, "Aclamação", totalRecAclamacao);
                comecaDados += 15;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                                     comecaDados, filtro, cabecalho);
                }
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Total de Adesões", totalAdmissoes);
                ImprimirLinhaDescrCont(gfx, 200, comecaDados + 70, "Total de Demissões", totalRecebidoBatismo);
                comecaDados += 5;
                gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 70, 575, comecaDados + 70);
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                                     comecaDados, filtro, cabecalho);
                }
                #endregion

                #region Impretações de Bençãos
                comecaDados += 15;
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Impretações de Bençãos", "");
                comecaDados += 15;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                if (relMensal.Casamento.Any())
                {
                    gfx.DrawString("Nome Noivo", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    gfx.DrawString("Nome Noiva", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(175, comecaDados + 70, 0, 0));
                    gfx.DrawString("Nome Pastor", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(345, comecaDados + 70, 0, 0));
                    gfx.DrawString("Data", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(520, comecaDados + 70, 0, 0));
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }

                    foreach (var itemCasamento in relMensal.Casamento)
                    {
                        var qtdLinhasPular = new List<int>();
                        var font = new XFont("Arial", 10, XFontStyle.Regular);
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemCasamento.NoivoNome, font, 25, 184, comecaDados + 70));
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemCasamento.NoivaNome, font, 175, 344, comecaDados + 70));
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemCasamento.PastorNome, font, 345, 504, comecaDados + 70));
                        gfx.DrawString(itemCasamento.Data.Date.ToShortDateString(), font, XBrushes.Black, new XRect(520, comecaDados + 70, 0, 0));
                        comecaDados += (12 * qtdLinhasPular.Max());
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                          comecaDados, filtro, cabecalho);
                        }
                    }
                }
                else
                {
                    gfx.DrawString("Não há impetrações de Bençãos para o mês.", new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }
                    comecaDados += 5;
                }
                gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 70, 575, comecaDados + 70);
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                #endregion

                #region Admissão por Reconciliação
                comecaDados += 15;
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Admissão por Reconciliação", "");
                comecaDados += 15;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                if (relMensal.Reconciliacao.Any())
                {
                    gfx.DrawString("Nome", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    gfx.DrawString("Data", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }

                    foreach (var itemReconciliacao in relMensal.Reconciliacao)
                    {
                        var qtdLinhasPular = new List<int>();
                        var font = new XFont("Arial", 10, XFontStyle.Regular);
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemReconciliacao.Nome, font, 25, 184, comecaDados + 70));
                        gfx.DrawString(itemReconciliacao.Data.Date.ToShortDateString(), font, XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                        comecaDados += (12 * qtdLinhasPular.Max());
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                          comecaDados, filtro, cabecalho);
                        }
                    }
                }
                else
                {
                    gfx.DrawString("Não há Admissões por Reconciliação para o mês.", new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }
                    comecaDados += 5;
                }
                gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 70, 575, comecaDados + 70);
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                #endregion

                #region Recebido por Carta de Mudança
                comecaDados += 15;
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Recebido por Carta de Mudança", "");
                comecaDados += 15;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                if (relMensal.RecebidoCartaMudanca.Any())
                {
                    gfx.DrawString("Nome", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    gfx.DrawString("Data", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }

                    foreach (var itemRecebidoCartaMudanca in relMensal.RecebidoCartaMudanca)
                    {
                        var qtdLinhasPular = new List<int>();
                        var font = new XFont("Arial", 10, XFontStyle.Regular);
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemRecebidoCartaMudanca.Nome, font, 25, 184, comecaDados + 70));
                        gfx.DrawString(itemRecebidoCartaMudanca.Data.Date.ToShortDateString(), font, XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                        comecaDados += (12 * qtdLinhasPular.Max());
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                          comecaDados, filtro, cabecalho);
                        }
                    }
                }
                else
                {
                    gfx.DrawString("Não há membros recebidos por Carta de Mudança para o mês.", new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }
                    comecaDados += 5;
                }
                gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 70, 575, comecaDados + 70);
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                #endregion

                #region Recebido por Aclamação
                comecaDados += 15;
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Recebido por Aclamação", "");
                comecaDados += 15;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                if (relMensal.RecebidoAclamacao.Any())
                {
                    gfx.DrawString("Nome", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    gfx.DrawString("Data", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }

                    foreach (var itemRecebidoAclamacao in relMensal.RecebidoAclamacao)
                    {
                        var qtdLinhasPular = new List<int>();
                        var font = new XFont("Arial", 10, XFontStyle.Regular);
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemRecebidoAclamacao.Nome, font, 25, 184, comecaDados + 70));
                        gfx.DrawString(itemRecebidoAclamacao.Data.Date.ToShortDateString(), font, XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                        comecaDados += (12 * qtdLinhasPular.Max());
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                          comecaDados, filtro, cabecalho);
                        }
                    }
                }
                else
                {
                    gfx.DrawString("Não há membros recebidos por Aclamação para o mês.", new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }
                    comecaDados += 5;
                }
                gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 70, 575, comecaDados + 70);
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                #endregion

                #region Recebido por Transferência
                comecaDados += 15;
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Recebido por Transferência", "");
                comecaDados += 15;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                if (relMensal.RecebidoTransferencia.Any())
                {
                    gfx.DrawString("Nome", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    gfx.DrawString("Data", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }

                    foreach (var itemRecebidoTransferencia in relMensal.RecebidoTransferencia)
                    {
                        var qtdLinhasPular = new List<int>();
                        var font = new XFont("Arial", 10, XFontStyle.Regular);
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemRecebidoTransferencia.Nome, font, 25, 184, comecaDados + 70));
                        gfx.DrawString(itemRecebidoTransferencia.Data.Date.ToShortDateString(), font, XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                        comecaDados += (12 * qtdLinhasPular.Max());
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                          comecaDados, filtro, cabecalho);
                        }
                    }
                }
                else
                {
                    gfx.DrawString("Não há membros recebidos por Transferência para o mês.", new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }
                    comecaDados += 5;
                }
                gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 70, 575, comecaDados + 70);
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                #endregion

                #region Saída por
                comecaDados += 15;
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Saída por", "");
                comecaDados += 15;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                if (relMensal.SaidaPor.Any())
                {
                    gfx.DrawString("Nome", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    gfx.DrawString("Motivo", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(260, comecaDados + 70, 0, 0));
                    gfx.DrawString("Data", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }

                    foreach (var itemSaidaPor in relMensal.SaidaPor)
                    {
                        var qtdLinhasPular = new List<int>();
                        var font = new XFont("Arial", 10, XFontStyle.Regular);
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemSaidaPor.Nome, font, 25, 259, comecaDados + 70));
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemSaidaPor.Motivo, font, 260, 499, comecaDados + 70));
                        gfx.DrawString(itemSaidaPor.Data.Date.ToShortDateString(), font, XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                        comecaDados += (12 * qtdLinhasPular.Max());
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                          comecaDados, filtro, cabecalho);
                        }
                    }
                }
                else
                {
                    gfx.DrawString("Não há Saídas de membros no mês.", new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }
                    comecaDados += 5;
                }
                gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 70, 575, comecaDados + 70);
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                #endregion

                #region Saída por Transferência
                comecaDados += 15;
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Saída por Transferência", "");
                comecaDados += 15;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                if (relMensal.SaidaPorTranferencia.Any())
                {
                    gfx.DrawString("Nome", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    gfx.DrawString("Congregação", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(260, comecaDados + 70, 0, 0));
                    gfx.DrawString("Data", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }

                    foreach (var itemSaidaPorTranferencia in relMensal.SaidaPorTranferencia)
                    {
                        var qtdLinhasPular = new List<int>();
                        var font = new XFont("Arial", 10, XFontStyle.Regular);
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemSaidaPorTranferencia.Nome, font, 25, 259, comecaDados + 70));
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemSaidaPorTranferencia.NomeCongregacao, font, 260, 499, comecaDados + 70));
                        gfx.DrawString(itemSaidaPorTranferencia.Data.Date.ToShortDateString(), font, XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                        comecaDados += (12 * qtdLinhasPular.Max());
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                          comecaDados, filtro, cabecalho);
                        }
                    }
                }
                else
                {
                    gfx.DrawString("Não há Saídas por Transferência no mês.", new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }
                    comecaDados += 5;
                }
                gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 70, 575, comecaDados + 70);
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                #endregion

                #region Saída por Carta de Mudança
                comecaDados += 15;
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Saída por Carta de Mudança", "");
                comecaDados += 15;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                if (relMensal.SaidaPorMudanca.Any())
                {
                    gfx.DrawString("Nome", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    gfx.DrawString("Congregação", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(260, comecaDados + 70, 0, 0));
                    gfx.DrawString("Data", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }

                    foreach (var itemSaidaPorMudanca in relMensal.SaidaPorMudanca)
                    {
                        var qtdLinhasPular = new List<int>();
                        var font = new XFont("Arial", 10, XFontStyle.Regular);
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemSaidaPorMudanca.Nome, font, 25, 259, comecaDados + 70));
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemSaidaPorMudanca.NomeCongregacao, font, 260, 499, comecaDados + 70));
                        gfx.DrawString(itemSaidaPorMudanca.Data.Date.ToShortDateString(), font, XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                        comecaDados += (12 * qtdLinhasPular.Max());
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                          comecaDados, filtro, cabecalho);
                        }
                    }
                }
                else
                {
                    gfx.DrawString("Não há Saídas por Carta de Mudança no mês.", new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }
                    comecaDados += 5;
                }
                gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 70, 575, comecaDados + 70);
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                #endregion

                #region Funeral
                comecaDados += 15;
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Funeral", "");
                comecaDados += 15;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                if (relMensal.Funeral.Any())
                {
                    gfx.DrawString("Nome", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    gfx.DrawString("Data", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }

                    foreach (var itemFuneral in relMensal.Funeral)
                    {
                        var qtdLinhasPular = new List<int>();
                        var font = new XFont("Arial", 10, XFontStyle.Regular);
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemFuneral.Nome, font, 25, 184, comecaDados + 70));
                        gfx.DrawString(itemFuneral.Data.Date.ToShortDateString(), font, XBrushes.Black, new XRect(500, comecaDados + 70, 0, 0));
                        comecaDados += (12 * qtdLinhasPular.Max());
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                          comecaDados, filtro, cabecalho);
                        }
                    }
                }
                else
                {
                    gfx.DrawString("Não há Funerais para o mês.", new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }
                    comecaDados += 5;
                }
                gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 70, 575, comecaDados + 70);
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                #endregion

                #region Crianças Apresentadas
                comecaDados += 15;
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Crianças Apresentadas", "");
                comecaDados += 15;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                if (relMensal.CriancasApresentadas.Any())
                {
                    gfx.DrawString("Nome", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    gfx.DrawString("Dt.Nasc.", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(120, comecaDados + 70, 0, 0));
                    gfx.DrawString("Pai", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(180, comecaDados + 70, 0, 0));
                    gfx.DrawString("Mãe", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(300, comecaDados + 70, 0, 0));
                    gfx.DrawString("Pastor Oficiante", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(420, comecaDados + 70, 0, 0));
                    gfx.DrawString("Data Apres.", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(520, comecaDados + 70, 0, 0));
                    comecaDados += 12;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }

                    foreach (var itemCriancasApresentadas in relMensal.CriancasApresentadas)
                    {
                        var qtdLinhasPular = new List<int>();
                        var font = new XFont("Arial", 10, XFontStyle.Regular);
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemCriancasApresentadas.NomeCrianca, font, 25, 119, comecaDados + 70));
                        gfx.DrawString(itemCriancasApresentadas.DataNascimento.Date.ToShortDateString(), font, XBrushes.Black, new XRect(120, comecaDados + 70, 0, 0));
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemCriancasApresentadas.NomePai, font, 180, 299, comecaDados + 70));
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemCriancasApresentadas.NomeMae, font, 300, 419, comecaDados + 70));
                        qtdLinhasPular.Add(ImprimirLinha(gfx, itemCriancasApresentadas.PastorOficiante, font, 420, 519, comecaDados + 70));
                        gfx.DrawString(itemCriancasApresentadas.DataApresentacaoCrianca.Date.ToShortDateString(), font, XBrushes.Black, new XRect(520, comecaDados + 70, 0, 0));
                        comecaDados += (12 * qtdLinhasPular.Max());
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                          comecaDados, filtro, cabecalho);
                        }
                    }
                }
                else
                {
                    gfx.DrawString("Não há Crianças apresentadas para o mês.", new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);
                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                      comecaDados, filtro, cabecalho);
                    }
                    comecaDados += 5;
                }
                comecaDados += 12;
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Total Crianças Apresentadas", relMensal.Totais.FirstOrDefault().TotalCriancasApresentadas.ToString());
                comecaDados += 5;
                gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 70, 575, comecaDados + 70);
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                #endregion

                comecaDados += 12;
                var TotalCongregados = relMensal.Totais.FirstOrDefault().TotalCongregados.ToString();
                var TotalMembros = relMensal.Totais.FirstOrDefault().TotalMembros.ToString();
                ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Total Congregados", TotalCongregados);
                ImprimirLinhaDescrCont(gfx, 260, comecaDados + 70, "Total Membros", TotalMembros);
                comecaDados += 12;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                comecaDados += 10;
                gfx.DrawString("OBS: ESTE RELATÓRIO DEVE ESTAR ATUALIZADO ATÉ A REUNIÃO DE OBREIROS DO MÊS SEGUINTE.", new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                comecaDados += 20;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                gfx.DrawString("_________________________________________", new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(0, comecaDados + 70, page.Width / 2, 0), new XStringFormat() { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center });
                gfx.DrawString("_________________________________________", new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(page.Width / 2, comecaDados + 70, page.Width / 2, 0), new XStringFormat() { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center });
                comecaDados += 15;
                gfx.DrawString("Dirigente da Congregação", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(0, comecaDados + 70, page.Width / 2, 0), new XStringFormat() { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center });
                gfx.DrawString("Secretário(a) da Congregação", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(page.Width / 2, comecaDados + 70, page.Width / 2, 0), new XStringFormat() { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center });
                comecaDados += 30;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, $"Relatório Mensal - Ref.: {mes.PadLeft(2, '0')}/{ano}", UserAppContext.Current.Usuario.Username,
                                  comecaDados, filtro, cabecalho);
                }
                gfx.DrawString("_______________________________________________", new XFont("Arial", 10, XFontStyle.Regular), XBrushes.Black, new XRect(0, comecaDados + 70, page.Width, 0), new XStringFormat() { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center });
                comecaDados += 15;
                gfx.DrawString(pastor, new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(0, comecaDados + 70, page.Width, 0), new XStringFormat() { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center });

                var nomeArquivo = $"RelatorioMensal_{congregacaoId}_{mes}_{ano}";
                MemoryStream stream = new MemoryStream();
                document.Save(stream, false);
                return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/pdf");

            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        public FileStreamResult RelatorioEventos(string congregacao,
                                                 int mes,
                                                 int ano,
                                                 string tipoEvento,
                                                 bool simplificado)
        {
            try
            {
                int.TryParse(congregacao, out int congrId);
                Enum.TryParse(tipoEvento, out TipoEvento tpEvento);

                var mem = _relatoriosAppService.RelatorioEventos(congrId, mes, ano, tpEvento);
                if (!mem.Eventos.Any())
                    throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

                var filtro = "";
                if (congrId > 0)
                {
                    var congrNome = _congregacaoAppService.GetById(congrId, UserAppContext.Current.Usuario.Id).Nome;
                    if (congrNome.Length > 20)
                        congrNome = $"{congrNome.Substring(0, 20)}...";
                    filtro += $"Congregação: {congrNome}";
                }
                else
                    filtro += "Todas as Congregações";

                if (mes > 0)
                    filtro += $" - Mês: {mes}";

                if (ano > 0)
                    filtro += $" - Ano: {ano}";

                if (tpEvento != TipoEvento.NaoDefinido)
                    filtro += $" - Tipo de Evento: {tpEvento.GetDisplayAttributeValue()}";
                else
                    filtro += " - Tipo de Evento: Todos";


                var document = new PdfDocument();
                document.Info.Title = "Relatório de Eventos";
                document.Info.Author = "Architect Systems";
                document.Info.Subject = "Membros";
                document.Info.Keywords = "PDFsharp, XGraphics";

                PdfPage page = document.AddPage();
                page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                page.Size = PdfSharpCore.PageSize.A4;
                var gfx = XGraphics.FromPdfPage(page);

                int comecaDados = 10;
                var contaPagina = 1;
                Dictionary<string, int> cabecalho = new Dictionary<string, int>();

                var nomecliente = _configuration["ParametrosSistema:NomeCliente"].ToString();
                comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "EVENTOS", UserAppContext.Current.Usuario.Username,
                    comecaDados, filtro, cabecalho);
                if (!simplificado)
                {

                    DateTime dataInicial = new DateTime(ano, (mes == 0 ? 1 : mes), 1);
                    DateTime dataFinal;
                    if (mes == 0)
                        dataFinal = new DateTime(ano, 12, 31);
                    else
                        dataFinal = dataInicial.AddMonths(1).AddDays(-1);

                    while (dataInicial <= dataFinal)
                    {
                        if (dataInicial.Day == 1)
                        {
                            comecaDados += 2;
                            var mesExt = $"{MesExtenso(dataInicial.Month)}/{dataInicial.Year}";
                            XRect rect = new XRect(0, comecaDados + 60, (page.Width - mesExt.Length), 0);
                            XStringFormat format = new XStringFormat
                            {
                                Alignment = XStringAlignment.Center
                            };
                            gfx.DrawString(mesExt, new XFont("Arial", 12, XFontStyle.Bold), XBrushes.Black, rect, format);
                            comecaDados += 8;
                            var arial8Bold = new XFont("Arial", 8, XFontStyle.Bold);
                            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 70, 575, comecaDados + 70);
                            comecaDados += 10;
                            gfx.DrawString("DIA", arial8Bold, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                            gfx.DrawString("SEMANA", arial8Bold, XBrushes.Black, new XRect(50, comecaDados + 70, 0, 0));
                            gfx.DrawString("HORÁRIO", arial8Bold, XBrushes.Black, new XRect(100, comecaDados + 70, 0, 0));
                            gfx.DrawString("PROGRAMAÇÃO DO DIA", arial8Bold, XBrushes.Black, new XRect(150, comecaDados + 70, 0, 0));
                            comecaDados += 5;
                            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 70, 575, comecaDados + 70);
                        }

                        comecaDados += 10;
                        var arial8Reg = new XFont("Arial", 8, XFontStyle.Regular);
                        var semExt = SemanaExtenso(dataInicial.Date);
                        var color = XBrushes.Black;
                        if (semExt == "DOM" || mem.Feriados.Any(f => f.Data.Date == dataInicial.Date))
                            color = XBrushes.DarkRed;
                        gfx.DrawString(dataInicial.Day.ToString().PadLeft(2, '0'), arial8Reg, color, new XRect(28, comecaDados + 70, 0, 0));
                        gfx.DrawString(semExt, arial8Reg, color, new XRect(60, comecaDados + 70, 0, 0));

                        if (mem.Feriados.Any(f => f.Data.Date == dataInicial.Date))
                        {
                            foreach (var fer in mem.Feriados.Where(f => f.Data.Date == dataInicial.Date))
                            {
                                gfx.DrawString(fer.Descricao, arial8Reg, color, new XRect(150, comecaDados + 70, 0, 0));
                                comecaDados += 10;
                            }
                            comecaDados -= 10;
                        }

                        if (mem.Eventos.Any(e => e.DataHoraInicio.Date == dataInicial.Date))
                        {
                            if (mem.Feriados.Any(f => f.Data.Date == dataInicial.Date))
                                comecaDados += 10;
                            var eventos = mem.Eventos.Where(e => e.DataHoraInicio.Date == dataInicial.Date);
                            foreach (var item in eventos)
                            {
                                gfx.DrawString(string.Format("{0:HH:mm}", item.DataHoraInicio), arial8Reg, XBrushes.Black, new XRect(108, comecaDados + 70, 0, 0));
                                var qtdLinhasPular = new List<int>
                            {
                                ImprimirLinha(gfx, item.Descricao, arial8Reg, 150, 580, comecaDados + 70)
                            };
                                comecaDados += (12 * qtdLinhasPular.Max());
                            }
                            comecaDados -= 10;
                        }
                        comecaDados += 4;
                        gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 70, 575, comecaDados + 70);
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "EVENTOS", UserAppContext.Current.Usuario.Username,
                                    comecaDados, filtro, cabecalho);
                        }

                        dataInicial = dataInicial.AddDays(1);
                        if (dataInicial.Day == 1 && dataInicial <= dataFinal)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "EVENTOS", UserAppContext.Current.Usuario.Username,
                                               comecaDados, filtro, cabecalho);
                        }
                    }
                }
                else
                {
                    var grpMes = mem.Eventos.GroupBy(x => new { x.DataHoraInicio.Month, x.DataHoraInicio.Year })
                                .Select(g => new { g.Key.Month, g.Key.Year, Total = g.Count() })
                                .OrderBy(o => o.Month);
                    foreach (var grp in grpMes)
                    {
                        var cont = 0;
                        var mesExt = $"{MesExtenso(grp.Month)}/{grp.Year}";
                        XRect rect = new XRect(0, comecaDados + 60, (page.Width - mesExt.Length), 0);
                        XStringFormat format = new XStringFormat
                        {
                            Alignment = XStringAlignment.Center
                        };
                        gfx.DrawString(mesExt, new XFont("Arial", 12, XFontStyle.Bold), XBrushes.Black, rect, format);
                        comecaDados += 15;
                        var arial8Bold = new XFont("Arial", 8, XFontStyle.Bold);
                        gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 60, 575, comecaDados + 60);
                        comecaDados += 10;
                        gfx.DrawString("DIA", arial8Bold, XBrushes.Black, new XRect(25, comecaDados + 60, 0, 0));
                        gfx.DrawString("SEMANA", arial8Bold, XBrushes.Black, new XRect(50, comecaDados + 60, 0, 0));
                        gfx.DrawString("HORÁRIO", arial8Bold, XBrushes.Black, new XRect(100, comecaDados + 60, 0, 0));
                        gfx.DrawString("PROGRAMAÇÃO DO DIA", arial8Bold, XBrushes.Black, new XRect(150, comecaDados + 60, 0, 0));
                        comecaDados += 5;
                        gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 60, 575, comecaDados + 60);
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "EVENTOS", UserAppContext.Current.Usuario.Username,
                                    comecaDados, filtro, cabecalho);
                        }
                        comecaDados += 10;
                        var eventos = mem.Eventos.Where(e => e.DataHoraInicio.Month == grp.Month);
                        foreach (var evt in eventos)
                        {
                            cont++;
                            var arial8Reg = new XFont("Arial", 8, XFontStyle.Regular);
                            var semExt = SemanaExtenso(evt.DataHoraInicio.Date);
                            var color = XBrushes.Black;
                            if (semExt == "DOM" || mem.Feriados.Any(f => f.Data.Date == evt.DataHoraInicio.Date))
                                color = XBrushes.DarkRed;
                            gfx.DrawString(evt.DataHoraInicio.Day.ToString().PadLeft(2, '0'), arial8Reg, color, new XRect(28, comecaDados + 60, 0, 0));
                            gfx.DrawString(semExt, arial8Reg, color, new XRect(60, comecaDados + 60, 0, 0));
                            gfx.DrawString(string.Format("{0:HH:mm}", evt.DataHoraInicio), arial8Reg, XBrushes.Black, new XRect(108, comecaDados + 60, 0, 0));
                            var qtdLinhasPular = new List<int>
                            {
                                ImprimirLinha(gfx, evt.Descricao, arial8Reg, 150, 580, comecaDados + 60)
                            };
                            comecaDados += (10 * qtdLinhasPular.Max());

                            if (cont >= grp.Total)
                            {
                                gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados + 60, 575, comecaDados + 60);
                                comecaDados += 5;
                            }
                            if (comecaDados > 740)
                            {
                                page = document.AddPage();
                                page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                                page.Size = PdfSharpCore.PageSize.A4;
                                gfx = XGraphics.FromPdfPage(page);
                                contaPagina += 1;
                                comecaDados = 10;
                                comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "EVENTOS", UserAppContext.Current.Usuario.Username,
                                        comecaDados, filtro, cabecalho);
                                comecaDados += 5;
                            }
                        }
                    }
                }

                var nomeArquivo = $"RelatorioEventos_{congregacao}";
                MemoryStream stream = new MemoryStream();
                document.Save(stream, false);
                return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/pdf");

            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        public FileStreamResult RelatorioPresencaLista(int idPresenca,
                                                       int idData)
        {
            try
            {
                var presenca = _relatoriosAppService.RelatorioPresencaLista(idPresenca, idData, UserAppContext.Current.Usuario.Id);
                if (!presenca.Any())
                    throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

                var filtro = $"Curso/Evento: {presenca.FirstOrDefault().Descricao}";


                var document = new PdfDocument();
                document.Info.Title = "Relatório de Lista Presença - CURSOS/EVENTOS";
                document.Info.Author = "Architect Systems";
                document.Info.Subject = "Relatório de Lista Presença - CURSOS/EVENTOS";
                document.Info.Keywords = "PDFsharp, XGraphics";

                PdfPage page = document.AddPage();
                page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                page.Size = PdfSharpCore.PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);

                int comecaDados = 10;
                var contaPagina = 1;
                Dictionary<string, int> cabecalho = new Dictionary<string, int>
                    {
                        { "NOME", 25 },
                        { "CPF", 210 },
                        { "CONGREG./IGREJA.", 270 },
                        { "CARGO", 440 },
                        { "SITUAÇÃO", 530 }
                    };
                var nomecliente = _configuration["ParametrosSistema:NomeCliente"].ToString();

                comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA PRESENÇA - CURSOS/EVENTO", UserAppContext.Current.Usuario.Username,
                    comecaDados, filtro, cabecalho);

                var grpDatas = presenca.GroupBy(x => new { x.Id, x.DataHoraInicio, x.DataHoraFim })
                                .Select(g => new { g.Key.Id, g.Key.DataHoraInicio, g.Key.DataHoraFim, Total = g.Count() })
                                .OrderBy(o => o.DataHoraInicio);

                foreach (var itemData in grpDatas)
                {
                    var font = new XFont("Arial", 8, XFontStyle.Bold);
                    gfx.DrawString($"Data: {itemData.DataHoraInicio.ToShortDateString()} - Início: {itemData.DataHoraInicio.ToShortTimeString()} - Fim: {itemData.DataHoraFim.ToShortDateString()}", font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                    comecaDados += 12;

                    var inscr = presenca.Where(c => c.DataHoraInicio == itemData.DataHoraInicio && c.DataHoraFim == itemData.DataHoraFim).OrderBy(o => o.Nome);
                    foreach (var item in inscr)
                    {
                        font = new XFont("Arial", 8, XFontStyle.Regular);
                        var nome = item.Nome;
                        if (item.MembroId != 0)
                            nome += $" - {item.MembroId}";
                        var congr = item.Igreja;
                        if (item.CongregacaoId != 0)
                            congr += $" - {item.CongregacaoId}";

                        gfx.DrawString(nome, font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                        gfx.DrawString(item.CPF, font, XBrushes.Black, new XRect(210, comecaDados + 80, 0, 0));
                        gfx.DrawString(congr, font, XBrushes.Black, new XRect(270, comecaDados + 80, 0, 0));
                        gfx.DrawString(item.Cargo, font, XBrushes.Black, new XRect(440, comecaDados + 80, 0, 0));
                        gfx.DrawString(item.Situacao.GetDisplayAttributeValue(), font, XBrushes.Black, new XRect(530, comecaDados + 80, 0, 0));

                        comecaDados += 12;
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA PRESENÇA - CURSOS/EVENTO", UserAppContext.Current.Usuario.Username,
                                                             comecaDados, filtro, cabecalho);
                        }
                        if (!string.IsNullOrWhiteSpace(item.Justificativa))
                        {
                            ImprimirLinhaDescrCont(gfx, 25, comecaDados + 80, "JUSTIFICATIVA", item.Justificativa, new XFont("Arial", 8, XFontStyle.Bold), new XFont("Arial", 8, XFontStyle.Regular));
                            comecaDados += 15;
                            if (comecaDados > 740)
                            {
                                page = document.AddPage();
                                page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                                page.Size = PdfSharpCore.PageSize.A4;
                                gfx = XGraphics.FromPdfPage(page);
                                contaPagina += 1;
                                comecaDados = 10;
                                comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA PRESENÇA - CURSOS/EVENTO", UserAppContext.Current.Usuario.Username,
                                                                 comecaDados, filtro, cabecalho);
                            }
                        }
                    }
                    font = new XFont("Arial", 8, XFontStyle.Bold);
                    gfx.DrawString($"Data: {itemData.DataHoraInicio.ToShortDateString()} - Início: {itemData.DataHoraInicio.ToShortTimeString()} - Fim: {itemData.DataHoraFim.ToShortDateString()} - Total: {itemData.Total}", font, XBrushes.Black, new XRect(25, comecaDados + 80, 0, 0));
                    comecaDados += 24;
                    if (comecaDados > 740)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);

                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, "LISTA DE PRESENÇA - CURSOS/EVENTO", UserAppContext.Current.Usuario.Username,
                                           comecaDados, filtro, cabecalho);
                    }
                }

                var nomeArquivo = $"listapresenca_{presenca.FirstOrDefault().Id}";
                MemoryStream stream = new MemoryStream();
                document.Save(stream, false);
                return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/pdf");
            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        public FileStreamResult RelatorioPresencaInscritos(int idPresenca,
                                                           int congregacao,
                                                           string tipo,
                                                           bool carimbo,
                                                           string formato)
        {
            try
            {
                var tpRel = 0;
                if (!string.IsNullOrWhiteSpace(tipo))
                    tpRel = tipo == "Membro" ? 1 : 2;

                var lista = _relatoriosAppService.RelatorioInscricoes(idPresenca, congregacao, tpRel, UserAppContext.Current.Usuario.Id);
                if (!lista.Any())
                    throw new Erro("Não foi encontrado registros para o Filtro selecionado.");

                string filtro = "";
                if (idPresenca > 0)
                {
                    var pres = _presencaAppService.GetById(idPresenca, 0);
                    var descr = pres.Descricao;
                    if (descr.Length > 15)
                        descr = $"{descr.Substring(0, 15)}...";
                    filtro = $"Curso/Evento: {descr}";
                }
                else
                    filtro = $"Curso/Evento: Todos";

                if (congregacao > 0)
                {
                    var congr = _congregacaoAppService.GetById(congregacao, UserAppContext.Current.Usuario.Id).Nome;
                    if (congr.Length > 15)
                        congr = $"{congr.Substring(0, 15)}...";
                    filtro += $" - Congregação: {congr}";
                }
                else
                    filtro += " - Todas as Congregações";

                if (!string.IsNullOrWhiteSpace(tipo))
                    filtro += $" - Tipo de Inscrição: {(tipo == "Membro" ? "Membros" : "Não Membros")}";
                else
                    filtro += " - Todos Tipo de Inscrição";

                var nomeArquivo = $"Inscritos_{(idPresenca > 0 ? idPresenca.ToString() : "Todos")}";
                return RelatorioPresencaInscritosPDF(lista, filtro, nomeArquivo, carimbo);

            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        private FileStreamResult RelatorioPresencaInscritosPDF(List<PresencaLista> lista,
                                                               string filtro,
                                                               string nomeArquivo,
                                                               bool carimbo)
        {
            var document = new PdfDocument();
            document.Info.Title = "Relatório de Lista de Inscritos";
            document.Info.Author = "Architect Systems";
            document.Info.Subject = "Lista de Inscritos";
            document.Info.Keywords = "PDFsharp, XGraphics";

            PdfPage page = document.AddPage();
            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
            page.Size = PdfSharpCore.PageSize.A4;
            XGraphics gfx = XGraphics.FromPdfPage(page);

            int comecaDados = 10;
            var contaPagina = 1;
            Dictionary<string, int> cabecalho = new Dictionary<string, int>();

            var nomecliente = _configuration["ParametrosSistema:NomeCliente"].ToString();
            var cabec = "INSCRIÇÕES";

            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, cabec, UserAppContext.Current.Usuario.Username,
                comecaDados, filtro, cabecalho);


            var right = new XStringFormat
            {
                Alignment = XStringAlignment.Far
            };
            var left = new XStringFormat
            {
                Alignment = XStringAlignment.Near
            };


            //somente vai imprimir o footer caso seja escolhido um unico curso, para uma unica congregação e um tipo de relatorio.
            var tamFolha = carimbo ? 710 : 810;
            var grpEvento = lista
                            .GroupBy(g => new { g.Id, g.Descricao, g.Valor })
                            .Select(g => new { g.Key.Id, g.Key.Descricao, g.Key.Valor, Total = g.Count() })
                            .OrderBy(o => o.Id);


            var font10 = new XFont("Arial", 10, XFontStyle.Bold);
            var fontArial8 = new XFont("Arial", 8, XFontStyle.Bold);
            comecaDados += 71;
            var qtdEvt = 0;
            foreach (var evt in grpEvento)
            {
                var impValor = evt.Valor > 0;
                qtdEvt++;
                Dictionary<string, int> header;
                if (impValor)
                {
                    header = new Dictionary<string, int>
                            {
                                { "Nº", 25 },
                                { "NOME COMPLETO", 50 },
                                { "CPF", 270 },
                                { "CARGO", 370 },
                                { "SITUAÇÃO", 500 }
                            };
                }
                else
                {
                    header = new Dictionary<string, int>
                            {
                                { "Nº", 25 },
                                { "NOME COMPLETO", 50 },
                                { "CPF", 270 },
                                { "CARGO", 370 }
                            };
                }

                gfx.DrawString($"CURSO/EVENTO: {evt.Descricao}", font10, XBrushes.Black, new XRect(25, comecaDados, 0, 0));
                comecaDados += 12;
                ControlarPaginaRelInscritos(carimbo, filtro, document, ref page, ref gfx, ref comecaDados, ref contaPagina, cabecalho, nomecliente, cabec, tamFolha);
                var grpCongr = lista
                        .Where(w => w.Id == evt.Id)
                        .GroupBy(x => new { x.CongregacaoId, x.Igreja })
                        .Select(g => new { g.Key.CongregacaoId, g.Key.Igreja, Total = g.Count() })
                        .OrderBy(o => o.Igreja);

                var qtdCongr = 0;
                foreach (var congr in grpCongr)
                {
                    qtdCongr++;
                    gfx.DrawString($"CONGR./IGREJA: {(string.IsNullOrWhiteSpace(congr.Igreja) ? "Não informado" : congr.Igreja)}", font10, XBrushes.Black, new XRect(25, comecaDados, 0, 0));
                    comecaDados += 12;
                    ControlarPaginaRelInscritos(carimbo, filtro, document, ref page, ref gfx, ref comecaDados, ref contaPagina, cabecalho, nomecliente, cabec, tamFolha);

                    foreach (var item in header)
                        gfx.DrawString(item.Key, fontArial8, XBrushes.Black, new XRect(item.Value, comecaDados, 0, 0));
                    comecaDados += 12;
                    ControlarPaginaRelInscritos(carimbo, filtro, document, ref page, ref gfx, ref comecaDados, ref contaPagina, cabecalho, nomecliente, cabec, tamFolha);

                    var membros = lista.Where(m => m.Id == evt.Id && m.Igreja == congr.Igreja).OrderBy(o => o.Nome);
                    var cont = 1;
                    foreach (var item in membros)
                    {
                        var qtdLinhasPular = new List<int>();

                        var font = new XFont("Arial", 10, XFontStyle.Regular);
                        gfx.DrawString(cont.ToString(), font, XBrushes.Black, new XRect(25, comecaDados, 0, 0));
                        qtdLinhasPular.Add(ImprimirLinha(gfx, item.Nome, font, 50, 249, comecaDados));
                        qtdLinhasPular.Add(ImprimirLinha(gfx, item.CPF, font, 270, 379, comecaDados));
                        qtdLinhasPular.Add(ImprimirLinha(gfx, item.Cargo, font, 370, 529, comecaDados));
                        if (impValor)
                            qtdLinhasPular.Add(ImprimirLinha(gfx, item.Pago ? "Pago" : "Em aberto", font, 500, 580, comecaDados));
                        comecaDados += (12 * qtdLinhasPular.Max());
                        ControlarPaginaRelInscritos(carimbo, filtro, document, ref page, ref gfx, ref comecaDados, ref contaPagina, cabecalho, nomecliente, cabec, tamFolha);
                        cont++;
                    }

                    if (carimbo)
                    {
                        var qtdPago = membros.Count(p => p.Igreja == congr.Igreja && p.Pago);
                        var qtdNaoPago = membros.Count(p => p.Igreja == congr.Igreja && !p.Pago);
                        var valor = evt.Valor;

                        gfx.DrawString("Total de Inscritos: ", font10, XBrushes.Black, new XRect(25, comecaDados, 250, 0), left);
                        gfx.DrawString(membros.Count().ToString(), font10, XBrushes.Black, new XRect(25, comecaDados, 250, 0), right);
                        comecaDados += 12;
                        ControlarPaginaRelInscritos(carimbo, filtro, document, ref page, ref gfx, ref comecaDados, ref contaPagina, cabecalho, nomecliente, cabec, tamFolha);

                        if (impValor)
                        {
                            gfx.DrawString("Valor(es) Pago(s): ", font10, XBrushes.Black, new XRect(25, comecaDados, 250, 0), left);
                            gfx.DrawString((qtdPago * valor).ToString("C", System.Globalization.CultureInfo.CurrentCulture), font10, XBrushes.Black, new XRect(25, comecaDados, 250, 0), right);
                            comecaDados += 12;
                            ControlarPaginaRelInscritos(carimbo, filtro, document, ref page, ref gfx, ref comecaDados, ref contaPagina, cabecalho, nomecliente, cabec, tamFolha);

                            gfx.DrawString("Valor(es) em Aberto: ", font10, XBrushes.Black, new XRect(25, comecaDados, 250, 0), left);
                            gfx.DrawString((qtdNaoPago * valor).ToString("C", System.Globalization.CultureInfo.CurrentCulture), font10, XBrushes.Black, new XRect(25, comecaDados, 250, 0), right);
                            comecaDados += 12;
                            ControlarPaginaRelInscritos(carimbo, filtro, document, ref page, ref gfx, ref comecaDados, ref contaPagina, cabecalho, nomecliente, cabec, tamFolha);
                        }
                        FooterPresencaInscrito(gfx);

                        if (qtdCongr < grpCongr.Count())
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);

                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, cabec, UserAppContext.Current.Usuario.Username,
                                                             comecaDados, filtro, cabecalho);
                            comecaDados += 71;
                            gfx.DrawString($"CURSO/EVENTO: {evt.Descricao}", font10, XBrushes.Black, new XRect(25, comecaDados, 0, 0));
                            comecaDados += 12;
                            ControlarPaginaRelInscritos(carimbo, filtro, document, ref page, ref gfx, ref comecaDados, ref contaPagina, cabecalho, nomecliente, cabec, tamFolha);
                        }
                    }
                    else
                    {
                        gfx.DrawString($"CONGR./IGREJA: {(string.IsNullOrWhiteSpace(congr.Igreja) ? "Não informado" : congr.Igreja)} - Total: {congr.Total}", font10, XBrushes.Black, new XRect(25, comecaDados, 0, 0));
                        comecaDados += 12;
                        ControlarPaginaRelInscritos(carimbo, filtro, document, ref page, ref gfx, ref comecaDados, ref contaPagina, cabecalho, nomecliente, cabec, tamFolha);
                    }
                }
                if (carimbo)
                {
                    FooterPresencaInscrito(gfx);

                    if (qtdEvt < grpEvento.Count())
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        page.Size = PdfSharpCore.PageSize.A4;
                        gfx = XGraphics.FromPdfPage(page);

                        contaPagina += 1;
                        comecaDados = 10;
                        comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, cabec, UserAppContext.Current.Usuario.Username,
                                                         comecaDados, filtro, cabecalho);
                        comecaDados += 71;
                        ControlarPaginaRelInscritos(carimbo, filtro, document, ref page, ref gfx, ref comecaDados, ref contaPagina, cabecalho, nomecliente, cabec, tamFolha);
                    }
                }
                else
                {
                    gfx.DrawString($"CURSO/EVENTO: {evt.Descricao} - Total de Inscritos: {evt.Total}", font10, XBrushes.Black, new XRect(25, comecaDados, 0, 0));
                    comecaDados += 20;
                    ControlarPaginaRelInscritos(carimbo, filtro, document, ref page, ref gfx, ref comecaDados, ref contaPagina, cabecalho, nomecliente, cabec, tamFolha);
                }
            }


            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/pdf");
        }

        private static void ControlarPaginaRelInscritos(bool carimbo,
                                                        string filtro,
                                                        PdfDocument document,
                                                        ref PdfPage page,
                                                        ref XGraphics gfx,
                                                        ref int comecaDados,
                                                        ref int contaPagina,
                                                        Dictionary<string, int> cabecalho,
                                                        string nomecliente,
                                                        string cabec,
                                                        int tamFolha)
        {
            if (comecaDados > tamFolha)
            {
                if (carimbo)
                    FooterPresencaInscrito(gfx);

                page = document.AddPage();
                page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                page.Size = PdfSharpCore.PageSize.A4;
                gfx = XGraphics.FromPdfPage(page);

                contaPagina += 1;
                comecaDados = 10;
                comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, cabec, UserAppContext.Current.Usuario.Username,
                                                 comecaDados, filtro, cabecalho);
                comecaDados += 71;
            }
        }

        private static void FooterPresencaInscrito(XGraphics gfx)
        {
            var linhaHfooter = 720;
            var linhaVfooter = 300;
            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, linhaHfooter, 575, linhaHfooter);
            gfx.DrawLine(new XPen(XColor.FromName("Black")), linhaVfooter, 720, linhaVfooter, 822);
            var fontArial6 = new XFont("Arial", 6, XFontStyle.Regular);
            var center = new XStringFormat
            {
                Alignment = XStringAlignment.Center
            };
            gfx.DrawString("CARIMBO SECRETARIA", fontArial6, XBrushes.Black, new XRect(0, 770, 300, 0), center);
            gfx.DrawString("CARIMBO TESOURARIA", fontArial6, XBrushes.Black, new XRect(290, 770, 300, 0), center);
        }

        #region Presenca - Frequencia
        public FileStreamResult RelatorioPresencaFrequencia(int idPresenca,
                                                            int idData,
                                                            int congregacao,
                                                            int situacao,
                                                            string formato)
        {
            try
            {
                string filtro = "";
                if (idPresenca > 0)
                {
                    var pres = _presencaAppService.GetById(idPresenca, 0);
                    var descr = pres.Descricao;
                    if (descr.Length > 15)
                        descr = $"{descr.Substring(0, 15)}...";
                    filtro = $"Curso/Evento: {descr}";
                }
                else
                {
                    filtro = $"Curso/Evento: Todos";
                }

                if (idData > 0)
                {
                    var datas = _presencaAppService.ListarPresencaDatas(idPresenca);
                    filtro += $" - Data: {datas.FirstOrDefault().DataHoraInicio.ToShortDateString()}";
                }
                else
                    filtro += " - Todas as datas";

                if (congregacao > 0)
                {
                    var congr = _congregacaoAppService.GetById(congregacao, UserAppContext.Current.Usuario.Id).Nome;
                    if (congr.Length > 15)
                        congr = $"{congr.Substring(0, 15)}...";
                    filtro += $" - Congregação: {congr}";
                }
                else
                    filtro += " - Todas as Congregações";

                if (situacao > 0)
                {
                    if (Enum.TryParse(situacao.ToString(), out SituacaoPresenca sit))
                        filtro += $" - Situação: {sit.GetDisplayAttributeValue()}";
                }
                else
                    filtro += " - Todas as Situações";

                var nomeArquivo = $"Frequencia_{(idPresenca > 0 ? idPresenca.ToString() : "Todos")}";

                if (formato == "Excel")
                    return RelatorioPresencaFrequenciaExcel(filtro, nomeArquivo, idPresenca, idData, congregacao, situacao);

                return RelatorioPresencaFrequenciaPDF(filtro, nomeArquivo, idPresenca, idData, congregacao, situacao);

            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        private FileStreamResult RelatorioPresencaFrequenciaPDF(string filtro,
                                                                string nomeArquivo,
                                                                int idPresenca,
                                                                int idData,
                                                                int congregacao,
                                                                int situacao)
        {
            var document = new PdfDocument();
            document.Info.Title = "Relatório de Lista de Presença - Frequência";
            document.Info.Author = "Architect Systems";
            document.Info.Subject = "Lista de Presença";
            document.Info.Keywords = "PDFsharp, XGraphics";

            PdfPage page = document.AddPage();
            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
            page.Size = PdfSharpCore.PageSize.A4;
            XGraphics gfx = XGraphics.FromPdfPage(page);

            int comecaDados = 10;
            var contaPagina = 1;
            Dictionary<string, int> cabecalho = new Dictionary<string, int>();

            var nomecliente = _configuration["ParametrosSistema:NomeCliente"].ToString();
            var cabec = "LISTA DE PRESENÇA - CURSOS/EVENTOS - Frequência";
            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, cabec, UserAppContext.Current.Usuario.Username,
                comecaDados, filtro, cabecalho);

            var listaPresenca = _presencaAppService.GetById(idPresenca, UserAppContext.Current.Usuario.Id);
            if (listaPresenca == null || listaPresenca.Id == 0)
                throw new Erro("Curso/Evento não localizado.");

            var font10 = new XFont("Arial", 10, XFontStyle.Bold);
            var datasSel = listaPresenca.Datas;
            if (idData > 0)
            {
                datasSel = datasSel.Where(x => x.Id == idData).ToList();
            }

            foreach (var data in datasSel.OrderBy(d => d.DataHoraInicio))
            {
                gfx.DrawString($"Data: {data.DataHoraInicio.ToShortDateString()} - Hora Início: {data.DataHoraInicio.ToShortTimeString()} - Hora Final: {data.DataHoraFim.ToShortTimeString()}", font10, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                comecaDados += 12;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);

                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, cabec, UserAppContext.Current.Usuario.Username,
                                                     comecaDados, filtro, cabecalho);

                }

                Dictionary<string, int> header = new Dictionary<string, int>
                    {
                        { "NOME COMPLETO", 25 },
                        { "CONGREGAÇÃO/IGREJA", 230 },
                        { "CARGO", 410 },
                        { "SITUAÇÃO", 520 }
                    };
                var fontArial8 = new XFont("Arial", 8, XFontStyle.Bold);
                var fontArial8Normal = new XFont("Arial", 8, XFontStyle.Regular);
                foreach (var h in header)
                    gfx.DrawString(h.Key, fontArial8, XBrushes.Black, new XRect(h.Value, comecaDados + 70, 0, 0));
                comecaDados += 12;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);

                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, cabec, UserAppContext.Current.Usuario.Username,
                                                     comecaDados, filtro, cabecalho);

                }

                var membros = _presencaAppService.ConsultarPresencaInscricoesDatas(idPresenca, data.Id).ToList();

                if (congregacao > 0)
                {
                    membros = membros.Where(x => x.CongregacaoId == congregacao).ToList();
                }

                if (situacao > 0)
                {
                    if (Enum.TryParse(situacao.ToString(), out SituacaoPresenca sit))
                    {
                        membros = membros.Where(x => x.Situacao == sit).ToList();
                    }
                }
                var font = new XFont("Arial", 10, XFontStyle.Regular);

                if (membros.Count == 0)
                {
                    gfx.DrawString("Não foi localizado inscrições para essa data.", font, XBrushes.Black, new XRect(25, comecaDados + 70, 0, 0));
                }
                else
                {
                    foreach (var mem in membros.OrderByDescending(m => m.Situacao).ThenBy(o => o.Nome))
                    {
                        var qtdLinhasPular = new List<int>
                            {
                                ImprimirLinha(gfx, mem.Nome, font, 25, 249, comecaDados + 70),
                                ImprimirLinha(gfx, mem.Igreja, font, 230, 379, comecaDados + 70),
                                ImprimirLinha(gfx, mem.Cargo, font, 410, 529, comecaDados + 70),
                                ImprimirLinha(gfx, mem.Situacao.GetDisplayAttributeValue(), font, 520, 580, comecaDados + 70)
                            };
                        comecaDados += (12 * qtdLinhasPular.Max());
                        if (comecaDados > 740)
                        {
                            page = document.AddPage();
                            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                            page.Size = PdfSharpCore.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            contaPagina += 1;
                            comecaDados = 10;
                            comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, cabec, UserAppContext.Current.Usuario.Username,
                                                             comecaDados, filtro, cabecalho);
                        }
                        if (!string.IsNullOrWhiteSpace(mem.Justificativa))
                        {
                            ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "JUSTIFICATIVA", mem.Justificativa, fontArial8, fontArial8Normal);
                            comecaDados += 15;
                            if (comecaDados > 740)
                            {
                                page = document.AddPage();
                                page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                                page.Size = PdfSharpCore.PageSize.A4;
                                gfx = XGraphics.FromPdfPage(page);
                                contaPagina += 1;
                                comecaDados = 10;
                                comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, cabec, UserAppContext.Current.Usuario.Username,
                                                                 comecaDados, filtro, cabecalho);
                            }
                        }
                    }
                    if (situacao == 0)
                    {
                        ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Quantidade de Inscritos", membros.Count().ToString());
                        ImprimirLinhaDescrCont(gfx, 200, comecaDados + 70, "Ausentes", membros.Count(m => m.Situacao == SituacaoPresenca.Ausente).ToString());
                        ImprimirLinhaDescrCont(gfx, 400, comecaDados + 70, "Presentes", membros.Count(m => m.Situacao == SituacaoPresenca.Presente).ToString());
                    }
                    else if (situacao == 1)
                    {
                        ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Quantidade de Presentes", membros.Count(m => m.Situacao == SituacaoPresenca.Presente).ToString());
                    }
                    else if (situacao == 2)
                    {
                        ImprimirLinhaDescrCont(gfx, 25, comecaDados + 70, "Quantidade de Ausentes", membros.Count(m => m.Situacao == SituacaoPresenca.Ausente).ToString());
                    }
                }
                comecaDados += 20;
                if (comecaDados > 740)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);

                    contaPagina += 1;
                    comecaDados = 10;
                    comecaDados = RelatorioCabecalho(page, gfx, contaPagina, nomecliente, cabec, UserAppContext.Current.Usuario.Username,
                                                     comecaDados, filtro, cabecalho);

                }
            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/pdf");
        }

        private FileStreamResult RelatorioPresencaFrequenciaExcel(string filtro,
                                                                  string nomeArquivo,
                                                                  int idPresenca,
                                                                  int idData,
                                                                  int congregacao,
                                                                  int situacao)
        {
            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("Frequencia");

            Dictionary<string, int> cabecalho = new Dictionary<string, int>()
                {
                    { "Nome", 1 },
                    { "Congregação/Igreja", 2 },
                    { "Cargo", 3 },
                    { "Situação", 4 },
                    { "Justificativa", 5 }
                };

            int linha = RelatorioCabecalhoExcel(workSheet, _configuration["ParametrosSistema:NomeCliente"].ToString(), "LISTA DE PRESENÇA - CURSOS/EVENTOS - Frequência",
                UserAppContext.Current.Usuario.Username, filtro, cabecalho);


            var listaPresenca = _presencaAppService.GetById(idPresenca, UserAppContext.Current.Usuario.Id);
            if (listaPresenca == null || listaPresenca.Id == 0)
                throw new Erro("Curso/Evento não localizado.");

            var datasSel = listaPresenca.Datas;
            if (idData > 0)
            {
                datasSel = datasSel.Where(x => x.Id == idData).ToList();
            }

            foreach (var data in datasSel.OrderBy(d => d.DataHoraInicio))
            {
                workSheet.Row(linha).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Row(linha).Style.Font.Bold = true;
                workSheet.Cells[linha, 1].Value = $"Data: {data.DataHoraInicio.ToShortDateString()} - Hora Início: {data.DataHoraInicio.ToShortTimeString()} - Hora Final: {data.DataHoraFim.ToShortTimeString()}";
                linha++;


                var membros = _presencaAppService.ConsultarPresencaInscricoesDatas(idPresenca, data.Id).ToList();
                if (congregacao > 0)
                {
                    membros = membros.Where(x => x.CongregacaoId == congregacao).ToList();
                }

                if (situacao > 0)
                {
                    if (Enum.TryParse(situacao.ToString(), out SituacaoPresenca sit))
                    {
                        membros = membros.Where(x => x.Situacao == sit).ToList();
                    }
                }
                var font = new XFont("Arial", 10, XFontStyle.Regular);

                if (membros.Count == 0)
                {
                    workSheet.Cells[linha, 1].Value = "Não foi localizado inscrições para essa data.";
                    linha++;
                }
                else
                {
                    foreach (var mem in membros.OrderByDescending(m => m.Situacao).ThenBy(o => o.Nome))
                    {
                        workSheet.Cells[linha, 1].Value = mem.Nome;
                        workSheet.Cells[linha, 2].Value = mem.Igreja;
                        workSheet.Cells[linha, 3].Value = mem.Cargo;
                        workSheet.Cells[linha, 4].Value = mem.Situacao.GetDisplayAttributeValue();
                        workSheet.Cells[linha, 5].Value = mem.Justificativa;
                        linha++;
                    }
                    linha++;
                    if (situacao == 0)
                    {
                        workSheet.Cells[linha, 1].Value = "Quantidade de Inscritos";
                        workSheet.Cells[linha, 2].Value = membros.Count().ToString();
                        linha++;
                        workSheet.Cells[linha, 1].Value = "Ausentes";
                        workSheet.Cells[linha, 2].Value = membros.Count(m => m.Situacao == SituacaoPresenca.Ausente).ToString();
                        linha++;
                        workSheet.Cells[linha, 1].Value = "Presentes";
                        workSheet.Cells[linha, 2].Value = membros.Count(m => m.Situacao == SituacaoPresenca.Presente).ToString();
                        linha++;
                    }
                    else if (situacao == 1)
                    {
                        workSheet.Cells[linha, 1].Value = "Quantidade de Presentes";
                        workSheet.Cells[linha, 2].Value = membros.Count(m => m.Situacao == SituacaoPresenca.Presente).ToString();
                        linha++;
                    }
                    else if (situacao == 2)
                    {
                        workSheet.Cells[linha, 1].Value = "Quantidade de Ausentes";
                        workSheet.Cells[linha, 2].Value = membros.Count(m => m.Situacao == SituacaoPresenca.Ausente).ToString();
                        linha++;
                    }
                }
                linha++;
            }

            for (int i = 1; i <= cabecalho.Count(); i++)
                workSheet.Column(i).AutoFit();

            var stream = new MemoryStream(excel.GetAsByteArray());
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/octet-stream", "xlsx");
        }
        #endregion

        #region Presenca - Frequencia Lista
        public FileStreamResult RelatorioPresencaFrequenciaLista(string congregacao,
                                                                 string dataInicio,
                                                                 string dataFinal,
                                                                 string cargos,
                                                                 string eventos)
        {
            try
            {
                int congrId = 0;
                if (!string.IsNullOrEmpty(congregacao))
                {
                    int.TryParse(congregacao, out congrId);
                }

                if (!DateTime.TryParse(dataInicio, out DateTime dtIni))
                    dtIni = DateTime.MinValue;

                if (!DateTime.TryParse(dataFinal, out DateTime dtFin))
                    dtFin = DateTime.MinValue;

                var relatorio = _relatoriosAppService.RelatorioFrequencia(congrId, dtIni, dtFin, cargos, eventos);
                if (!relatorio.Membros.Any())
                    throw new Erro("Não foram localizados registros para o Filtro selecionado.");

                string filtro = "";
                if (congrId > 0)
                {
                    var congreg = _congregacaoAppService.GetById(congrId, UserAppContext.Current.Usuario.Id).Nome;
                    filtro += $"Congregação: {congreg}";
                }
                else
                    filtro += "Congregação: Todas as Congregações";

                filtro += $" - Período: {dataInicio} a {dataFinal}";

                if (!string.IsNullOrEmpty(cargos))
                {
                    var filtroCargo = "";

                    var carg = _cargoService.GetAll(0);

                    var listaCargos = cargos.Split('_');
                    if (listaCargos.Length - 1 == carg.Count())
                    {
                        filtroCargo = "Todos";
                    }
                    else
                    {
                        foreach (var item in listaCargos)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                if (item == "0")
                                    filtroCargo += $", Membro/Visitante";
                                else
                                    filtroCargo += $", {carg.FirstOrDefault(A => A.Id == Convert.ToInt32(item)).Descricao}";
                            }

                        }
                        if (!string.IsNullOrEmpty(filtroCargo))
                            filtroCargo = filtroCargo.Substring(2, filtroCargo.Length - 2);
                    }
                    filtro += $" - Cargos: {filtroCargo}";
                }

                if (!string.IsNullOrEmpty(eventos))
                {
                    var filtroEvento = "";

                    var evt = _tipoEventoService.GetAll(0);

                    var listaEventos = eventos.Split('_');
                    if (listaEventos.Length == evt.Count())
                    {
                        filtroEvento = "Todos";
                    }
                    else
                    {
                        foreach (var item in listaEventos)
                        {
                            if (!string.IsNullOrEmpty(item))
                                filtroEvento += $", {evt.FirstOrDefault(A => A.Id == Convert.ToInt32(item)).Descricao}";
                        }
                        if (!string.IsNullOrEmpty(filtroEvento))
                            filtroEvento = filtroEvento.Substring(2, filtroEvento.Length - 2);
                    }
                    filtro += $" - Eventos: {filtroEvento}";
                }

                var nomeArquivo = $"Frequencia_Lista";
                return RelatorioPresencaFrequenciaListaExcel(relatorio, filtro, nomeArquivo, congrId == 0);
            }
            catch (Exception ex)
            {
                return TratarException(ex);
            }
        }

        private FileStreamResult RelatorioPresencaFrequenciaListaExcel(RelatorioFrequencia relatorio,
                                                                       string filtro,
                                                                       string nomeArquivo,
                                                                       bool congregacao)
        {
            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet workSheet = excel.Workbook.Worksheets.Add("Frequencia");

            Dictionary<string, int> cabecalho = new Dictionary<string, int>();
            var coluna = 1;
            if (congregacao)
            {
                cabecalho.Add("Congregação/Igreja", coluna++);
                cabecalho.Add("Nome", coluna++);
                cabecalho.Add("Cargo", coluna++);
            }
            else
            {
                cabecalho.Add("Nome", coluna++);
                cabecalho.Add("Cargo", coluna++);
            }

            foreach (var data in relatorio.Datas.OrderBy(o => o.Data))
            {
                cabecalho.Add(data.Data.Date.ToShortDateString(), coluna++);
            }
            cabecalho.Add("% Freq.", coluna++);

            int linha = RelatorioCabecalhoExcel(workSheet, _configuration["ParametrosSistema:NomeCliente"].ToString(), "LISTA DE PRESENÇA - CURSOS/EVENTOS - Frequência",
                UserAppContext.Current.Usuario.Username, filtro, cabecalho);

            /*RELAÇÃO DE NOMES*/
            if (relatorio.Membros.Count > 0)
            {
                foreach (var mem in relatorio.Membros.OrderBy(o => o.Nome))
                {
                    coluna = 1;
                    if (congregacao)
                        workSheet.Cells[linha, coluna++].Value = mem.Congregacao;
                    workSheet.Cells[linha, coluna++].Value = mem.Nome;
                    workSheet.Cells[linha, coluna++].Value = !string.IsNullOrEmpty(mem.Cargo) ? mem.Cargo : mem.Id > 0 ? "Membro" : "Visitante";
                    linha++;
                }
            }
            coluna = 1;

            workSheet.Cells[linha, coluna].Style.Font.Bold = true;
            workSheet.Cells[linha, coluna++].Value = "Total";

            workSheet.Cells[linha, coluna].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[linha, coluna].Style.Font.Bold = true;
            workSheet.Cells[linha, coluna++].Value = relatorio.Membros.Count();


            if (relatorio.Datas.Count > 0)
            {
                coluna = 3;
                if (congregacao)
                    coluna++;

                foreach (var data in relatorio.Datas.OrderBy(d => d.Data))
                {
                    linha -= relatorio.Membros.Count();
                    var total = 0;
                    foreach (var mem in relatorio.Membros.OrderBy(o => o.Nome))
                    {
                        var inscId = 0;
                        if (mem.Id > 0)
                        {
                            if (relatorio.InscricoesCurso.Any(a => a.MembroId == mem.Id && a.PresencaId == data.PresencaId))
                            {
                                inscId = relatorio.InscricoesCurso.FirstOrDefault(a => a.MembroId == mem.Id && a.PresencaId == data.PresencaId).Id;
                            }
                        }
                        else
                        {
                            if (relatorio.InscricoesCurso.Any(a => a.CPF == mem.CPF && a.PresencaId == data.PresencaId))
                            {
                                inscId = relatorio.InscricoesCurso.FirstOrDefault(a => a.CPF == mem.CPF && a.PresencaId == data.PresencaId).Id;
                            }
                        }

                        if (inscId > 0 && relatorio.Presencas.Any(p => p.InscricaoId == inscId && p.DataId == data.Id && p.Situacao == 1))
                        {
                            workSheet.Cells[linha, coluna].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            workSheet.Cells[linha, coluna].Value = "P";
                            total++;
                        }
                        linha++;
                    }
                    workSheet.Cells[linha, coluna].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[linha, coluna].Value = total;
                    workSheet.Cells[linha, coluna].Style.Font.Bold = true;
                    coluna++;
                }
            }
            linha -= relatorio.Membros.Count();
            var inscr = from l in relatorio.Membros
                        join i in relatorio.InscricoesCurso on l.Id equals i.MembroId
                        select new { MembroId = l.Id, InscricaoId = i.Id, CPF = l.CPF };

            foreach (var mem in relatorio.Membros.OrderBy(o => o.Nome))
            {
                var tot = 0;

                foreach (var m in inscr.Where(m => mem.Id > 0 ? m.MembroId == mem.Id : m.CPF == mem.CPF))
                {
                    if (m.InscricaoId > 0)
                    {
                        tot += relatorio.Presencas.Where(p => p.InscricaoId == m.InscricaoId).Count(p => p.Situacao == 1);
                    }
                }
                workSheet.Cells[linha, coluna].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                double perc = (tot * 100) / relatorio.Datas.Count();
                workSheet.Cells[linha, coluna].Value = $"{Math.Round(perc, 1)}%";
                linha++;
            }

            for (int i = 1; i <= cabecalho.Count(); i++)
                workSheet.Column(i).AutoFit();

            var stream = new MemoryStream(excel.GetAsByteArray());
            return GerarRelatorio(nomeArquivo, stream.ToArray(), "application/octet-stream", "xlsx");
        }
        #endregion


        #region Carteirinhas
        [HttpPost]
        public JsonResult ListarCarteirinhas(bool pesqMembro,
                                             List<string> congregacao,
                                             List<string> cargo,
                                             int idBatismo,
                                             bool imprimirObrVinc)
        {
            var relatorio = _relatoriosAppService.MembrosGrid(pesqMembro, congregacao, cargo, idBatismo, imprimirObrVinc);

            var JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(relatorio, Newtonsoft.Json.Formatting.None);
            return Json(JsonString);
        }


        [HttpPost]
        public FileStreamResult Carteirinhas([FromServices] ILogger<MembroController> logger,
                                             [FromServices] IMembroAppService membroAppService,
                                             [FromServices] IImpressaoMembroAppService carteirinhaAppService,
                                             [FromBody] CarteirinhaLoteVM carteirinha)
        {
            try
            {
                var carts = membroAppService.ListarCarteirinhaMembros(carteirinha.Membros.ToArray());
                if (carts.Any())
                {
                    var doc = carteirinhaAppService.GerarCarteirinha(carts, carteirinha.AtualizaDtValidade);

                    var stream = new MemoryStream();
                    var filename = $"CarteirinhaMembro.pdf";
                    doc.Save(stream, false);
                    return GerarArquivoDownload(stream.ToArray(), "application/pdf", filename, logger);
                }
                throw new Exception("Não foram encontrados membros para a emissão da Carteirinha");
            }
            catch (Exception ex)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                string messages = System.Text.Json.JsonSerializer.Serialize(new
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

        private FileStreamResult GerarArquivoDownload(byte[] relatorio,
                                                      string mimeType,
                                                      string filename,
                                                      ILogger<MembroController> logger)
        {
            try
            {
                var stream = new MemoryStream(relatorio);
                var fileStreamResult = new FileStreamResult(stream, mimeType)
                {
                    FileDownloadName = filename
                };
                Response.StatusCode = StatusCodes.Status200OK;
                return fileStreamResult;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                string messages = System.Text.Json.JsonSerializer.Serialize(new
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
        #endregion

        #endregion
    }
}