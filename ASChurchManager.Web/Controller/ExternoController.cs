using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Models.Externo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASChurchManager.Web.Controller
{
    public class ExternoController : Microsoft.AspNetCore.Mvc.Controller
    {
        #region Confirmação de Membro
        [AllowAnonymous]
        public IActionResult PreTela()
        {
            return View("PreTela");
        }

        [AllowAnonymous, ValidateAntiForgeryToken, HttpPost]
        public JsonResult PesquisarMembro([FromServices] IMembroAppService appService, int fase, string cpf, string valor)
        {
            var membro = appService.GetByCPF(cpf);
            if (membro == null)
                return Json(new { Status = "ERRO", Msg = "CPF não localizado!<br /> Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro." });

            if (membro.Status != Status.Ativo)
                return Json(new { Status = "ERRO", Msg = "Membro não localizado!<br />Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro." });

            if (membro.MembroConfirmado == 1)
                return Json(new { Status = "ERRO", Msg = "Membro já realizou a Confirmação do Cadastro!<br />Caso tenha a necessidade de mais alguma atualizaçao de dados, favor entrar em contato com a secretaria de sua Congregação para os ajustes no Cadastro." });

            if (fase > 2)
                return Json(new { Status = "ERRO", Msg = "Fase inválida." });

            if (fase == 1)
            {
                if (!string.IsNullOrWhiteSpace(membro.NomeMae))
                {
                    var nomeMae = membro.NomeMae.Trim().Split(' ');
                    if (nomeMae.Length > 0)
                    {
                        if (string.IsNullOrEmpty(nomeMae[0]) || nomeMae[0].Trim().ToUpper() != valor.Trim().ToUpper())
                            return Json(new { Status = "ERRO", Msg = "O Nome da Mãe não corresponde ao Cadastro!<br />Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro." });
                    }
                    else
                        return Json(new { Status = "ERRO", Msg = "O Nome da Mãe não corresponde ao Cadastro!<br />Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro." });
                }
                else
                    return Json(new { Status = "ERRO", Msg = "O Nome da Mãe não corresponde ao Cadastro!<br />Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro." });

            }
            else if (fase == 2)
            {
                try
                {
                    var dataNascDig = Convert.ToDateTime(valor);
                    if (membro.DataNascimento.Value.Date != dataNascDig.Date)
                        return Json(new { Status = "ERRO", Msg = "Data de Nascimento! <br />Favor entrar em contato com a secretaria de sua Congregação para a regularização do Cadastro." });

                    byte[] cpfAsBytes = System.Text.Encoding.ASCII.GetBytes(membro.Cpf);
                    string cpf64 = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(cpfAsBytes);
                    return Json(new { Status = "OK", Url = Url.Action("DadosMembro", "Externo", new { chave = cpf64, Area = "" }) });
                }
                catch
                {
                    return Json(new { Status = "ERRO", Msg = "Erro no formato da data, digite DD/MM/AAAA" });
                }
            }

            return Json(new { Status = "OK" });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult DadosMembro([FromServices] IMembroAppService appService, [FromServices] IConfiguration _configuration,
            string chave)
        {
            byte[] chaveAsBytes = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode(chave);
            string CpfDigitado = System.Text.Encoding.ASCII.GetString(chaveAsBytes);

            var membro = appService.GetByCPF(CpfDigitado);
            var dadosVM = new DadosMembroVM()
            {
                Cpf = membro.Cpf,
                Id = membro.Id,
                SiteKey = _configuration["ParametrosSistema:CaptchaSite"]
            };

            dadosVM.SelectEstadoCivil = Enum.GetValues(typeof(EstadoCivil))
                     .Cast<EstadoCivil>()
                     .Select(e => new SelectListItem
                     {
                         Value = ((int)e).ToString(),
                         Text = e.GetDisplayAttributeValue()
                     });
            return View(dadosVM);
        }

        [AllowAnonymous]
        public IActionResult PopUpLGPD(string nome, string rg, string cpf, string ip)
        {
            var dadosVM = new DadosMembroVM()
            {
                Cpf = cpf,
                Nome = nome,
                Rg = rg,
                IP = ip,
                DataTermo = $"Mauá, {DateTime.Now.Day} de {DateTime.Now.GetMonthName().ToString(System.Globalization.CultureInfo.GetCultureInfo("pt-BR"))} de {DateTime.Now.Year}"
            };
            return View(dadosVM);
        }

        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> SalvarMembroAsync([FromServices] IMembroAppService appService, [FromServices] IConfiguration _configuration,
            [FromServices] IHttpContextAccessor contexto, DadosMembroVM dados)
        {
            try
            {
                var secretKey = _configuration["ParametrosSistema:CaptchaChave"];
                var token = contexto.HttpContext.Request.Form["g-recaptcha-response"].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new { status = "Erro", msg = "Não foi possível obter o Captcha!" });
                }

                var captchaIsValid = await ValidarCaptcha(token, secretKey);
                if (!captchaIsValid)
                {
                    return Json(new { status = "Erro", msg = "Captcha inválido!" });
                }

                string ip = dados.IP;
                if (string.IsNullOrWhiteSpace(System.Text.RegularExpressions.Regex.Replace(dados.IP, "[^0-9]", "")))
                    ip = contexto.HttpContext.Connection.RemoteIpAddress.ToString();

                var membro = DePara(dados);
                appService.AtualizarMembroExterno(membro, ip);

                var membroCad = appService.GetById(membro.Id, 0);
                var msg = string.Empty;
                if (string.IsNullOrWhiteSpace(membroCad.FotoUrl))
                    msg = "Favor procurar a secretária de sua Congregação para atualização da Foto e a Emissão da Carteirinha do Membro.";
                return Json(new { status = "OK", msg, url = _configuration["ParametrosSistema:UrlDirecionarExt"] });
            }
            catch (Exception ex)
            {
                return Json(new { status = "ERRO", msg = ex.Message });
            }
        }

        private async Task<bool> ValidarCaptcha(string token, string secret)
        {
            using var client = new System.Net.Http.HttpClient();
            var result = await client.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={token}").Result.Content.ReadAsStringAsync();
            return (bool)JObject.Parse(result.ToString())["success"];
        }

        private Membro DePara(DadosMembroVM entity)
        {
            var ret = new Membro()
            {
                Id = entity.Id,
                //Nome = entity.Nome,
                Cpf = entity.Cpf,
                //RG = entity.Rg,
                //OrgaoEmissor = entity.OrgaoEmissor,
                //DataNascimento = entity.DataNascimento,
                //NomePai = entity.NomePai,
                //NomeMae = entity.NomeMae,
                //EstadoCivil = entity.EstadoCivil.GetValueOrDefault(),
                //Sexo = entity.Sexo.GetValueOrDefault(),
                //Escolaridade = entity.Escolaridade.GetValueOrDefault(),
                //Nacionalidade = entity.Nacionalidade,
                //NaturalidadeEstado = entity.NaturalidadeEstado.ToString(),
                //NaturalidadeCidade = entity.NaturalidadeCidade,
                //Profissao = entity.Profissao,
                TelefoneResidencial = entity.TelefoneResidencial,
                TelefoneCelular = entity.TelefoneCelular,
                TelefoneComercial = entity.TelefoneComercial,
                Email = entity.Email
            };
            ret.Endereco = new Endereco()
            {
                Logradouro = entity.Logradouro,
                Numero = entity.Numero,
                Complemento = entity.Complemento,
                Bairro = entity.Bairro,
                Cidade = entity.Cidade,
                Estado = entity.Estado.ToString(),
                Pais = entity.Pais,
                Cep = entity.Cep,
            };
            return ret;
        }
        #endregion

        #region Frequencia
        [AllowAnonymous]
        public ActionResult Frequencia()
        {
            return View("PopUpFrequencia");
        }


        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public JsonResult MarcarPresenca([FromServices] ILogger<ExternoController> logger,
            [FromServices] IPresencaAppService presencaAppService, [FromServices] IMembroAppService membroAppService,
            string code)
        {
            try
            {
                byte[] codeAsBytes = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode(code);
                string codOriginal = System.Text.Encoding.ASCII.GetString(codeAsBytes);

                string tipo = codOriginal.Substring(0, 2);
                string codigo = codOriginal[2..];

                //Caso seja feito o registro com a Carteirinha
                if (tipo == "RM")
                {
                    var lInscAut = presencaAppService.ConsultarPresencaPorStatusData(0, StatusPresenca.Andamento).ToList();
                    if (lInscAut.Any() && lInscAut.FirstOrDefault().InscricaoAutomatica)
                    {
                        var evento = lInscAut.FirstOrDefault();
                        if (!evento.Datas.Any(p => p.Status == StatusPresenca.Andamento))
                            throw new Erro("Não foi localizado Evento com lista de chamada em Andamento.");

                        if (evento.Datas.Count(p => p.Status == StatusPresenca.Andamento) > 1)
                            throw new Erro("Evento inicializado incorretamente.");

                        if (!int.TryParse(codigo, out int rm))
                            throw new Erro("Falha ao decodificar o QRCode.");

                        var insc = presencaAppService.ConsultarPresencaInscricao(evento.Id, rm, null, 0);
                        int id = 0;
                        string nome = "";
                        if (insc == null || insc.Id == 0)
                        {
                            var membro = new PresencaMembro
                            {
                                PresencaId = (int)evento.Id,
                                MembroId = rm,
                                Pago = false,
                                Usuario = rm
                            };
                            id = presencaAppService.SalvarInscricao(membro);

                            var dadosMembro = membroAppService.GetById(rm, 0);
                            nome = dadosMembro.Nome;
                        }
                        else
                        {
                            id = (int)insc.Id;
                            nome = insc.Nome;
                        }

                        var datas = evento.Datas.First(p => p.Status == StatusPresenca.Andamento);
                        var pres = presencaAppService.ConsultarPresencaInscricaoDatas(id, datas.Id);
                        if (pres != null && pres.Id > 0 && pres.Tipo == TipoRegistro.Automatica)
                            throw new Erro("Presença já registrada!");

                        presencaAppService.SalvarPresencaInscricaoDatas(id, datas.Id, SituacaoPresenca.Presente, TipoRegistro.Automatica);

                        return Json(new { status = "OK", nome = nome, evento = evento.Descricao, dtEvento = datas.DataHoraInicio.ToShortDateString() });
                    }
                    else
                        throw new Erro("Não foi localizado Evento com lista de chamada em Andamento.");
                }
                else
                {
                    if (!int.TryParse(codigo, out int idInscricao))
                        throw new Erro("Falha ao decodificar o QRCode.");

                    var inscr = presencaAppService.ConsultarPresencaInscricao(idInscricao);
                    var evento = presencaAppService.GetById(inscr.PresencaId, 0);

                    if (!evento.Datas.Any(p => p.Status == StatusPresenca.Andamento))
                        throw new Erro("Não foi localizado Evento com lista de chamada em Andamento.");

                    if (evento.Datas.Count(p => p.Status == StatusPresenca.Andamento) > 1)
                        throw new Erro("Evento inicializado incorretamente.");

                    var datas = evento.Datas.First(p => p.Status == StatusPresenca.Andamento);

                    var pres = presencaAppService.ConsultarPresencaInscricaoDatas(inscr.Id, datas.Id);
                    if (pres != null && pres.Id > 0 && pres.Tipo == TipoRegistro.Automatica)
                        throw new Erro("Presença já registrada!");

                    presencaAppService.SalvarPresencaInscricaoDatas((int)inscr.Id, datas.Id, SituacaoPresenca.Presente, TipoRegistro.Automatica);

                    return Json(new { status = "OK", nome = inscr.Nome, evento = evento.Descricao, dtEvento = datas.DataHoraInicio.ToShortDateString() });//, data = presenca.OrderBy(a => a.DataHoraInicio) });
                }
            }
            catch (Erro ex)
            {
                return Json(new { status = "ERROR", ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "ERROR", ex.Message });
            }
        }
        #endregion
    }
}
