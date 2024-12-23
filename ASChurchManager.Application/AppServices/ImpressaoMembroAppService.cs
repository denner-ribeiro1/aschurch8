using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using QRCoder;
using SkiaSharp;

namespace ASChurchManager.Application.AppServices;

public class ImpressaoMembroAppService : IImpressaoMembroAppService
{
    #region Variaveis
    private readonly IMembroAppService membroAppService;
    private readonly IWebHostEnvironment env;
    private readonly IArquivosAzureAppService arqAzureService;
    private readonly IConfiguration configuration;
    #endregion

    #region Constantes
    private const double tamMaxH = 68.0;
    private const double tamMaxW = 55.0;
    #endregion

    #region Construtor
    public ImpressaoMembroAppService(IMembroAppService _membroAppService,
                                 IWebHostEnvironment _env,
                                 IArquivosAzureAppService _arqAzureService,
                                 IConfiguration _configuration)
    {
        membroAppService = _membroAppService;
        env = _env;
        arqAzureService = _arqAzureService;
        configuration = _configuration;
    }
    #endregion

    #region Metodos privados
    private static (byte[] FileContents, int Height, int Width) Resize(byte[] fileContents)
    {
        MemoryStream ms = new(fileContents);
        SKBitmap sourceBitmap = SKBitmap.Decode(ms);

        int height = sourceBitmap.Height;
        int width = sourceBitmap.Width;

        if (height > tamMaxH)
        {
            double percentual = height / tamMaxH;
            height = (int)(height / percentual);
            width = (int)(width / percentual);
            if (width > tamMaxW)
            {
                percentual = width / tamMaxW;
                height = (int)(height / percentual);
                width = (int)(width / percentual);
            }
            SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.High);
            SKImage scaledImage = SKImage.FromBitmap(scaledBitmap);
            SKData data = scaledImage.Encode();

            return (data.ToArray(), height, width);
        }
        return (fileContents, height, width);
    }

    private static string GerarIdBase64(int id)
    {
        string chave = $"RM{id.ToString().PadLeft(9, '0')}";
        byte[] chaveAsByte = System.Text.Encoding.ASCII.GetBytes(chave);
        return Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(chaveAsByte);
    }

    private static byte[] GenerateImage(string valor, IConfiguration _configuration)
    {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(valor, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new BitmapByteQRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(2);
        return qrCodeImage;
    }

    private static byte[] GenerateByteArray(string url, IConfiguration _configuration) => GenerateImage(url, _configuration);

    private static int ImprimirLinha(XGraphics gfx, string descr, string text, int colIni, int colFim, int altura)
    {
        var tam = colFim - colIni;
        var qtdLinha = 1;
        var fontRegular = new XFont("Arial", 10, XFontStyle.Regular);

        var strFinal = "";
        if (!string.IsNullOrEmpty(descr))
            strFinal += $"{descr}: ";
        if (!string.IsNullOrEmpty(text))
            strFinal += text;

        XSize size = gfx.MeasureString(strFinal, fontRegular);

        if (size.Width < tam)
        {
            gfx.DrawString(strFinal, fontRegular, XBrushes.Black, new XRect(colIni, altura, 0, 0));
        }
        else
        {
            var strAux = strFinal.Split(" ");
            var strRet = "";
            foreach (var item in strAux)
            {
                XSize sizeRet = gfx.MeasureString(strRet + item, fontRegular);
                if (sizeRet.Width <= tam)
                    strRet += item + " ";
                else
                {
                    gfx.DrawString(strRet.Trim(), fontRegular, XBrushes.Black, new XRect(colIni, altura, 0, 0));
                    qtdLinha++;
                    altura += 18;
                    strRet = item + " ";
                }
            }
            if (!string.IsNullOrWhiteSpace(strRet))
                gfx.DrawString(strRet, fontRegular, XBrushes.Black, new XRect(colIni, altura, 0, 0));
        }
        return qtdLinha;
    }
    private static void NovaPagina(ref PdfDocument document, ref PdfPage page, ref XGraphics gfx, ref int comecaDados, ref int contaPagina)
    {
        page = document.AddPage();
        page.Orientation = PageOrientation.Portrait;
        page.Size = PageSize.A4;
        gfx = XGraphics.FromPdfPage(page);
        contaPagina += 1;
        gfx.DrawRectangle(new XPen(XColor.FromName("Black")), 20, 20, 555, page.Height.Point - 40);
        comecaDados = 30;
    }
    #endregion

    #region Metodos públicos
    public PdfDocument GerarCarteirinha(IEnumerable<Carteirinha> carts, bool atualizaDtValidade = false)
    {
        var doc = new PdfDocument();
        doc.Info.Title = "Carteirinhas - Membro";
        doc.Info.Author = "Architect Systems";
        doc.Info.Subject = "Carteirinhas";
        doc.Info.Keywords = "PDFsharp, XGraphics";

        PdfPage page = doc.AddPage();
        page.Orientation = PageOrientation.Portrait;
        page.Size = PageSize.A4;
        XGraphics gfx = XGraphics.FromPdfPage(page);

        var webRoot = env.WebRootPath;

        if (carts.Any())
        {
            var posY = 0;
            foreach (var item in carts)
            {
                var cart = item;
                if (atualizaDtValidade && item.TipoCarteirinha != TipoCarteirinha.Membro)
                {
                    cart = membroAppService.CarteirinhaMembro(Convert.ToInt32(item.Id), true);
                }

                if (posY > 550)
                {
                    posY = 0;
                    page = doc.AddPage();
                    page.Orientation = PageOrientation.Portrait;
                    page.Size = PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                }

                /*Imagem da Carteirinha*/
                var tipoCarteira = "membro";
                switch (cart.TipoCarteirinha)
                {
                    case TipoCarteirinha.Cooperador:
                        tipoCarteira = "cooperador";
                        break;
                    case TipoCarteirinha.Diacono:
                        tipoCarteira = "diacono";
                        break;
                    case TipoCarteirinha.Presbitero:
                        tipoCarteira = "presbitero";
                        break;
                    case TipoCarteirinha.Evangelista:
                        tipoCarteira = "evangelista";
                        break;
                    case TipoCarteirinha.Pastor:
                        tipoCarteira = "pastor";
                        break;
                }

                string[] paths = { webRoot, "images", "carteirinhas", configuration["ParametrosSistema:NomeAcronimo"], $"{tipoCarteira}.jpg" };
                string pathImgFundo = Path.Combine(paths);
                XImage fundoImg = XImage.FromFile(pathImgFundo);

                double height = page.Width / fundoImg.PixelWidth * fundoImg.PixelHeight;
                double width = page.Width;
                height *= 0.86;
                width *= 0.905;
                gfx.DrawImage(fundoImg, 20, 20 + posY, width, height);

                if (cart.TipoCarteirinha == TipoCarteirinha.Membro)
                {
                    if (!string.IsNullOrWhiteSpace(cart.FotoUrl))
                    {
                        var arquivo = arqAzureService.DownloadFromUrl(cart.FotoUrl);
                        if (arquivo.StatusRetorno == TipoStatusRetorno.OK)
                        {
                            var (FileContents, Height, Width) = Resize(arquivo.BlobArray);
                            Stream stream = new MemoryStream(FileContents);

                            XImage xfoto1 = XImage.FromStream(() => stream);
                            var y = tamMaxH - Height;
                            var x = tamMaxW - Width;
                            gfx.DrawImage(xfoto1, x > 5 ? 225 + (x / 2) : 225, y > 5 ? 93 + (y / 2) + posY : 93 + posY, Width, Height);
                        }
                    }
                    var fontIdade = new XFont("Arial", 8, XFontStyle.Regular);
                    gfx.DrawString(cart.Id.ToString(), fontIdade, XBrushes.Black, new XRect(70, 155 + posY, 0, 0));
                    gfx.DrawString(cart.Nome, fontIdade, XBrushes.Black, new XRect(47, 178 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.NomePai))
                        gfx.DrawString(cart.NomePai, fontIdade, XBrushes.Black, new XRect(303, 58 + posY, 0, 0));
                    gfx.DrawString(cart.NomeMae, fontIdade, XBrushes.Black, new XRect(303, 77 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.Cidade))
                        gfx.DrawString(cart.Cidade, fontIdade, XBrushes.Black, new XRect(338, 99 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.Estado))
                        gfx.DrawString(cart.Estado, fontIdade, XBrushes.Black, new XRect(511, 99 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.DataNascimento))
                        gfx.DrawString(cart.DataNascimento, fontIdade, XBrushes.Black, new XRect(310, 122 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.EstadoCivil))
                        gfx.DrawString(cart.EstadoCivil, fontIdade, XBrushes.Black, new XRect(372, 122 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.RG))
                        gfx.DrawString(cart.RG, fontIdade, XBrushes.Black, new XRect(455, 122 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.DataBatismoAguas))
                        gfx.DrawString(cart.DataBatismoAguas, fontIdade, XBrushes.Black, new XRect(310, 145 + posY, 0, 0));

                    string chave64 = GerarIdBase64((int)cart.Id);
                    var imageAsByte = GenerateByteArray(chave64, configuration);
                    using var streamQr = new MemoryStream(imageAsByte);
                    if (streamQr != null)
                    {
                        XImage xfoto = XImage.FromStream(() => streamQr);
                        gfx.DrawImage(xfoto, 485, 130 + posY);
                    }

                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(cart.FotoUrl))
                    {
                        var arquivo = arqAzureService.DownloadFromUrl(cart.FotoUrl);
                        if (arquivo.StatusRetorno == Domain.Entities.TipoStatusRetorno.OK)
                        {
                            var (FileContents, Height, Width) = Resize(arquivo.BlobArray);
                            Stream stream = new MemoryStream(FileContents);

                            XImage xfoto1 = XImage.FromStream(() => stream);
                            var y = tamMaxH - Height;
                            var x = tamMaxW - Width;
                            gfx.DrawImage(xfoto1, x > 5 ? 225 + (x / 2) : 225, y > 5 ? 93 + (y / 2) + posY : 93 + posY, Width, Height);
                        }
                    }
                    var fontIdade = new XFont("Arial", 8, XFontStyle.Regular);
                    gfx.DrawString(cart.Id.ToString(), fontIdade, XBrushes.Black, new XRect(70, 155 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.DataValidadeCarteirinha))
                        gfx.DrawString(cart.DataValidadeCarteirinha, fontIdade, XBrushes.Black, new XRect(176, 155 + posY, 0, 0));
                    gfx.DrawString(cart.Nome, fontIdade, XBrushes.Black, new XRect(50, 178 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.NomePai))
                        gfx.DrawString(cart.NomePai, fontIdade, XBrushes.Black, new XRect(305, 57 + posY, 0, 0));
                    gfx.DrawString(cart.NomeMae, fontIdade, XBrushes.Black, new XRect(305, 76 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.Cidade))
                        gfx.DrawString(cart.Cidade, fontIdade, XBrushes.Black, new XRect(340, 98 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.Estado))
                        gfx.DrawString(cart.Estado, fontIdade, XBrushes.Black, new XRect(510, 98 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.DataNascimento))
                        gfx.DrawString(cart.DataNascimento, fontIdade, XBrushes.Black, new XRect(300, 121 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.EstadoCivil))
                        gfx.DrawString(cart.EstadoCivil, fontIdade, XBrushes.Black, new XRect(347, 121 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.RG))
                        gfx.DrawString(cart.RG, fontIdade, XBrushes.Black, new XRect(445, 121 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.DataBatismoAguas))
                        gfx.DrawString(cart.DataBatismoAguas, fontIdade, XBrushes.Black, new XRect(300, 142 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.LocalConsagracao))
                        gfx.DrawString(cart.LocalConsagracao, new XFont("Arial", 6, XFontStyle.Regular), XBrushes.Black, new XRect(362, 142 + posY, 0, 0));
                    if (!string.IsNullOrEmpty(cart.DataConsagracao))
                        gfx.DrawString(cart.DataConsagracao, new XFont("Arial", 6, XFontStyle.Regular), XBrushes.Black, new XRect(445, 142 + posY, 0, 0));

                    string chave64 = GerarIdBase64((int)cart.Id);
                    var imageAsByte = GenerateByteArray(chave64, configuration);
                    using var streamQr = new MemoryStream(imageAsByte);
                    if (streamQr != null)
                    {
                        XImage xfoto = XImage.FromStream(() => streamQr);
                        gfx.DrawImage(xfoto, 485, 130 + posY);
                    }
                }
                posY += 180;
            }
        }
        return doc;
    }

    public PdfDocument GerarFichaMembro(int membroId, bool imprimirCurso = false)
    {
        var mem = membroAppService.FichaMembro(membroId);
        if (!mem.Membro.Any())
            throw new Erro("Membro não encontrado.");

        var webRoot = env.WebRootPath;
        var document = new PdfDocument();
        document.Info.Title = "Ficha de Membros";
        document.Info.Author = "Architect Systems";
        document.Info.Subject = "Ficha de Membros";
        document.Info.Keywords = "PDFsharp, XGraphics";

        PdfPage page = document.AddPage();
        page.Orientation = PageOrientation.Portrait;
        page.Size = PageSize.A4;
        XGraphics gfx = XGraphics.FromPdfPage(page);

        int comecaDados = 25;
        var contaPagina = 1;
        var tamPag = 800;

        //Retangulo da Tela Toda
        gfx.DrawRectangle(new XPen(XColor.FromName("Black")), 20, 20, 555, page.Height.Point - 40);
        comecaDados += 2;

        var membro = mem.Membro.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(membro.FotoUrl))
        {
            var arquivo = arqAzureService.DownloadFromUrl(membro.FotoUrl);
            if (arquivo.StatusRetorno == TipoStatusRetorno.OK)
            {
                var (FileContents, Height, Width) = Resize(arquivo.BlobArray);
                Stream strFoto = new MemoryStream(FileContents);

                XImage xfoto1 = XImage.FromStream(() => strFoto);
                gfx.DrawImage(xfoto1, 25, 25, Width, Height);
            }
        }

        var caminhoImagem = configuration["ParametrosSistema:ImagemMini"].Split('/');
        List<string> pathLogo = [webRoot, .. caminhoImagem.Where(p => !string.IsNullOrEmpty(p))];

        string nomeLogo = Path.Combine(pathLogo.ToArray());

        XImage logo = XImage.FromFile(nomeLogo);
        gfx.DrawImage(logo, 480, comecaDados, 85, 80);

        comecaDados += 3;
        gfx.DrawString("FICHA DE MEMBRO", new XFont("Arial", 16, XFontStyle.Bold), XBrushes.Black, new XRect(0, comecaDados, page.Width, 0), new XStringFormat() { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center });
        comecaDados += 30;
        var linhasAPular = new List<int>
                {
                    ImprimirLinha(gfx, "RM", membro.Id.ToString(), 90,  150, comecaDados),
                    ImprimirLinha(gfx, "Membro", membro.Nome, 160, 480, comecaDados)
                };
        comecaDados += 18 * linhasAPular.Max();
        linhasAPular =
            [
                ImprimirLinha(gfx, "Congregação", membro.Congregacao, 90, 500, comecaDados)
            ];
        comecaDados += 18 * linhasAPular.Max();
        linhasAPular = new List<int>
                {
                    ImprimirLinha(gfx, "Tipo Membro", membro.TipoMembro, 90, 240, comecaDados),
                    ImprimirLinha(gfx, "Status", membro.Status, 250, 400, comecaDados),
                    ImprimirLinha(gfx, "ABEDABE", membro.MembroAbedabe, 400, 500, comecaDados)
                };
        comecaDados += 18 * linhasAPular.Max();
        linhasAPular = new List<int>
                {
                    ImprimirLinha(gfx, "Pai", !string.IsNullOrEmpty(membro.IdPai) && membro.IdPai != "0" ? $"{membro.IdPai} - {membro.NomePai}" : membro.NomePai, 25, 289, comecaDados),
                    ImprimirLinha(gfx, "Mãe", !string.IsNullOrEmpty(membro.IdMae) && membro.IdMae != "0" ? $"{membro.IdMae} - {membro.NomeMae}" : membro.NomeMae, 290, 570, comecaDados)
                };
        comecaDados += 18 * linhasAPular.Max();
        linhasAPular = new List<int>
                {
                    ImprimirLinha(gfx, "Estado Civil", membro.EstadoCivil, 25, 200, comecaDados)
                };
        if (!string.IsNullOrEmpty(membro.NomeConjuge))
            linhasAPular.Add(ImprimirLinha(gfx, "Cônjuge", !string.IsNullOrEmpty(membro.IdConjuge) && membro.IdConjuge != "0" ? $"{membro.IdConjuge} - {membro.NomeConjuge}" : membro.NomeConjuge, 210, 570, comecaDados));
        comecaDados += 18 * linhasAPular.Max();
        linhasAPular = new List<int>
                {
                    ImprimirLinha(gfx, "Sexo", membro.Sexo, 25, 110, comecaDados),
                    ImprimirLinha(gfx, "Data Nasc.", membro.DataNascimento, 115, 230, comecaDados),
                    ImprimirLinha(gfx, "Escolaridade", membro.Escolaridade, 235, 410, comecaDados),
                    ImprimirLinha(gfx, "Profissão", membro.Profissao, 410, 570, comecaDados)
                };
        comecaDados += 18 * linhasAPular.Max();
        linhasAPular = new List<int>
                {
                    ImprimirLinha(gfx, "Natural", $"{membro.NaturalidadeCidade} - {membro.NaturalidadeEstado}", 25, 350, comecaDados),
                    ImprimirLinha(gfx, "Nacionalidade", membro.Nacionalidade, 355, 550, comecaDados)
                };
        comecaDados += 5 * linhasAPular.Max();
        linhasAPular = new List<int>();
        gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);

        comecaDados += 12;
        gfx.DrawString("Documentos:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
        comecaDados += 18;
        ImprimirLinha(gfx, "CPF", membro.Cpf, 25, 205, comecaDados);
        ImprimirLinha(gfx, "RG", membro.RG, 205, 395, comecaDados);
        ImprimirLinha(gfx, "Orgão Emissor", membro.OrgaoEmissor, 395, 550, comecaDados);
        comecaDados += 5;
        gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);

        comecaDados += 12;
        gfx.DrawString("Contatos:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
        comecaDados += 18;
        ImprimirLinha(gfx, "Tel.Resid.", membro.TelefoneResidencial, 25, 395, comecaDados);
        ImprimirLinha(gfx, "Tel.Cel.", membro.TelefoneCelular, 395, 550, comecaDados);
        comecaDados += 18;
        ImprimirLinha(gfx, "E-mail", membro.Email, 25, 420, comecaDados);
        ImprimirLinha(gfx, "Tel.Com.", membro.TelefoneComercial, 430, 550, comecaDados);
        comecaDados += 5;
        gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);

        comecaDados += 12;
        gfx.DrawString("Endereço:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
        comecaDados += 18;
        linhasAPular = new List<int>
                {
                    ImprimirLinha(gfx, "Logradouro", membro.Logradouro, 25, 550, comecaDados)
                };
        comecaDados += 18 * linhasAPular.Max();
        ImprimirLinha(gfx, "Número", membro.Numero, 25, 200, comecaDados);
        ImprimirLinha(gfx, "Complemento.", membro.Complemento, 200, 550, comecaDados);
        comecaDados += 18;
        ImprimirLinha(gfx, "Bairro", membro.Bairro, 25, 250, comecaDados);
        if (membro.Pais == "Brasil")
        {
            ImprimirLinha(gfx, "Cidade/UF.", $"{membro.Cidade} - {membro.Estado}", 260, 550, comecaDados);
        }
        else
        {
            ImprimirLinha(gfx, "Cidade/Estado/Província.", $"{membro.Cidade} - {membro.Estado}", 260, 550, comecaDados);
        }

        comecaDados += 18;
        ImprimirLinha(gfx, "Pais", membro.Pais, 25, 250, comecaDados);
        if (membro.Pais == "Brasil")
        {
            ImprimirLinha(gfx, "CEP.", membro.Cep, 260, 550, comecaDados);
        }
        else
        {
            ImprimirLinha(gfx, "Código Postal.", membro.Cep, 260, 550, comecaDados);
        }

        comecaDados += 5;
        gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);

        comecaDados += 12;
        gfx.DrawString("Recebido Por:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
        comecaDados += 18;
        ImprimirLinha(gfx, "Recebido Por", membro.RecebidoPor, 25, 290, comecaDados);
        ImprimirLinha(gfx, "Data Recepção", membro.DataRecepcao, 300, 550, comecaDados);
        comecaDados += 5;
        gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);

        comecaDados += 12;
        gfx.DrawString("Batismo:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
        comecaDados += 18;
        ImprimirLinha(gfx, "Data Batismo Águas", membro.DataBatismoAguas, 25, 290, comecaDados);
        ImprimirLinha(gfx, "Batismo Espírito Santo", membro.BatimoEspiritoSanto, 300, 550, comecaDados);
        comecaDados += 5;
        gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);

        comecaDados += 12;
        gfx.DrawString("Situação:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
        comecaDados += 18;
        gfx.DrawString("Situação:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
        gfx.DrawString("Data:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(170, comecaDados, 0, 0));
        gfx.DrawString("Observação:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(230, comecaDados, 0, 0));
        comecaDados += 18;

        if (mem.Situacao.Count != 0)
        {
            foreach (var item in mem.Situacao)
            {
                linhasAPular = new List<int>
                        {
                            ImprimirLinha(gfx, "", item.Situacao, 25, 170, comecaDados),
                            ImprimirLinha(gfx, "", item.Data, 170, 230, comecaDados),
                            ImprimirLinha(gfx, "", item.Observacao, 240, 550, comecaDados)
                        };
                comecaDados += 18 * linhasAPular.Max();

                if (comecaDados > tamPag)
                    NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);
            }
            comecaDados -= 13;
            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);
        }
        else
        {
            ImprimirLinha(gfx, "Não há Situações cadastradas para o Membro ", "", 25, 500, comecaDados);
            comecaDados += 5;
            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);
        }
        if (comecaDados > tamPag)
            NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);

        comecaDados += 12;
        gfx.DrawString("Cargos:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
        comecaDados += 18;
        if (comecaDados > tamPag)
            NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);
        gfx.DrawString("Cargos:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
        gfx.DrawString("Local de Consagração:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(120, comecaDados, 0, 0));
        gfx.DrawString("Data:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(320, comecaDados, 0, 0));
        gfx.DrawString("Confradesp Nº:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(380, comecaDados, 0, 0));
        gfx.DrawString("CGADB Nº:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(480, comecaDados, 0, 0));
        comecaDados += 18;
        if (comecaDados > tamPag)
            NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);

        if (mem.Cargo.Count != 0)
        {
            foreach (var item in mem.Cargo)
            {
                linhasAPular = new List<int>
                        {
                            ImprimirLinha(gfx, "", item.Cargo, 25, 119, comecaDados),
                            ImprimirLinha(gfx, "", item.LocalConsagracao, 120, 319, comecaDados),
                            ImprimirLinha(gfx, "", item.DataCargo, 320, 379, comecaDados),
                            ImprimirLinha(gfx, "", item.Confradesp, 380, 479, comecaDados),
                            ImprimirLinha(gfx, "", item.CGADB, 480, 550, comecaDados)
                        };
                comecaDados += 18 * linhasAPular.Max();

                if (comecaDados > tamPag)
                    NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);
            }
            comecaDados -= 13;
            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);
        }
        else
        {
            ImprimirLinha(gfx, "Não há Cargos cadastradas para o Membro ", "", 25, 500, comecaDados);
            comecaDados += 5;
            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);
        }
        if (comecaDados > tamPag)
            NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);

        comecaDados += 12;
        gfx.DrawString("Observações:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
        comecaDados += 18;
        if (comecaDados > tamPag)
            NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);
        gfx.DrawString("Observação:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
        gfx.DrawString("Data:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(300, comecaDados, 0, 0));
        gfx.DrawString("Resp.p/Cadastro:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(360, comecaDados, 0, 0));
        comecaDados += 18;
        if (comecaDados > tamPag)
            NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);

        if (mem.Observacao.Count != 0)
        {
            foreach (var item in mem.Observacao)
            {
                linhasAPular = new List<int>
                        {
                            ImprimirLinha(gfx, "", item.Observacao, 25, 299, comecaDados),
                            ImprimirLinha(gfx, "", item.DataCadastro, 300, 359, comecaDados),
                            ImprimirLinha(gfx, "", item.Nome, 360, 550, comecaDados)
                        };
                comecaDados += 18 * linhasAPular.Max();

                if (comecaDados > tamPag)
                    NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);
            }
            comecaDados -= 13;
            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);
        }
        else
        {
            ImprimirLinha(gfx, "Não há Observações cadastradas para o Membro ", "", 25, 500, comecaDados);
            comecaDados += 5;
            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);
        }
        if (comecaDados > tamPag)
            NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);

        if (imprimirCurso)
        {
            comecaDados += 12;
            gfx.DrawString("Cursos/Eventos:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
            comecaDados += 18;
            if (comecaDados > tamPag)
                NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);

            if (mem.Presenca.Any())
            {
                foreach (var item in mem.Presenca)
                {
                    gfx.DrawString($"{item.Id} - {item.Descricao}", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
                    comecaDados += 18;
                    if (comecaDados > tamPag)
                        NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);

                    foreach (var data in item.Datas)
                    {
                        var sit = "";
                        switch (data.Situacao)
                        {
                            case "0": sit = "Não Registrado"; break;
                            case "1": sit = "Presente"; break;
                            case "2": sit = "Ausente"; break;
                        }
                        if (data.Situacao == "2")
                        {
                            linhasAPular = new List<int>
                                    {
                                        ImprimirLinha(gfx, "Data", data.DataHoraInicio, 35, 150, comecaDados),
                                        ImprimirLinha(gfx, "Situação", sit, 120, 280, comecaDados),
                                        ImprimirLinha(gfx, "Justificativa", data.Justificativa, 290, 570, comecaDados),
                                    };
                        }
                        else
                        {
                            linhasAPular = new List<int>
                                    {
                                        ImprimirLinha(gfx, "Data", data.DataHoraInicio, 35, 150, comecaDados),
                                        ImprimirLinha(gfx, "Situação", sit, 120, 280, comecaDados),
                                    };
                        }

                        comecaDados += 18 * linhasAPular.Max();
                        if (comecaDados > tamPag)
                            NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);
                    }

                }
                comecaDados -= 13;
                gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);
            }
            else
            {
                ImprimirLinha(gfx, "Não há Cursos/Eventos cadastrados para o Membro ", "", 25, 500, comecaDados);
                comecaDados += 5;
                gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);
            }
            if (comecaDados > tamPag)
                NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);

        }

        comecaDados += 12;
        gfx.DrawString("Histórico de Transferências:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
        comecaDados += 18;
        if (comecaDados > tamPag)
            NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);
        gfx.DrawString("Congregação de Origem:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(25, comecaDados, 0, 0));
        gfx.DrawString("Congregação de Destino:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(265, comecaDados, 0, 0));
        gfx.DrawString("Data:", new XFont("Arial", 10, XFontStyle.Bold), XBrushes.Black, new XRect(505, comecaDados, 0, 0));
        comecaDados += 18;
        if (comecaDados > tamPag)
            NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);

        if (mem.Historico.Any())
        {
            foreach (var item in mem.Historico)
            {
                linhasAPular = new List<int>
                        {
                            ImprimirLinha(gfx, "", item.CongregacaoOrigem, 25, 264, comecaDados),
                            ImprimirLinha(gfx, "", item.CongregacaoDestino, 265, 504, comecaDados),
                            ImprimirLinha(gfx, "", item.DataDaTransferencia, 505, 560, comecaDados)
                        };
                comecaDados += 18 * linhasAPular.Max();
                if (comecaDados > tamPag)
                    NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);
            }
            comecaDados -= 13;
            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);
        }
        else
        {
            ImprimirLinha(gfx, "", "Não há Transferências cadastradas para o Membro", 25, 500, comecaDados);
            comecaDados += 5;
            gfx.DrawLine(new XPen(XColor.FromName("Black")), 20, comecaDados, 575, comecaDados);
        }
        if (comecaDados > tamPag)
            NovaPagina(ref document, ref page, ref gfx, ref comecaDados, ref contaPagina);

        return document;

    }
    #endregion
}
