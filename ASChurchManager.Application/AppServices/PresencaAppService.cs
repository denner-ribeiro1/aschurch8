using ASChurchManager.Application.Interfaces;
using ASChurchManager.Application.Lib;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces.Repository;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASChurchManager.Application.AppServices
{
    public class PresencaAppService : BaseAppService<Presenca>, IPresencaAppService
    {
        private readonly IPresencaRepository _presencaRepository;
        private readonly IArquivosAzureAppService _arqAzureService;
        private readonly IMembroAppService _membroAppService;
        private readonly IUsuarioAppService _usuarioService;
        private readonly ICongregacaoAppService _congregacaoService;


        public PresencaAppService(IPresencaRepository presencaRepository
            , IArquivosAzureAppService arquivosAzureAppService
            , IMembroAppService membroAppService
            , IUsuarioAppService usuarioService
            , ICongregacaoAppService congregacaoService)
            : base(presencaRepository)
        {
            _presencaRepository = presencaRepository;
            _arqAzureService = arquivosAzureAppService;
            _membroAppService = membroAppService;
            _usuarioService = usuarioService;
            _congregacaoService = congregacaoService;
        }


        private bool ValidarConfiguracao(Presenca entity)
        {
            if (entity.Datas.Any(a => a.DataHoraInicio > a.DataHoraFim))
                throw new Erro($"Existe(m) Data(s) com a Data Hora Fim maior que a Data Hora Inicial {entity.Datas.FirstOrDefault(a => a.DataHoraInicio > a.DataHoraFim)}");
            if (entity.CongregacaoId == 0)
                throw new Erro($"Congregação é de preenchimento obrigatório");
            if (entity.DataMaxima == DateTime.MinValue)
                throw new Erro($"Data Máxima é de preenchimento obrigatório");
            if (string.IsNullOrWhiteSpace(entity.Descricao))
                throw new Erro($"Descrição é de preenchimento obrigatório");
            if (entity.TipoEventoId == 0)
                throw new Erro($"Tipo de Evento é de preenchimento obrigatório");

            return true;
        }
        public override long Add(Presenca entity, long usuarioID = 0)
        {
            try
            {
                ValidarConfiguracao(entity);
                entity.Id = base.Add(entity);
                return entity.Id;
            }
            catch (Erro ex)
            {
                entity.PreencherStatusErro(ex);
                return 0;
            }

            catch (Exception ex)
            {
                entity.PreencherStatusErro(ex);
                return 0;
            }
        }

        public override int Delete(Presenca entity, long usuarioID = 0)
        {
            return Delete(entity.Id, usuarioID);
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            var ret = base.Delete(id, usuarioID);
            var container = $"presenca{id}";
            _arqAzureService.DeleteSpecificContainerAsync(container).Wait();
            return ret;
        }
        public IEnumerable<Presenca> ListarPresencaPaginado(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor, bool naoMembro, long usuarioID)
        {
            return _presencaRepository.ListarPresencaPaginado(pageSize, rowStart, out rowCount, sorting, campo, valor, naoMembro, usuarioID);
        }

        public IEnumerable<Presenca> ListarPresencaEmAberto(long usuarioID)
        {
            return _presencaRepository.ListarPresencaEmAberto(usuarioID);
        }

        private string GerarIdBase64(int id)
        {
            string chave = $"ID{id.ToString().PadLeft(9, '0')}";
            byte[] chaveAsByte = System.Text.Encoding.ASCII.GetBytes(chave);
            return Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(chaveAsByte);
        }
        public int SalvarInscricao(PresencaMembro entity)
        {
            var id = _presencaRepository.SalvarInscricao(entity);
            return id;
        }

        private static byte[] GenerateImage(string valor)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(valor, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new BitmapByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(2);
            return qrCodeImage;
        }

        private static byte[] GenerateByteArray(string url)
        {
            return GenerateImage(url);
        }

        public List<PresencaMembro> ConsultarPresencaInscricaoPorPresencaId(long idPresenca, long usuarioID)
        {
            return _presencaRepository.ConsultarPresencaInscricaoPorPresencaId(idPresenca, usuarioID);
        }

        public async Task<int> DeleteInscricaoAsync(int id)
        {
            await _arqAzureService.DeleteSpecificFileAsync($"qrcode_{id}.png", "presencaqrcode");
            var ret = _presencaRepository.DeleteInscricaoAsync(id).Result;
            return ret;
        }

        public PresencaMembro ConsultarPresencaInscricao(long idPresenca, long idMembro, string cpf, long usuarioID)
        {
            return _presencaRepository.ConsultarPresencaInscricao(idPresenca, idMembro, cpf, usuarioID);
        }

        public int AtualizarPagoInscricao(int id, bool pago)
        {
            return _presencaRepository.AtualizarPagoInscricao(id, pago);
        }

        public async Task<List<PresencaMembro>> LerArquivoExcelAsync(IFormFile file, int id, long usuarioID)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new Erro("Arquivo não selecionado");
                }
                var lpresenca = _presencaRepository.ConsultarPresencaInscricaoPorPresencaId(id, usuarioID);

                var presenca = new List<PresencaMembro>();
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream).ConfigureAwait(false);

                    using (var package = new ExcelPackage(memoryStream))
                    {
                        if (package.Workbook.Worksheets.Count() == 0){
                            throw new Erro($"Falha ao ler o arquivo {file.FileName}. Arquivo no formato inválido.");
                        }
                        var totalRows = package.Workbook.Worksheets[0].Dimension?.Rows;

                        ValidarHeader(package);

                        for (int j = 2; j <= totalRows.Value; j++)
                        {
                            var membro = new PresencaMembro
                            {
                                Id = j,
                                MembroId = 0
                            };

                            if (package.Workbook.Worksheets[0].Cells[j, 1].Value != null)
                                membro.Nome = package.Workbook.Worksheets[0].Cells[j, 1].Value.ToString();

                            if (package.Workbook.Worksheets[0].Cells[j, 2].Value != null)
                                membro.CPF = package.Workbook.Worksheets[0].Cells[j, 2].Value.ToString();

                            if (package.Workbook.Worksheets[0].Cells[j, 3].Value != null)
                                membro.Igreja = package.Workbook.Worksheets[0].Cells[j, 3].Value.ToString();

                            if (package.Workbook.Worksheets[0].Cells[j, 4].Value != null)
                                membro.Cargo = package.Workbook.Worksheets[0].Cells[j, 4].Value.ToString();

                            membro.OK = true;
                            membro.NomeArquivo = file.FileName;
                            presenca.Add(membro);
                        }
                    }
                }
                //Validações
                // Verificando se o CPF já está cadastrado no curso, pertence a um membro e já validando status e tipo
                var config = GetById(id, usuarioID);
                var congrSede = _congregacaoService.GetSede();
                var congrUsuario = _usuarioService.GetById(usuarioID, 0).Congregacao.Id;

                foreach (var item in presenca.Where(p => p.OK))
                {
                    if (lpresenca.Any(p => p.PresencaId == id && p.CPF == item.CPF))
                    {
                        item.OK = false;
                        item.PreencherStatusErro(999, $"Linha {item.Id} - CPF {item.CPF} já cadastrado para o Curso/Evento!");
                    }
                    else
                    {
                        var membro = _membroAppService.GetByCPF(item.CPF);

                        if (membro != null && membro.Id > 0)
                        {
                            /*Usuário não é usuario da congregação Sede*/
                            if (congrUsuario != congrSede.Id)
                            {
                                if (membro.Congregacao.Id != congrUsuario && membro.Congregacao.CongregacaoResponsavelId != congrUsuario) 
                                {
                                    item.OK = false;
                                    item.PreencherStatusErro(999, $"Linha {item.Id} - Membro vinculado a Congregação {membro.Congregacao.Nome}!");
                                }
                            }

                            if (membro.Status != Status.Ativo)
                            {
                                item.OK = false;
                                item.PreencherStatusErro(999, $"Linha {item.Id} - Membro não Ativo!");
                            }

                            if (item.OK && membro.TipoMembro != TipoMembro.Membro)
                            {
                                item.OK = false;
                                item.PreencherStatusErro(999, $"Linha {item.Id} - Membro Inválido!");
                            }

                            if (item.OK)
                            {
                                item.MembroId = (int)membro.Id;
                                item.Nome = membro.Nome;
                                item.Igreja = membro.Congregacao.Nome;
                                item.CongregacaoId = (int)membro.Congregacao.Id;

                                var cargos = _membroAppService.ListarCargosMembro(membro.Id);
                                if (cargos.Any())
                                {
                                    var cargo = cargos.FirstOrDefault(a => a.DataCargo == cargos.Max(d => d.DataCargo)).TipoCarteirinha;
                                    item.Cargo = cargo.GetDisplayAttributeValue();
                                }
                            }
                        }
                    }

                    if (item.OK && !config.NaoMembros && item.MembroId == 0)
                    {
                        item.OK = false;
                        item.PreencherStatusErro(999, $"Linha {item.Id} - Inscrição de Não Membros não permitido para o Evento!");
                    }

                    if (config.ExclusivoCongregacao && item.MembroId > 0 && item.CongregacaoId != config.CongregacaoId)
                    {
                        item.OK = false;
                        item.PreencherStatusErro(999, $"Linha {item.Id} - Evento não permite a inscrição de membros não pertecentes a Congregação!!");
                    }
                }


                // VISITANTE SEM NOME
                foreach (var item in presenca.Where(p => string.IsNullOrWhiteSpace(p.Nome) && p.OK && p.MembroId == 0))
                {
                    item.OK = false;
                    item.PreencherStatusErro(999, $"Linha {item.Id} - NOME não preenchido!");
                }
                // VISITANTE SEM CPF
                foreach (var item in presenca.Where(p => string.IsNullOrWhiteSpace(p.CPF) && p.OK && p.MembroId == 0))
                {
                    item.OK = false;
                    item.PreencherStatusErro(999, $"Linha {item.Id} - CPF não preenchido!");
                }
                //VALIDAR CPF
                foreach (var item in presenca.Where(p => p.OK))
                {
                    if (!ValidarCPF(item.CPF))
                    {
                        item.OK = false;
                        item.PreencherStatusErro(999, $"Linha {item.Id} - CPF {item.CPF} inválido!");
                    }
                }

                foreach (var item in presenca.GroupBy(i => i.CPF).Where(c => c.Count() > 1).Select(g => g.Key))
                {
                    var pres = presenca.FirstOrDefault(p => p.CPF == item);
                    pres.OK = false;
                    pres.PreencherStatusErro(999, $"Encontrado para o CPF {item} mais que um registro no arquivo!");
                }

                return presenca;
            }
            catch (Exception ex)
            {
                var ret = new List<PresencaMembro>();
                var pres = new PresencaMembro();
                pres.PreencherStatusErro(ex);
                ret.Add(pres);
                return ret;
            }
        }

        private bool ValidarCPF(string value)
        {
            if (value != null)
            {
                var valueValidLength = 11;
                var maskChars = new[] { ".", "-" };
                var multipliersForFirstDigit = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                var multipliersForSecondDigit = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

                var mod11 = new Mod11();
                return mod11.IsValid(value.ToString(), valueValidLength, maskChars, multipliersForFirstDigit, multipliersForSecondDigit);
            }
            return false;
        }

        private bool ValidarHeader(ExcelPackage package)
        {
            if (package.Workbook.Worksheets[0].Cells[1, 1].Value != null &&
                package.Workbook.Worksheets[0].Cells[1, 1].Value.ToString().ToUpper() != "NOME")
                throw new Erro("Cabeçalho do Arquivo Inválido. Coluna NOME não encontrada");

            if (package.Workbook.Worksheets[0].Cells[1, 2].Value != null &&
                package.Workbook.Worksheets[0].Cells[1, 2].Value.ToString().ToUpper() != "CPF")
                throw new Erro("Cabeçalho do Arquivo Inválido. Coluna CPF não encontrada");

            if (package.Workbook.Worksheets[0].Cells[1, 3].Value != null &&
                package.Workbook.Worksheets[0].Cells[1, 3].Value.ToString().ToUpper() != "IGREJA")
                throw new Erro("Cabeçalho do Arquivo Inválido. Coluna IGREJA não encontrada");

            if (package.Workbook.Worksheets[0].Cells[1, 4].Value != null &&
                package.Workbook.Worksheets[0].Cells[1, 4].Value.ToString().ToUpper() != "CARGO")
                throw new Erro("Cabeçalho do Arquivo Inválido. Coluna CARGO não encontrada");

            return true;
        }

        public int SalvarInscricaoArquivo(string nomeArquivo)
        {
            return _presencaRepository.SalvarInscricaoArquivo(nomeArquivo);
        }

        public List<PresencaDatas> ListarPresencaDatas(long idPresenca)
        {
            return _presencaRepository.ListarPresencaDatas(idPresenca);
        }

        public int AtualizarStatusData(int idData, StatusPresenca status)
        {
            return _presencaRepository.AtualizarStatusData(idData, status);
        }

        public IEnumerable<Presenca> ConsultarPresencaPorStatusData(int id, StatusPresenca status)
        {
            return _presencaRepository.ConsultarPresencaPorStatusData(id, status);
        }

        public int SalvarPresencaInscricaoDatas(int idInscricao, int idData, SituacaoPresenca situacao, TipoRegistro tipo, string justificativa = "")
        {
            return _presencaRepository.SalvarPresencaInscricaoDatas(idInscricao, idData, situacao, tipo, justificativa);
        }

        public PresencaMembro ConsultarPresencaInscricao(long idInscricao)
        {
            return _presencaRepository.ConsultarPresencaInscricao(idInscricao);
        }

        public PresencaMembro ConsultarPresencaInscricaoDatas(long idInscricao, int idData)
        {
            return _presencaRepository.ConsultarPresencaInscricaoDatas(idInscricao, idData);
        }

        public List<PresencaMembro> ConsultarPresencaInscricoesDatas(int idPresenca, int idData)
        {
            return _presencaRepository.ConsultarPresencaInscricoesDatas(idPresenca, idData);
        }
        private int ImprimirLinha(XGraphics gfx, string text, XFont font, int colIni, int colFim, int altura, XStringFormat xStringFormat = null)
        {
            var tam = colFim - colIni;
            var qtdLinha = 1;
            if (string.IsNullOrWhiteSpace(text))
                text = "";

            XSize size = gfx.MeasureString(text, font);

            if (size.Width < tam)
            {
                if (xStringFormat == null)
                    gfx.DrawString(text, font, XBrushes.Black, new XRect(colIni, altura, tam, 0));
                else
                    gfx.DrawString(text, font, XBrushes.Black, new XRect(colIni, altura, tam, 0), xStringFormat);
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
                        if (xStringFormat == null)
                            gfx.DrawString(strRet.Trim(), font, XBrushes.Black, new XRect(colIni, altura, tam, 0));
                        else
                            gfx.DrawString(strRet.Trim(), font, XBrushes.Black, new XRect(colIni, altura, tam, 0), xStringFormat);
                        qtdLinha++;
                        altura += 20;
                        strRet = item + " ";
                    }

                }
                if (!string.IsNullOrWhiteSpace(strRet))
                {
                    if (xStringFormat == null)
                        gfx.DrawString(strRet, font, XBrushes.Black, new XRect(colIni, altura, tam, 0));
                    else
                        gfx.DrawString(strRet, font, XBrushes.Black, new XRect(colIni, altura, tam, 0), xStringFormat);
                }
            }
            return qtdLinha;
        }
        private void GerarEtiqueta(XGraphics gfx, string congrecacao, string nome, string cargo, long id, long membroId, bool etiqueta1, int linha, int margemTop, int margemEsq)
        {
            var fontArial16 = new XFont("Arial", 16, XFontStyle.Bold);
            var fontArial10 = new XFont("Arial", 10, XFontStyle.Bold);
            var fontArial14 = new XFont("Arial", 14, XFontStyle.Bold);
            var tamColuna = 190;

            var center = new XStringFormat
            {
                Alignment = XStringAlignment.Center
            };

            int iniEt = 50 + margemEsq;
            if (!etiqueta1)
                iniEt = 340 + margemEsq;
            int posIni = 0;

            // Linha do Nome
            switch (linha)
            {
                case 0:
                    posIni = 95;
                    break;
                case 1:
                    posIni = 265;
                    break;
                case 2:
                    posIni = 435;
                    break;
                case 3:
                    posIni = 605;
                    break;
            }
            posIni += margemTop;
            var qtd = ImprimirLinha(gfx, string.IsNullOrWhiteSpace(nome) ? "" : nome.Trim(), fontArial16, iniEt, iniEt + tamColuna, posIni, center);

            // Linha do Cargo
            posIni += 20 * qtd;
            if (!string.IsNullOrWhiteSpace(cargo))
            {
                ImprimirLinha(gfx, $"({cargo.Trim()})", fontArial14, iniEt, iniEt + tamColuna, posIni, center);
            }
            else
            {
                if (membroId > 0)
                    ImprimirLinha(gfx, $"(Membro)", fontArial14, iniEt, iniEt + tamColuna, posIni, center);
                else
                    ImprimirLinha(gfx, $"(Visitante)", fontArial14, iniEt, iniEt + tamColuna, posIni, center);
            }

            // Linha da Congregação
            posIni = 0;
            switch (linha)
            {
                case 0:
                    posIni = 190;
                    break;
                case 1:
                    posIni = 360;
                    break;
                case 2:
                    posIni = 530;
                    break;
                case 3:
                    posIni = 700;
                    break;
            }
            posIni += margemTop;
            if (!string.IsNullOrWhiteSpace(congrecacao))
                ImprimirLinha(gfx, congrecacao.Trim(), fontArial10, iniEt, iniEt + tamColuna, posIni, center);

            int iniQr = 225 + margemEsq;
            if (!etiqueta1)
                iniQr = 495 + margemEsq;

            // Linha do QrCode
            int posIniQrCode = 0;
            switch (linha)
            {
                case 0:
                    posIniQrCode = 135;
                    break;
                case 1:
                    posIniQrCode = 305;
                    break;
                case 2:
                    posIniQrCode = 475;
                    break;
                case 3:
                    posIniQrCode = 645;
                    break;
            }
            posIniQrCode += margemTop;

            string chave64 = GerarIdBase64((int)id);
            var imageAsByte = GenerateByteArray(chave64);
            using var stream = new MemoryStream(imageAsByte);
            if (stream != null)
            {
                XImage xfoto = XImage.FromStream(() => stream);
                gfx.DrawImage(xfoto, iniQr, posIniQrCode);
            }

        }

        public byte[] Etiquetas(int idPresenca, int congregacao, int tipo, int membroId, string cpf, string posicao, int margemTop, int margemEsq, long usuarioID)
        {
            var inscritos = _presencaRepository.ConsultarPresencaEtiquetas(idPresenca, congregacao, tipo, membroId, cpf, usuarioID);
            if (inscritos.Count == 0)
                throw new Erro("Não foram localizadas inscrições para o Filtro selecionado");

            var presenca = _presencaRepository.GetById(idPresenca, 0);

            var document = new PdfDocument();
            document.Info.Title = "Etiquetas";
            document.Info.Author = "Architect Systems";
            document.Info.Subject = "Etiquetas";
            document.Info.Keywords = "PDFsharp, XGraphics";

            PdfPage page = document.AddPage();
            page.Orientation = PdfSharpCore.PageOrientation.Portrait;
            page.Size = PdfSharpCore.PageSize.A4;
            XGraphics gfx = XGraphics.FromPdfPage(page);


            gfx.DrawString(presenca.Descricao, new XFont("Arial", 7, XFontStyle.Regular), XBrushes.Black, new XRect(10, 10, 600, 0), new XStringFormat { Alignment = XStringAlignment.Center });
            var linha = 0;
            bool etiquetaEsq = true;
            if (!string.IsNullOrWhiteSpace(posicao) && posicao.Length == 2)
            {
                int.TryParse(posicao.Substring(0, 1), out linha);
                linha--;

                etiquetaEsq = posicao.Substring(1, 1) == "E";
            }
            foreach (var item in inscritos)
            {
                if (linha > 3)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    linha = 0;
                    gfx.DrawString(presenca.Descricao, new XFont("Arial", 7, XFontStyle.Regular), XBrushes.Black, new XRect(10, 10, 600, 0), new XStringFormat { Alignment = XStringAlignment.Center });

                    etiquetaEsq = true;
                }

                GerarEtiqueta(gfx, item.Igreja, item.Nome, item.Cargo, item.Id, item.MembroId, etiquetaEsq, linha, margemTop, margemEsq);
                etiquetaEsq = !etiquetaEsq;
                if (etiquetaEsq)
                    linha++;
            }

            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }

        public List<PresencaMembro> ConsultarPresencaEtiquetas(int idInscricao, int idCongregacao, int tipo, int membroId, string cpf, long usuarioId)
        {
            return _presencaRepository.ConsultarPresencaEtiquetas(idInscricao, idCongregacao, tipo, membroId, cpf, usuarioId);
        }

        public List<ArquivoAzure> ListaArquivos(int idPresenca)
        {
            var container = $"presenca{idPresenca}";
            var lista = _arqAzureService.ListaArquivos(container);
            return lista;
        }

        public Task<RetornoAzure> DownloadArquivo(int idPresenca, string nomeArquivo)
        {
            var container = $"presenca{idPresenca}";
            return _arqAzureService.DownloadFromStorageAsync(nomeArquivo, container);
        }

        public Task<bool> DeleteFilesAsync(int idPresenca, string nomeArquivo)
        {
            var container = $"presenca{idPresenca}";
            return _arqAzureService.DeleteSpecificFileAsync(nomeArquivo, container);
        }

        public string UploadFileToStorage(int idPresenca, IFormFile file)
        {
            var container = $"presenca{idPresenca}";
            var nome = file.FileName.Trim('\"');
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();
            return _arqAzureService.UploadArrayByteAsync(fileBytes, nome, container).Result;
        }

        public List<PresencaDatas> ListarPresencaDatasEmAndamento()
        {
            return _presencaRepository.ListarPresencaDatasEmAndamento();
        }

        public IEnumerable<PresencaMembro> ListarPresencaDatasPaginado(int pageSize, int rowStart, out int rowCount, string sorting, int idPresenca, int idData, string campo, string valor, long usuarioID)
        {
            return _presencaRepository.ListarPresencaDatasPaginado(pageSize, rowStart, out rowCount, sorting, idPresenca, idData, campo, valor, usuarioID);
        }

        public bool ExisteInscricaoDatas(int idData)
        {
            return _presencaRepository.ExisteInscricaoDatas(idData);
        }

        public IEnumerable<Presenca> ConsultarPresencaIdData(int idData)
        {
            return _presencaRepository.ConsultarPresencaIdData(idData);
        }
    }
}

