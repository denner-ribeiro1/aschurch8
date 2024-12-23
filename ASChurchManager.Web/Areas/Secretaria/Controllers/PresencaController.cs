using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Secretaria.Models.Presenca;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Filters.Menu;
using ASChurchManager.Web.Lib;
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
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ASChurchManager.Web.Areas.Secretaria.Controllers
{
    [Rotina(Area.Secretaria), ControllerAuthorize("Presenca")]
    public class PresencaController : BaseController
    {
        private readonly ITipoEventoAppService _tipoEventoAppService;
        private readonly IPresencaAppService _presencaAppService;

        public PresencaController(ITipoEventoAppService tipoEventoAppService,
                                  IPresencaAppService presencaAppService,
                                  IMemoryCache cache,
                                  IUsuarioLogado usuLog,
                                  IConfiguration _configuration,
                                  IRotinaAppService _rotinaAppService)
          : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _tipoEventoAppService = tipoEventoAppService;
            _presencaAppService = presencaAppService;
        }


        #region Configuração Lista de Presença

        #region Privates 
        private static Presenca ViewModelParaModelConfiguracao(PresencaVM model)
        {
            var ret = new Presenca()
            {
                Id = model.Id,
                TipoEventoId = (int)model.TipoEventoSelecionado,
                Descricao = model.Descricao,
                Valor = model.Valor.GetValueOrDefault(0),
                DataMaxima = model.DataMaximaCadastro.Value.Date,
                ExclusivoCongregacao = model.Exclusivo,
                GerarEventos = model.GerarEventos,
                NaoMembros = model.NaoMembros,
                InscricaoAutomatica = model.CongregacaoId == 1 && model.InscricaoAutomatica,
                Status = model.Status,
                CongregacaoId = (int)model.CongregacaoId
            };

            if (model.Datas != null)
            {
                var datas = new List<PresencaDatas>();
                foreach (var item in model.Datas
                                        .Where(d => (d.Acao == Acao.Create && d.Id == 0) || ((d.Acao == Acao.Update || d.Acao == Acao.Delete) && d.Id > 0))
                                        .OrderBy(a => a.Data))
                {
                    TimeSpan.TryParse(item.HoraInicio, out TimeSpan horaIni);
                    TimeSpan.TryParse(item.HoraFinal, out TimeSpan horaFin);

                    datas.Add(new PresencaDatas
                    {
                        Acao = item.Acao,
                        Id = item.Id,
                        DataHoraInicio = Convert.ToDateTime(item.Data).Add(horaIni),
                        DataHoraFim = Convert.ToDateTime(item.Data).Add(horaFin),
                        Status = item.Status
                    });
                }
                ret.Datas = datas;
            }
            return ret;
        }
        private static PresencaVM ModelParaViewModelConfiguracao(Presenca model,
                                                                 List<ArquivoAzure> arquivos)
        {
            var modelVm = new PresencaVM
            {
                CongregacaoId = model.CongregacaoId,
                Valor = (float)model.Valor,
                DataAlteracao = model.DataAlteracao,
                DataCriacao = model.DataCriacao,
                DataMaximaCadastro = model.DataMaxima,
                CongregacaoNome = model.Congregacao.Nome,
                Descricao = model.Descricao,
                Exclusivo = model.ExclusivoCongregacao,
                NaoMembros = model.NaoMembros,
                InscricaoAutomatica = model.CongregacaoId == 1 && model.InscricaoAutomatica,
                GerarEventos = model.GerarEventos,
                Id = model.Id,
                Status = model.Status,
                TipoEventoSelecionado = model.TipoEventoId,
                Datas = []
            };
            foreach (var item in model.Datas.OrderBy(a => a.DataHoraInicio))
            {
                modelVm.Datas.Add(new PresencaDataVM()
                {
                    Id = item.Id,
                    Data = item.DataHoraInicio.Date.ToShortDateString(),
                    HoraInicio = item.DataHoraInicio.ToString("HH:mm"),
                    HoraFinal = item.DataHoraFim.ToString("HH:mm")
                });
            }

            modelVm.Arquivos = DeParaArquivoVM(arquivos);
            return modelVm;
        }
        #endregion

        #region Configuracao
        [Action(Menu.Presenca, Menu.ConfiguracaoPresenca)]
        public ActionResult IndexConfiguracao([FromServices] ICongregacaoAppService _congrAppService)
        {
            var presencaVM = new IndexConfigVM();
            var _congregacoes = new List<SelectListItem>();
            foreach (var cong in _congrAppService.GetAll(UserAppContext.Current.Usuario.Id).OrderBy(p => p.Nome))
            {
                _congregacoes.Add(new SelectListItem()
                {
                    Text = cong.Nome,
                    Value = cong.Id.ToString()
                });
            }
            presencaVM.ListaCongregacoes = _congregacoes;

            IEnumerable<StatusPresenca> lStatus = Enum.GetValues(typeof(StatusPresenca))
                                                       .Cast<StatusPresenca>();
            var selectlst = (from item in lStatus
                             select new SelectListItem
                             {
                                 Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                 Value = ((int)item).ToString()
                             }).ToList();

            presencaVM.ListaStatusPresenca = selectlst.Where(a => a.Text != "Todos").ToList();

            var _lstTipoEventos = _tipoEventoAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList();
            presencaVM.ListaTipoEvento = _lstTipoEventos.ToSelectList("Id", "Descricao").ToList();

            return View(presencaVM);
        }

        [HttpPost]
        public JsonResult GetListConfiguracao([FromServices] ILogger<PresencaController> logger,
                                              string campo = "",
                                              string valor = "",
                                              int jtStartIndex = 0,
                                              int jtPageSize = 0,
                                              string jtSorting = null)
        {
            try
            {
                var presenca = _presencaAppService.ListarPresencaPaginado(jtPageSize, jtStartIndex, out int qtdRows, jtSorting, campo, valor,
                    false, UserAppContext.Current.Usuario.Id).ToList();
                var membrosVM = new List<PresencaGridVM>();
                presenca.ForEach(p => membrosVM.Add(new PresencaGridVM()
                {
                    Id = (int)p.Id,
                    Congregacao = p.Congregacao.Nome,
                    Descricao = p.Descricao,
                    TipoEvento = p.DescrTipoEventoId,
                    Status = p.Status.GetDisplayAttributeValue(),
                }));

                return Json(new { Result = "OK", Records = membrosVM, TotalRecordCount = qtdRows });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        public ActionResult CreateConfiguracao()
        {
            var _lstTipoEventos = _tipoEventoAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList();
            var presencaVM = new PresencaVM
            {
                Acao = Acao.Create,
                Status = StatusPresenca.EmAberto,
                CongregacaoId = UserAppContext.Current.Usuario.Congregacao.Id,
                CongregacaoNome = UserAppContext.Current.Usuario.Congregacao.Nome,
                SelectTiposEvento = _lstTipoEventos.ToSelectList("Id", "Descricao").ToList(),
                IsReadOnly = false
            };
            return View("Configuracao", presencaVM);
        }

        public ActionResult EditConfiguracao(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Presenca_EditConfiguracao&valor=0");
            var presenca = _presencaAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (presenca == null || presenca.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var arquivos = _presencaAppService.ListaArquivos((int)id);
            var presencaVM = ModelParaViewModelConfiguracao(presenca, arquivos);

            var _lstTipoEventos = _tipoEventoAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList();
            presencaVM.Acao = Acao.Update;
            presencaVM.SelectTiposEvento = _lstTipoEventos.ToSelectList("Id", "Descricao").ToList();
            presencaVM.IsReadOnly = false;

            return View("Configuracao", presencaVM);
        }

        public ActionResult DetailsConfiguracao(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Presenca_DetailsConfiguracao&valor=0");
            var presenca = _presencaAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (presenca == null || presenca.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var arquivos = _presencaAppService.ListaArquivos((int)id);
            var presencaVM = ModelParaViewModelConfiguracao(presenca, arquivos);

            var _lstTipoEventos = _tipoEventoAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList();
            presencaVM.Acao = Acao.Read;
            presencaVM.SelectTiposEvento = _lstTipoEventos.ToSelectList("Id", "Descricao").ToList();
            presencaVM.IsReadOnly = true;

            return View("Configuracao", presencaVM);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult DeleteConfiguracao([FromServices] ILogger<PresencaController> logger,
                                             long id)
        {
            try
            {
                _presencaAppService.Delete(id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                string message = ex.Message;
                if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint") ||
                    ex.Message.Contains("A instrução DELETE conflitou com a restrição do REFERENCE "))
                    message = "Não foi possível excluir este Curso/Evento. Ela está sendo utilizada em outros registros.";
                return Json(new { status = "Erro", mensagem = message, url = "" });
            }
            this.ShowMessage("Sucesso", "Cursos/Eventos excluídos com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Cursos/Eventos excluídos com sucesso!", url = Url.Action("IndexConfiguracao", "Presenca") });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult PersistirConfiguracao([FromServices] ILogger<PresencaController> logger,
                                                PresencaVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var presenca = ViewModelParaModelConfiguracao(model);

                    if (presenca.Datas.Count != 0 &&
                        presenca.DataMaxima > presenca.Datas.Max(d => d.DataHoraInicio.Date))
                        throw new Erro($"Data máxima para o Cadastro ({presenca.DataMaxima.ToShortDateString()}) é maior do que a última data do Curso/Evento ({presenca.Datas.Max(d => d.DataHoraInicio.Date).ToShortDateString()}) ");

                    _presencaAppService.Add(presenca);
                    if (presenca.StatusRetorno == TipoStatusRetorno.OK)
                    {
                        var msgAlert = $"Configuração de Controle de Presença em Eventos/Cursos incluída com sucesso!";
                        if (model.Acao == Acao.Update)
                            msgAlert = $"Configuração de Controle de Presença em Eventos/Cursos atualizada com sucesso!";
                        this.ShowMessage("Sucesso", msgAlert, AlertType.Success);

                        return Json(new { status = "OK", msg = msgAlert, url = Url.Action("IndexConfiguracao", "Presenca", new { Area = "Secretaria" }) });
                    }
                    else if (presenca.StatusRetorno == TipoStatusRetorno.VALIDACOES)
                    {
                        throw new Erro(string.Join(Environment.NewLine, presenca.ErrosRetorno.Select(e => e.Mensagem)));
                    }
                    else
                    {
                        throw new Exception(string.Join(Environment.NewLine, presenca.ErrosRetorno.Select(e => e.Mensagem)));
                    }
                }
                else
                {
                    var erros = new List<string>();
                    foreach (var modelState in ViewData.ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            erros.Add(error.ErrorMessage);
                        }
                    }
                    throw new Erro(string.Join(Environment.NewLine, erros));
                }
            }
            catch (Erro ex)
            {
                var msgAlert = $"Falha ao incluir a Configuração de Controle de Presença em Eventos/Cursos - Erro: {ex.Message}";
                if (model.Acao == Acao.Update)
                    msgAlert = $"Falha ao atualizar a Configuração de Controle de Presença em Eventos/Cursos - Erro: {ex.Message}";
                return Json(new { status = "Erro", membroid = model?.Id, msg = msgAlert, url = Url.Action("IndexConfiguracao", "Presenca", new { Area = "Secretaria" }) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                var msgAlert = $"Falha ao incluir a Configuração de Controle de Presença em Eventos/Cursos - Erro: {ex.Message}";
                if (model.Acao == Acao.Update)
                    msgAlert = $"Falha ao atualizar a Configuração de Controle de Presença em Eventos/Cursos - Erro: {ex.Message}";
                return Json(new { status = "Erro", membroid = model?.Id, msg = msgAlert, url = Url.Action("IndexConfiguracao", "Presenca", new { Area = "Secretaria" }) });
            }
        }

        #endregion
        private IndexIncricaoVM ViewModelIncricao(ICongregacaoAppService _congrAppService,
                                                  bool naoMembro,
                                                  bool captura = false)
        {
            var presencaVM = new IndexIncricaoVM
            {
                NaoMembro = naoMembro,
                Captura = captura
            };

            var _congregacoes = new List<SelectListItem>();
            foreach (var cong in _congrAppService.GetAll(UserAppContext.Current.Usuario.Id).OrderBy(p => p.Nome))
            {
                _congregacoes.Add(new SelectListItem()
                {
                    Text = cong.Nome,
                    Value = cong.Id.ToString()
                });
            }
            presencaVM.ListaCongregacoes = _congregacoes;

            IEnumerable<StatusPresenca> lStatus = Enum.GetValues(typeof(StatusPresenca))
                                                       .Cast<StatusPresenca>();
            var selectlst = (from item in lStatus
                             select new SelectListItem
                             {
                                 Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                 Value = ((int)item).ToString()
                             }).ToList();

            presencaVM.ListaStatusPresenca = selectlst.Where(a => a.Text != "Todos").ToList();

            var _lstTipoEventos = _tipoEventoAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList();
            presencaVM.ListaTipoEvento = _lstTipoEventos.ToSelectList("Id", "Descricao").ToList();

            return presencaVM;
        }

        #region Inscricao
        [Action(Menu.Presenca, Menu.InscricaoPresenca)]
        public ActionResult IndexInscricao([FromServices] ICongregacaoAppService _congrAppService)
        {
            return View("IndexInscricao", ViewModelIncricao(_congrAppService, false));
        }

        public ActionResult Inscricao([FromServices] ICongregacaoAppService _congrAppService,
                                      int id,
                                      bool naoMembro = false)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Presenca_Details&valor=0");

            var presenca = _presencaAppService.GetById(id, UserAppContext.Current.Usuario.Id);

            if (presenca == null || presenca.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var inscricaoVM = new InscricaoVM()
            {
                Presenca = presenca.Id,
                Descricao = presenca.Descricao,
                DataMaxima = presenca.DataMaxima.ToShortDateString(),
                DataHoraInicio = $"{presenca.DataHoraInicio.ToShortDateString()} {presenca.DataHoraInicio.ToShortTimeString()}",
                IsReadOnly = presenca.Status == StatusPresenca.Finalizado && !UserAppContext.Current.Usuario.Congregacao.Sede,
                Valor = presenca.Valor,
                PermiteInscricoes = DateTime.Now.Date <= presenca.DataMaxima.Date || UserAppContext.Current.Usuario.PermiteCadBatismoAposDataMaxima,
                NaoMembro = presenca.NaoMembros
            };

            var _congregacoes = new List<SelectListItem>();

            var inscr = _presencaAppService.ConsultarPresencaInscricaoPorPresencaId(id, UserAppContext.Current.Usuario.Id);

            if (naoMembro)
            {
                inscr = inscr.Where(p => p.MembroId == 0).ToList();
            }

            foreach (var cong in inscr.GroupBy(g => g.Igreja).OrderBy(p => p.Key))
            {
                _congregacoes.Add(new SelectListItem()
                {
                    Text = cong.Key,
                    Value = cong.Key
                });
            }
            inscricaoVM.ListaCongregacoes = _congregacoes;

            if (presenca.ExclusivoCongregacao)
            {
                var congr = _congrAppService.GetById(presenca.CongregacaoId);
                inscricaoVM.CongregacaoEventoId = (int)congr.Id;
                inscricaoVM.CongregacaoEvento = congr.Nome;
            }

            if (naoMembro)
                return View("InscricaoNaoMembro", inscricaoVM);
            return View("Inscricao", inscricaoVM);
        }

        [HttpPost]
        public JsonResult GetMembros([FromServices] ILogger<PresencaController> logger,
                                     int idPresenca,
                                     string filtro = "",
                                     string conteudo = "",
                                     bool naoMembro = false,
                                     bool somenteMembro = false)
        {
            try
            {
                var presenca = _presencaAppService.ConsultarPresencaInscricaoPorPresencaId(idPresenca, UserAppContext.Current.Usuario.Id);
                if (naoMembro)
                {
                    presenca = presenca.Where(p => p.MembroId == 0).ToList();
                }
                else if (somenteMembro)
                {
                    presenca = presenca.Where(p => p.MembroId > 0).ToList();
                }

                if (!string.IsNullOrWhiteSpace(filtro) && !string.IsNullOrWhiteSpace(conteudo))
                {
                    switch (filtro)
                    {
                        case "Id":
                            return Json(new { status = "OK", data = presenca.Where(p => p.MembroId == Convert.ToInt32(conteudo)).OrderBy(a => a.Nome).ToList() });
                        case "Nome":
                            return Json(new { status = "OK", data = presenca.Where(p => p.Nome.Contains(conteudo)).OrderBy(a => a.Nome).ToList() });
                        case "CPF":
                            return Json(new { status = "OK", data = presenca.Where(p => p.CPF == conteudo).OrderBy(a => a.Nome).ToList() });
                        case "CongregacaoId":
                            return Json(new { status = "OK", data = presenca.Where(p => p.Igreja.Trim().Equals(conteudo.Trim())).OrderBy(a => a.Nome).ToList() });

                        default:
                            break;
                    }
                }
                return Json(new { status = "OK", data = presenca.OrderBy(a => a.Nome) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "ERROR", ex.Message });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult AddMembros([FromServices] ILogger<PresencaController> logger,
                                     int idPresenca,
                                     int idMembro,
                                     bool pago)
        {
            try
            {
                var evento = _presencaAppService.GetById(idPresenca, UserAppContext.Current.Usuario.Id);
                if (DateTime.Now.Date > evento.DataMaxima.Date && !UserAppContext.Current.Usuario.PermiteCadBatismoAposDataMaxima)
                {
                    throw new Erro("Curso/Evento com as inscrições já finalizadas. Favor entrar em contato com a Secretaria da Sede");
                }

                var presenca = _presencaAppService.ConsultarPresencaInscricaoPorPresencaId(idPresenca, 0);
                if (presenca.Any(p => p.PresencaId == idPresenca && p.MembroId == idMembro))
                {
                    throw new Erro("Membro já cadastrado no Curso/Evento!");
                }

                var membro = new PresencaMembro
                {
                    PresencaId = idPresenca,
                    MembroId = idMembro,
                    Pago = pago,
                    Usuario = (int)UserAppContext.Current.Usuario.Id
                };
                var id = _presencaAppService.SalvarInscricao(membro);

                return Json(new { status = "OK" });
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

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult AddNaoMembros([FromServices] ILogger<PresencaController> logger,
                                                [FromServices] ICongregacaoAppService _congrAppService,
                                                [FromServices] IUsuarioAppService _usuAppService,
                                                int idPresenca,
                                                string nome,
                                                string cpf,
                                                string igreja,
                                                string cargo,
                                                bool pago)
        {
            try
            {
                var presenca = _presencaAppService.ConsultarPresencaInscricaoPorPresencaId(idPresenca, 0);
                if (presenca.Any(p => p.PresencaId == idPresenca && p.CPF == cpf))
                {
                    var usuario = presenca.FirstOrDefault(p => p.PresencaId == idPresenca && p.CPF == cpf).Usuario;
                    var congr = _usuAppService.GetById(usuario, 0)?.CongregacaoNome;
                    var msg = "Membro já cadastrado no Curso/Evento!";
                    if (!string.IsNullOrWhiteSpace(congr))
                    {
                        msg = $"Membro já cadastrado no Curso/Evento na congregação {congr}!";
                    }
                    throw new Erro(msg);
                }
                var membro = new PresencaMembro
                {
                    PresencaId = idPresenca,
                    Nome = nome,
                    CPF = cpf,
                    Igreja = igreja,
                    Cargo = cargo,
                    Pago = pago,
                    Usuario = (int)UserAppContext.Current.Usuario.Id,
                    CongregacaoId = (int)UserAppContext.Current.Usuario.Congregacao.Id
                };
                var id = _presencaAppService.SalvarInscricao(membro);
                return Json(new { status = "OK" });
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

        [HttpPost]
        public JsonResult BuscarMembro([FromServices] IMembroAppService _membroAppService,
                                       [FromServices] ICongregacaoAppService _congrAppService,
                                       long membroId)
        {
            var membro = _membroAppService.GetById(membroId, 0);
            if (membro.Id != 0)
            {
                if (UserAppContext.Current.Usuario.Congregacao.Sede)
                {
                    var obr = _congrAppService.ListarObreirosCongregacaoPorMembroId(membroId);
                    if (obr != null && obr.Any())
                    {
                        membro.Congregacao.Id = obr.FirstOrDefault().CongregacaoId;
                        membro.Congregacao.Nome = obr.FirstOrDefault().CongregacaoNome;
                        return Json(membro);
                    }

                    var congregacao = _congrAppService.GetAll(false);
                    var c = congregacao.FirstOrDefault(c => c.PastorResponsavelId == membroId);
                    if (c != null && c.Id > 0)
                    {
                        membro.Congregacao.Id = c.Id;
                        membro.Congregacao.Nome = c.Nome;
                        return Json(membro);
                    }

                    return Json(membro);
                }


                if (UserAppContext.Current.Usuario.Congregacao.Id == membro.Congregacao.Id)
                    return Json(membro);

                var obreiros = _congrAppService.ListarObreirosCongregacao(UserAppContext.Current.Usuario.Congregacao.Id);
                if (obreiros.Any(o => o.ObreiroId == membro.Id))
                {
                    membro.Congregacao.Id = UserAppContext.Current.Usuario.Congregacao.Id;
                    membro.Congregacao.Nome = UserAppContext.Current.Usuario.Congregacao.Nome;
                    return Json(membro);
                }

                var congr = _congrAppService.GetById(UserAppContext.Current.Usuario.Congregacao.Id);
                if (congr.PastorResponsavelId == membroId)
                {
                    membro.Congregacao.Id = UserAppContext.Current.Usuario.Congregacao.Id;
                    membro.Congregacao.Nome = UserAppContext.Current.Usuario.Congregacao.Nome;
                    return Json(membro);
                }


            }
            return Json(new Membro() { Id = 0 });
        }

        [HttpPost]
        public JsonResult BuscarMembroCPF([FromServices] IMembroAppService _membroAppService,
                                          [FromServices] ICongregacaoAppService _congrAppService,
                                          string cpf)
        {
            var membro = _membroAppService.GetByCPF(cpf);
            if (membro.Id != 0)
            {
                return UserAppContext.Current.Usuario.Congregacao.Sede
                    ? Json(new { status = "OK", id = membro.Id, msg = $"CPF cadastrado ao membro de RM nº {membro.Id} vinculado a congregação {membro.Congregacao.Nome}." })
                    : Json(new { status = "OK", id = 0, msg = $"CPF de um membro cadastrado na congregação {membro.Congregacao.Nome}." });
            }
            return Json(new { status = "OK", id = 0, msg = "" });
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult DelMembros([FromServices] ILogger<PresencaController> logger,
                                     int id)
        {
            try
            {
                var presenca = _presencaAppService.DeleteInscricaoAsync(id);
                return Json(new { status = "OK" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "ERROR", ex.Message });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult AtualizarPago([FromServices] ILogger<PresencaController> logger,
                                        int id,
                                        bool pago)
        {
            try
            {
                _presencaAppService.AtualizarPagoInscricao(id, pago);
                return Json(new { status = "OK" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "ERROR", ex.Message });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult AddNaoMembrosLista([FromServices] ILogger<PresencaController> logger,
                                             int idPresenca,
                                             List<PresencaMembro> presencas)
        {
            var listaErros = new List<string>();
            try
            {
                var presenca = _presencaAppService.ConsultarPresencaInscricaoPorPresencaId(idPresenca, UserAppContext.Current.Usuario.Id);

                int idArquivo = 0;
                var nomeArquivo = presencas.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.NomeArquivo)).NomeArquivo;
                if (!string.IsNullOrWhiteSpace(nomeArquivo))
                    idArquivo = _presencaAppService.SalvarInscricaoArquivo(nomeArquivo);

                foreach (var item in presencas.Where(p => p.OK))
                {
                    if (presenca.Any(p => p.PresencaId == idPresenca && p.CPF == item.CPF))
                    {
                        listaErros.Add($"CPF {item.CPF} já cadastrado no Curso/Evento!");
                    }
                    else
                    {
                        item.Id = 0;
                        item.PresencaId = idPresenca;
                        item.Pago = true;
                        item.Usuario = (int)UserAppContext.Current.Usuario.Id;
                        item.ArquivoId = idArquivo;
                        item.CongregacaoId = (int)UserAppContext.Current.Usuario.Congregacao.Id;

                        try
                        {
                            item.Id = _presencaAppService.SalvarInscricao(item);
                        }
                        catch (Exception ex)
                        {
                            listaErros.Add($"Inscrição não realizada. Erro: {ex.Message}");
                        }

                    }
                }
                return Json(new { status = "OK", presenca = presencas.OrderBy(a => a.Nome).ToList(), erros = listaErros });
            }
            catch (Erro ex)
            {
                listaErros.Add(ex.Message);
                return Json(new { status = "ERROR", erros = listaErros });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                listaErros.Add(ex.Message);
                return Json(new { status = "ERROR", erros = listaErros });
            }
        }

        [HttpPost]
        public JsonResult UploadFile(IList<IFormFile> files,
                                     int id)
        {
            try
            {
                var config = _presencaAppService.GetById(id, UserAppContext.Current.Usuario.Id);
                if (config == null || config.Id == 0)
                    throw new Erro("Evento não localizado!");

                var file = files[0];
                var presenca = _presencaAppService.LerArquivoExcelAsync(file, id, UserAppContext.Current.Usuario.Id).Result;
                if (presenca.FirstOrDefault().StatusRetorno == TipoStatusRetorno.OK)
                    return Json(new { status = "OK", data = presenca.OrderByDescending(o => o.MembroId).OrderBy(o => o.Nome) });
                else{
                    throw new Exception(presenca.FirstOrDefault().ErrosRetorno.FirstOrDefault().Mensagem);
                }

            }
            catch (Exception ex)
            {
                return Json(new { status = "ERRO", mensagem = ex.Message });
            }
        }

        public ActionResult DadosArquivo(int id,
                                         string arquivo)
        {
            ViewData["IdPresenca"] = id;

            var presenca = _presencaAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            ViewData["Presenca"] = $"{presenca.Descricao} - Data Inicio: {presenca.DataHoraInicio.ToShortDateString()} - Data Máxima: {presenca.DataMaxima.ToShortDateString()}";

            ViewData["NomeArquivo"] = arquivo;
            return View("PopUpArquivo");
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult CapturaArquivoInscricao([FromServices] ILogger<PresencaController> logger,
                                                  IList<IFormFile> files,
                                                  int idPresenca)
        {
            try
            {
                var file = files[0];
                var idArquivo = 0;

                var presenca = _presencaAppService.LerArquivoExcelAsync(file, idPresenca, UserAppContext.Current.Usuario.Id).Result;

                if (presenca.Count == 0)
                {
                    throw new Erro("Não foi possível capturar o Arquivo. Arquivo Vazio!");
                }

                if (presenca.Any(a => !a.OK))
                {
                    var msg = string.Join(Environment.NewLine, string.Join(Environment.NewLine, presenca.Where(w => !w.OK).Select(s => s.ErrosRetorno.FirstOrDefault().Mensagem)));
                    throw new Erro(msg.Replace(Environment.NewLine, "<br />"));
                }

                var nomeArquivo = presenca.FirstOrDefault().NomeArquivo;
                if (!string.IsNullOrWhiteSpace(nomeArquivo))
                {
                    idArquivo = _presencaAppService.SalvarInscricaoArquivo(nomeArquivo);
                }

                foreach (var item in presenca.Where(w => w.OK))
                {
                    item.Id = 0;
                    item.PresencaId = idPresenca;
                    item.Pago = true;
                    item.Usuario = (int)UserAppContext.Current.Usuario.Id;
                    item.ArquivoId = idArquivo;
                    item.CongregacaoId = (int)UserAppContext.Current.Usuario.Congregacao.Id;
                    if (item.MembroId > 0)
                    {
                        item.Nome = null;
                        item.CPF = null;
                        item.Igreja = null;
                        item.Cargo = null;
                    }
                    item.Id = _presencaAppService.SalvarInscricao(item);
                }
                return Json(new { status = "OK", msg = $"Inscrições realizadas com sucesso.<br/>Quantidade de inscritos: {presenca.Count(a => a.OK)}" });

            }
            catch (Exception ex)
            {
                return Json(new { status = "ERRO", mensagem = ex.Message });
            }
        }

        #endregion

        [Action(Menu.Presenca, Menu.FrequenciaAutomatica)]
        public ActionResult IndexFrequencia()
        {
            var freqVM = new IndexFrequenciaVM();

            var _lstPresenca = _presencaAppService.ListarPresencaEmAberto(UserAppContext.Current.Usuario.Id).ToList();
            foreach (var item in _lstPresenca)
            {
                freqVM.ListaPresencaAberto.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = $"{item.Descricao} - Data Inicio: {item.DataHoraInicio.ToShortDateString()} - Data Máxima: {item.DataMaxima.ToShortDateString()}"
                });
            }

            var _lstPresencaAnd = _presencaAppService.ConsultarPresencaPorStatusData(0, StatusPresenca.Andamento).ToList();
            freqVM.ListaPresencaAndamento = _lstPresencaAnd;

            return View("IndexFrequencia", freqVM);
        }

        [Action(Menu.Presenca, Menu.FrequenciaManual)]
        public ActionResult IndexFrequenciaManual([FromServices] ICongregacaoAppService _congrAppService)
        {
            var presencaVM = new IndexFrequenciaManualVM();
            var _congregacoes = new List<SelectListItem>();
            foreach (var cong in _congrAppService.GetAll(UserAppContext.Current.Usuario.Id).OrderBy(p => p.Nome))
            {
                _congregacoes.Add(new SelectListItem()
                {
                    Text = cong.Nome,
                    Value = cong.Id.ToString()
                });
            }
            presencaVM.ListaCongregacoes = _congregacoes;

            IEnumerable<StatusPresenca> lStatus = Enum.GetValues(typeof(StatusPresenca))
                                                       .Cast<StatusPresenca>();
            var selectlst = (from item in lStatus
                             select new SelectListItem
                             {
                                 Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                 Value = ((int)item).ToString()
                             }).ToList();

            presencaVM.ListaStatusPresenca = selectlst.Where(a => a.Text != "Todos").ToList();

            var _lstTipoEventos = _tipoEventoAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList();
            presencaVM.ListaTipoEvento = _lstTipoEventos.ToSelectList("Id", "Descricao").ToList();

            return View(presencaVM);
        }

        [HttpPost]
        public JsonResult GetListPresenca([FromServices] ILogger<PresencaController> logger,
                                          string filtro = "",
                                          string conteudo = "",
                                          bool naoMembro = false,
                                          int jtStartIndex = 0,
                                          int jtPageSize = 0,
                                          string jtSorting = null)
        {
            try
            {
                var presenca = _presencaAppService.ListarPresencaPaginado(jtPageSize, jtStartIndex, out int qtdRows, jtSorting, filtro, conteudo, naoMembro, UserAppContext.Current.Usuario.Id).ToList();
                var membrosVM = new List<PresencaGridVM>();
                presenca.ForEach(p => membrosVM.Add(new PresencaGridVM()
                {
                    Id = (int)p.Id,
                    Congregacao = p.Congregacao.Nome,
                    Descricao = p.Descricao,
                    TipoEvento = p.DescrTipoEventoId,
                    Status = p.Status.GetDisplayAttributeValue(),
                    DataMaxima = p.DataMaxima.ToShortDateString(),
                    DataHoraInicio = $"{p.DataHoraInicio.ToShortDateString()} {p.DataHoraInicio.ToShortTimeString()}"
                }));

                return Json(new { Result = "OK", Records = membrosVM, TotalRecordCount = qtdRows });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        public ActionResult FrequenciaManual(int idPresenca)
        {
            var pres = _presencaAppService.GetById(idPresenca, 0);

            var index = new IndexFrequenciaVM
            {
                PresencaId = idPresenca,
                Descricao = pres.Descricao,
                Status = pres.Status.GetDisplayAttributeValue()
            };

            var _congregacoes = new List<SelectListItem>();
            var inscr = _presencaAppService.ConsultarPresencaInscricaoPorPresencaId(idPresenca, UserAppContext.Current.Usuario.Id);

            foreach (var cong in inscr.GroupBy(g => g.Igreja).OrderBy(p => p.Key))
            {
                _congregacoes.Add(new SelectListItem()
                {
                    Text = cong.Key,
                    Value = cong.Key
                });
            }
            index.ListaCongregacoes = _congregacoes;


            return View(index);
        }

        [HttpPost]
        public JsonResult ListaInscricoes([FromServices] ILogger<PresencaController> logger,
                                          int idPresenca,
                                          int idData,
                                          string filtro = "",
                                          string conteudo = "",
                                          int jtStartIndex = 0,
                                          int jtPageSize = 0,
                                          string jtSorting = null)
        {
            try
            {
                if (idData > 0)
                {
                    var inscr = new List<InscricoesVM>();

                    var presenca = _presencaAppService.ListarPresencaDatasPaginado(jtPageSize, jtStartIndex, out int qtdRows, jtSorting,
                        idPresenca, idData, filtro, conteudo, UserAppContext.Current.Usuario.Id).ToList();

                    presenca.ForEach(p => inscr.Add(new InscricoesVM()
                    {
                        Id = p.Id,
                        Nome = p.Nome,
                        MembroId = p.MembroId > 0 ? p.MembroId.ToString() : "N/M",
                        CPF = p.CPF,
                        Igreja = p.Igreja,
                        Situacao = p.Situacao == SituacaoPresenca.Presente,
                        Tipo = p.Tipo.GetDisplayAttributeValue(),
                        Justificativa = p.Justificativa,
                        IdData = idData,
                        StatusData = p.StatusData.GetDisplayAttributeValue()
                    }));

                    return Json(new { Result = "OK", Records = inscr, TotalRecordCount = qtdRows });
                }
                else
                {
                    return Json(new { Result = "OK", Records = new List<InscricoesVM>(), TotalRecordCount = 0 });
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { Result = "ERROR", ex.Message });
            }
        }


        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult AtualizarFrequenciaManual([FromServices] ILogger<PresencaController> logger,
                                                    int id,
                                                    int idData,
                                                    bool situacao,
                                                    string justificativa)
        {
            try
            {
                _presencaAppService.SalvarPresencaInscricaoDatas(id, idData, situacao ? SituacaoPresenca.Presente : SituacaoPresenca.Ausente, TipoRegistro.Manual, situacao ? "" : justificativa);
                return Json(new { status = "OK" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "ERROR", ex.Message });
            }
        }


        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult SalvarFrequenciaManual([FromServices] ILogger<PresencaController> logger,
                                                 int idData,
                                                 bool finalizar)
        {
            try
            {
                _presencaAppService.AtualizarStatusData(idData, finalizar ? StatusPresenca.Finalizado : StatusPresenca.EmAberto);
                var Message = $"Lista de Presença finalizada com Sucesso.";
                if (!finalizar)
                    Message = $"Reabertura da Lista de Presença realizada com Sucesso.";
                this.ShowMessage("Frequência", Message);
                return Json(new { status = "OK", Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "ERROR", ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetDatas([FromServices] ILogger<PresencaController> logger,
                                   int idPresenca,
                                   bool somenteFinalizados = false,
                                   bool incluirAndamento = false)
        {
            try
            {
                var presenca = _presencaAppService.ListarPresencaDatas(idPresenca);

                if (somenteFinalizados)
                {
                    return Json(new
                    {
                        status = "OK",
                        data = presenca.Where(p => p.Status == StatusPresenca.Finalizado)
                                       .OrderBy(a => a.DataHoraInicio)
                    });
                }
                if (incluirAndamento)
                {
                    return Json(new
                    {
                        status = "OK",
                        data = presenca.Where(p => p.Status != StatusPresenca.Finalizado)
                                   .OrderBy(a => a.DataHoraInicio)
                    });
                }
                return Json(new
                {
                    status = "OK",
                    data = presenca.Where(p => p.Status == StatusPresenca.EmAberto)
                                   .OrderBy(a => a.DataHoraInicio)
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "ERROR", ex.Message });
            }
        }

        public JsonResult IniciarPararPresenca([FromServices] ILogger<PresencaController> logger,
                                               int presencaId,
                                               int dataId,
                                               string tipo)
        {
            try
            {
                var pres = _presencaAppService.GetById(presencaId, UserAppContext.Current.Usuario.Id);
                if (pres.Status == StatusPresenca.Finalizado)
                    throw new Erro("Curso/Evento já finalizado");

                if (pres.Datas.Any(p => p.Status == StatusPresenca.Andamento && p.Id != dataId))
                {
                    throw new Erro($"Já existe para o Curso/Evento uma data cadastradas iniciada. <br />Favor finalizar a Data ({pres.Datas.FirstOrDefault(p => p.Status == StatusPresenca.Andamento).DataHoraInicio.ToShortDateString()})");
                }

                if (pres.InscricaoAutomatica && tipo == "AND")
                {
                    var lInscAut = _presencaAppService.ConsultarPresencaPorStatusData(0, StatusPresenca.Andamento).ToList();
                    if (lInscAut.Count != 0)
                    {
                        throw new Erro($"Já existe um Curso/Evento com inscrição automática iniciado. <br />Favor finalizar o Curso/Evento({lInscAut.FirstOrDefault().Descricao})");
                    }
                }

                _presencaAppService.AtualizarStatusData(dataId, tipo == "AND" ? StatusPresenca.Andamento : pres.Status);

                var presenca = _presencaAppService.ConsultarPresencaPorStatusData(0, StatusPresenca.Andamento).ToList();

                return Json(new { status = "OK", data = presenca.OrderBy(a => a.DataHoraInicio) });
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

        public JsonResult BuscarInscricao([FromServices] ILogger<PresencaController> logger,
                                          int presencaId,
                                          string tipo,
                                          string valor)
        {
            try
            {
                if (presencaId == 0)
                    throw new Erro("Favor selecionar um Curso/Evento!");

                int membroId = 0;
                string cpf = "";
                if (tipo == "Membro")
                    int.TryParse(valor, out membroId);
                else if (tipo == "NaoMembro")
                    cpf = valor;

                var presenca = _presencaAppService.ConsultarPresencaInscricao(presencaId, membroId, cpf, UserAppContext.Current.Usuario.Id);

                return Json(new { status = "OK", data = presenca });

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

        [Action(Menu.Presenca, Menu.EmissaoEtiquetas)]
        public ActionResult EmissaoEtiquetas([FromServices] ICongregacaoAppService _congrAppService)
        {
            var emissaoVM = new EmissaoEtiquetaVM();

            var _congregacoes = new List<SelectListItem>();
            foreach (var cong in _congrAppService.GetAll(UserAppContext.Current.Usuario.Id).OrderBy(p => p.Nome))
            {
                _congregacoes.Add(new SelectListItem()
                {
                    Text = cong.Nome,
                    Value = cong.Id.ToString()
                });
            }
            emissaoVM.ListaCongregacoes = _congregacoes;

            var _lstPresenca = _presencaAppService.GetAll(UserAppContext.Current.Usuario.Id);
            foreach (var item in _lstPresenca)
            {
                emissaoVM.ListaPresenca.Add(new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = $"{item.Descricao} - Data Inicio: {item.DataHoraInicio.ToShortDateString()} - Data Máxima: {item.DataMaxima.ToShortDateString()}"
                });
            }

            return View(emissaoVM);
        }

        public FileStreamResult Etiquetas([FromServices] ILogger<PresencaController> logger,
                                          int idPresenca,
                                          string congregacao,
                                          string tipo,
                                          string valor,
                                          string posicao,
                                          int margemTop = 0,
                                          int margemEsq = 0)
        {
            try
            {
                int.TryParse(congregacao, out int congr);
                int membroId = 0;
                string cpf = "";
                int tipoInt = 0;
                if (tipo == "Membro")
                {
                    tipoInt = 1;
                    int.TryParse(valor, out membroId);
                }
                else if (tipo == "NaoMembro")
                {
                    tipoInt = 2;
                    if (!string.IsNullOrEmpty(valor))
                        cpf = valor;
                }

                var relatorio = _presencaAppService.Etiquetas(idPresenca, congr, tipoInt, membroId, cpf, posicao, margemTop, margemEsq, UserAppContext.Current.Usuario.Id);
                return GerarRelatorio("etiquetas", relatorio, "application/pdf");
            }
            catch (Exception ex)
            {
                return TratarException(ex, logger);
            }
        }

        private FileStreamResult GerarRelatorio(string nomeArquivo,
                                                byte[] bytes,
                                                string mimeType)
        {
            var extensao = "pdf";
            var filename = $"{nomeArquivo}.{extensao}";
            var stream = new MemoryStream(bytes);
            var fileStreamResult = new FileStreamResult(stream, mimeType)
            {
                FileDownloadName = filename
            };
            Response.StatusCode = StatusCodes.Status200OK;
            return fileStreamResult;

        }

        private FileStreamResult TratarException(Exception exc,
                                                 ILogger<PresencaController> logger)
        {
            if (!(exc is Erro))
                logger.LogError(exc, $"Usuário - {HttpContext.User.Identity.Name}");

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
        #endregion

        public FileStreamResult DownloadArquivo([FromServices] ILogger<PresencaController> logger,
                                                int idPresenca,
                                                string nomeArquivo)
        {
            try
            {
                var arquivo = _presencaAppService.DownloadArquivo(idPresenca, nomeArquivo).Result;

                if (arquivo.BlobStream == null)
                {
                    throw new Erro($"Arquivo ({nomeArquivo}) não localizado.");
                }
                var fileStreamResult = new FileStreamResult(arquivo.BlobStream, arquivo.ContentType)
                {
                    FileDownloadName = nomeArquivo
                };
                Response.StatusCode = StatusCodes.Status200OK;

                return fileStreamResult;
            }
            catch (Exception ex)
            {
                return TratarException(ex, logger);
            }
        }

        private static List<ArquivoVM> DeParaArquivoVM(List<ArquivoAzure> arquivos)
        {
            var ret = new List<ArquivoVM>();
            if (arquivos.Count > 0)
            {
                foreach (var item in arquivos)
                {
                    var arq = new ArquivoVM()
                    {
                        Nome = item.Nome,
                        Tamanho = item.Tamanho,
                        Tipo = Util.RetornaTipoArquivo(item)
                    };
                    ret.Add(arq);
                }
            }
            return ret;
        }

        public async Task<JsonResult> DeleteArquivoAsync([FromServices] ILogger<PresencaController> logger,
                                                         int idPresenca,
                                                         string nomeArquivo)
        {
            try
            {
                await _presencaAppService.DeleteFilesAsync(idPresenca, nomeArquivo);
                var arqs = DeParaArquivoVM(_presencaAppService.ListaArquivos(idPresenca));

                return Json(new { status = "OK", mensagem = "Arquivo excluído com sucesso!", data = arqs });
            }
            catch (Erro ex)
            {
                return Json(new { status = "Erro", mensagem = ex.Message, url = "" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuario: {HttpContext.User.Identity.Name}");
                return Json(new { status = "Erro", mensagem = ex.Message, url = "" });
            }

        }

        public JsonResult ArquivoJaExiste(int idPresenca,
                                          string nomeArquivo)
        {
            try
            {
                var arqs = _presencaAppService.ListaArquivos(idPresenca);
                return Json(new { status = "OK", data = arqs.Any(a => a.Nome == nomeArquivo) });
            }
            catch (Exception ex)
            {
                return Json(new { status = "ERRO", mensagem = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UploadArquivo([FromServices] ILogger<PresencaController> logger,
                                        IList<IFormFile> files,
                                        int idPresenca)
        {
            try
            {
                foreach (var file in files)
                {
                    if (file.Length > 7340032)
                        throw new Erro($"Arquivo maior que 7 MB. Tamanho do arquivo selecionado: {file.Length}");

                    var ret = _presencaAppService.UploadFileToStorage(idPresenca, file);
                    var arqs = DeParaArquivoVM(_presencaAppService.ListaArquivos(idPresenca));
                    return Json(new { status = "OK", data = arqs });
                }

                return Json(new { status = "ERRO", mensagem = "Não foi possível ler o arquivo" });
            }
            catch (Erro ex)
            {
                return Json(new { status = "ERRO", mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "ERRO", mensagem = ex.Message });
            }
        }


        public JsonResult ExisteInscricaoData(int idData)
        {
            try
            {
                if (idData > 0)
                    return Json(new { status = "OK", data = _presencaAppService.ExisteInscricaoDatas(idData) });
                return Json(new { status = "OK", data = false });
            }
            catch (Exception ex)
            {
                return Json(new { status = "ERRO", mensagem = ex.Message });
            }
        }
    }
}
