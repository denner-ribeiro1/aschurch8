using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Secretaria.Models.Congregacao;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Congregacao;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Filters.Menu;
using ASChurchManager.Web.Lib;
using ASChurchManager.Web.ViewModels.Shared;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ASChurchManager.Web.Areas.Secretaria.Controllers
{
    [Rotina(Area.Secretaria), ControllerAuthorize("Congregacao")]
    public class CongregacaoController : BaseController
    {
        private ICongregacaoAppService _appService;
        private IGrupoAppService _grupoAppService;
        private readonly IMapper _mapper;

        public CongregacaoController(ICongregacaoAppService appService,
                    IGrupoAppService grupoAppService,
                    IMapper mapper
                    , IMemoryCache cache
                    , IUsuarioLogado usuLog
                    , IConfiguration _configuration
                    , IRotinaAppService _rotinaAppService)
            : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _appService = appService;
            _grupoAppService = grupoAppService;
            _mapper = mapper;
        }

        // GET: Secretaria/Congregacao
        [Action(Menu.Congregacao)]
        public ActionResult Index()
        {
            return View(new IndexVM());
        }

        [HttpPost]
        public JsonResult GetList([FromServices] ILogger<CongregacaoController> logger,
             string filtro = "", string conteudo = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var congr = _appService.ListarCongregacaoPaginado(jtPageSize, jtStartIndex, jtSorting, filtro, conteudo, UserAppContext.Current.Usuario.Id, out int qtdRows).ToList();
                return Json(new { Result = "OK", Records = congr, TotalRecordCount = qtdRows });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { Result = "ERROR", ex.Message });
            }

        }


        // GET: Secretaria/Congregacao/Details/5
        public ActionResult Details(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Congregacao_Details&valor=0");

            var congregacao = _appService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (congregacao == null || congregacao.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var arq = _appService.ListaArquivos((int)id);
            var congregacaoVm = DeParaModelToVM(congregacao, Acao.Read, arq, true);
            return View(congregacaoVm);
        }

        private Congregacao DeParaVMtoModel(CongregacaoVM congregacaoVM)
        {
            var congr = new Congregacao()
            {
                Id = congregacaoVM.Id,
                CongregacaoResponsavelId = congregacaoVM.CongregacaoResponsavelId.Value,
                CongregacaoResponsavelNome = congregacaoVM.CongregacaoResponsavelNome,
                DataAlteracao = congregacaoVM.DataAlteracao,
                DataCriacao = congregacaoVM.DataCriacao,
                Nome = congregacaoVM.Nome,
                PastorResponsavelId = congregacaoVM.PastorResponsavelId.Value,
                PastorResponsavelNome = congregacaoVM.PastorResponsavelNome,
                Sede = congregacaoVM.Sede,
                CNPJ = congregacaoVM.CNPJ,
            };

            congr.Endereco = _mapper.Map<Endereco>(congregacaoVM.Endereco);
            congregacaoVM.Obreiros.ForEach(o => congr.Obreiros.Add(new CongregacaoObreiro()
            {
                ObreiroId = o.ObreiroId.Value,
                CongregacaoId = congregacaoVM.Id
            }));

            congregacaoVM.Grupos.ForEach(g => congr.Grupos.Add(new CongregacaoGrupo()
            {
                GrupoId = (int)g.GrupoId,
                NomeGrupo = g.NomeGrupo,
                ResponsavelId = g.ResponsavelId.Value,
                ResponsavelNome = g.ResponsavelNome,
                CongregacaoId = congregacaoVM.Id
            }));

            congregacaoVM.Observacoes.ForEach(g => congr.Observacoes.Add(new ObservacaoCongregacao()
            {
                Observacao = g.Observacao,
                DataCadastro = string.IsNullOrEmpty(g.DataCadastroObs) ? DateTime.MinValue : Convert.ToDateTime(g.DataCadastroObs),
                Usuario = new Usuario()
                {
                    Id = g.IdResponsavelObs,
                    Nome = g.ResponsavelObs
                },
                CongregacaoId = congregacaoVM.Id
            }));

            congr.Endereco.Pais = congregacaoVM.Endereco.Pais;
            if (!string.IsNullOrWhiteSpace(congregacaoVM.Endereco.Pais) && congregacaoVM.Endereco.Pais != "Brasil")
            {
                congr.Endereco.Cep = congregacaoVM.Endereco.CodigoPostal;
                congr.Endereco.Bairro = congregacaoVM.Endereco.BairroEstrangeiro;
                congr.Endereco.Cidade = congregacaoVM.Endereco.CidadeEstrangeiro;
                congr.Endereco.Estado = congregacaoVM.Endereco.Provincia;
            }
            else
            {
                congr.Endereco.Cep = congregacaoVM.Endereco.Cep;
                congr.Endereco.Bairro = congregacaoVM.Endereco.Bairro;
                congr.Endereco.Cidade = congregacaoVM.Endereco.Cidade;
                congr.Endereco.Estado = congregacaoVM.Endereco.Estado.ToString();
            }

            return congr;
        }

        private CongregacaoVM DeParaModelToVM(Congregacao congregacao, Acao acao, List<ArquivoAzure> arquivos, bool readOnly)
        {
            var enderecoVm = congregacao.Endereco != null && congregacao.Endereco.Logradouro != null
                ? _mapper.Map<EnderecoViewModel>(congregacao.Endereco)
                : new EnderecoViewModel();

            var congregacaoVm = new CongregacaoVM()
            {
                Id = congregacao.Id,
                CongregacaoResponsavelId = congregacao.CongregacaoResponsavelId,
                CongregacaoResponsavelNome = congregacao.CongregacaoResponsavelNome,
                DataAlteracao = congregacao.DataAlteracao,
                DataCriacao = congregacao.DataCriacao,
                Endereco = enderecoVm,
                Nome = congregacao.Nome,
                PastorResponsavelId = congregacao.PastorResponsavelId,
                PastorResponsavelNome = congregacao.PastorResponsavelNome,
                Sede = congregacao.Sede,
                CNPJ = congregacao.CNPJ,
                Acao = acao
            };

            congregacao.Grupos.ForEach(g => congregacaoVm.Grupos.Add(new GrupoVM()
            {
                GrupoId = g.GrupoId,
                Grupo = g.Grupo,
                NomeGrupo = g.NomeGrupo,
                ResponsavelId = g.ResponsavelId,
                ResponsavelNome = g.ResponsavelNome
            }));

            congregacao.Obreiros.ForEach(o => congregacaoVm.Obreiros.Add(new ObreiroVM()
            {
                ObreiroId = o.ObreiroId,
                ObreiroNome = o.ObreiroNome,
                ObreiroCargo = o.ObreiroCargo
            }));

            congregacao.Observacoes.ForEach(o => congregacaoVm.Observacoes.Add(new Models.Congregacao.ObservacaoVM()
            {
                Observacao = o.Observacao,
                DataCadastroObs = o.DataCadastro.ToString("dd/MM/yyyy HH':'mm':'ss"),
                IdResponsavelObs = (int)o.Usuario.Id,
                ResponsavelObs = o.Usuario.Nome
            }));

            congregacaoVm.Arquivos = DeParaArquivoVM(arquivos);

            if (congregacao.Endereco.Pais != "Brasil")
            {
                congregacaoVm.Endereco.BairroEstrangeiro = congregacao.Endereco.Bairro;
                congregacaoVm.Endereco.CidadeEstrangeiro = congregacao.Endereco.Cidade;
                congregacaoVm.Endereco.Provincia = congregacao.Endereco.Estado;
                congregacaoVm.Endereco.CodigoPostal = congregacao.Endereco.Cep;
            }

            congregacaoVm.IsReadOnly = readOnly;
            congregacaoVm.Endereco.IsReadOnly = readOnly;
            congregacaoVm.Grupo.IsReadOnly = readOnly;
            congregacaoVm.Obreiro.IsReadOnly = readOnly;
            congregacaoVm.Observacao.IsReadOnly = readOnly;
            return congregacaoVm;
        }

        // GET: Secretaria/Congregacao/Create
        public ActionResult Create([FromServices] IMembroAppService _membroAppService)
        {
            var _grupo = _grupoAppService.GetAll(UserAppContext.Current.Usuario.Id);
            var congregacaoVm = new CongregacaoVM();
            congregacaoVm.Grupo.SelectGrupos = _grupo.ToSelectList("Id", "Descricao");
            congregacaoVm.Acao = Acao.Create;

            var paises = _membroAppService.ConsultarPaises();
            congregacaoVm.Endereco.SelectPaises = paises.Select(e => new SelectListItem
            {
                Value = e.Nome,
                Text = e.Nome
            });
            return View(congregacaoVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult Create([FromServices] ILogger<CongregacaoController> logger,
            [FromServices] IMembroAppService _membroAppService,
            CongregacaoVM congregacaoVm)
        {
            try
            {
                if (congregacaoVm.Endereco.Pais == "Brasil")
                {
                    ModelState.Remove("Endereco.CodigoPostal");
                    ModelState.Remove("Endereco.CidadeEstrangeiro");
                    ModelState.Remove("Endereco.Provincia");
                }
                else
                {
                    ModelState.Remove("Endereco.Cep");
                    ModelState.Remove("Endereco.Bairro");
                    ModelState.Remove("Endereco.Cidade");
                    ModelState.Remove("Endereco.Estado");
                }
                if (ModelState.IsValid)
                {
                    _appService.Add(DeParaVMtoModel(congregacaoVm));
                    this.ShowMessage("Sucesso", "Cadastro de Congregação realizado com sucesso!", AlertType.Success);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                var msgAlert = $"Falha ao incluir a Congregação - Erro: {ex.Message}";
                this.ShowMessage("Congregação", msgAlert, AlertType.Error);
            }
            congregacaoVm.Grupo.SelectGrupos = _grupoAppService.GetAll(UserAppContext.Current.Usuario.Id)
                                                        .ToSelectList("Id", "Descricao");
            congregacaoVm.Acao = Acao.Create;
            var paises = _membroAppService.ConsultarPaises();
            congregacaoVm.Endereco.SelectPaises = paises.Select(e => new SelectListItem
            {
                Value = e.Nome,
                Text = e.Nome
            });
            return View(congregacaoVm);
        }

        public ActionResult Edit([FromServices] IMembroAppService _membroAppService, long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Congregacao_Edit&valor=0");

            var congregacao = _appService.GetById(id, UserAppContext.Current.Usuario.Id);

            if (congregacao == null || congregacao.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var arq = _appService.ListaArquivos((int)id);
            var congregacaoVm = DeParaModelToVM(congregacao, Acao.Update, arq, false);
            congregacaoVm.Grupo.SelectGrupos = _grupoAppService.GetAll(UserAppContext.Current.Usuario.Id).ToSelectList("Id", "Descricao");
            var paises = _membroAppService.ConsultarPaises();
            congregacaoVm.Endereco.SelectPaises = paises.Select(e => new SelectListItem
            {
                Value = e.Nome,
                Text = e.Nome
            });

            return View(congregacaoVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult Edit([FromServices] ILogger<CongregacaoController> logger,
            [FromServices] IMembroAppService _membroAppService,
            CongregacaoVM congregacaoVm)
        {
            try
            {
                if (congregacaoVm.Endereco.Pais == "Brasil")
                {
                    ModelState.Remove("Endereco.CodigoPostal");
                    ModelState.Remove("Endereco.CidadeEstrangeiro");
                    ModelState.Remove("Endereco.Provincia");
                }
                else
                {
                    ModelState.Remove("Endereco.Cep");
                    ModelState.Remove("Endereco.Bairro");
                    ModelState.Remove("Endereco.Cidade");
                    ModelState.Remove("Endereco.Estado");
                }
                if (ModelState.IsValid)
                {
                    _appService.Update(DeParaVMtoModel(congregacaoVm));
                    this.ShowMessage("Sucesso", "Congregação atualizada com sucesso!", AlertType.Success);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                this.ShowMessage("Congregação", $"Falha ao salvar o Congregado. Erro: {ex.Message}", AlertType.Error);
            }
            congregacaoVm.Grupo.SelectGrupos = _grupoAppService.GetAll(UserAppContext.Current.Usuario.Id)
                                                        .ToSelectList("Id", "Descricao");
            var paises = _membroAppService.ConsultarPaises();
            congregacaoVm.Endereco.SelectPaises = paises.Select(e => new SelectListItem
            {
                Value = e.Nome,
                Text = e.Nome
            });
            congregacaoVm.Acao = Acao.Update;
            return View(congregacaoVm);
        }


        [HttpPost]
        public JsonResult ConsultarQtdMembrosCongregacao(long id)
        {
            var congregacao = _appService.ConsultarQtdMembrosCongregacao(id);
            if (congregacao.Any())
            {
                congregacao.ForEach(x =>
                {
                    x.TipoMembroDescr = x.TipoMembro.GetDisplayAttributeValue();
                    x.StatusDescr = x.Status.GetDisplayAttributeValue();
                });
            }
            return Json(congregacao);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Delete([FromServices] ILogger<CongregacaoController> logger, long id, long congregacaoId)
        {
            try
            {
                _appService.Delete(id, congregacaoId, UserAppContext.Current.Usuario.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                string message = ex.Message;
                if (ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint") ||
                    ex.Message.Contains("A instrução DELETE conflitou com a restrição do REFERENCE "))
                    message = "Não foi possível excluir esta Congregação. Ela está sendo utilizada em outros registros.";
                return Json(new { status = "Erro", mensagem = message, url = "" });
            }
            this.ShowMessage("Sucesso", "Congregação excluída com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Congregação excluída com sucesso!", url = Url.Action("Index", "Congregacao") });
        }

        [HttpPost]
        public JsonResult BuscarCongregacao(long congregacaoId)
        {
            var congregacao = _appService.GetById(congregacaoId, UserAppContext.Current.Usuario.Id);

            return Json(congregacao);
        }

        [HttpPost]
        public JsonResult BuscarCongregacaoSemFiltroUsuario(long congregacaoId)
        {
            var congregacao = _appService.GetById(congregacaoId);
            return Json(congregacao);
        }

        public JsonResult BuscarPastorRespEObreiros([FromServices] IMembroAppService _membroAppService, long membroId, long congregacaoId)
        {
            return ValidarPastorRespEObreiros(_membroAppService, membroId, congregacaoId);
        }

        private JsonResult ValidarPastorRespEObreiros(IMembroAppService _membroAppService, long membroId, long congregacaoId)
        {
            var congregacao = _appService.GetAll(false);
            var congr = congregacao.FirstOrDefault(c => c.PastorResponsavelId == membroId);
            if (congr != null && ((congregacaoId == 0 && congr.PastorResponsavelId == membroId) ||
                                  (congr.Id != congregacaoId && congr.PastorResponsavelId == membroId)))
            {
                return Json(new { result = "ERRO", mensagem = $"Membro cadastrado como Pastor Responsável na Congregação {congr.Nome}" });
            }

            var congrObr = _appService.ListarObreirosCongregacaoPorMembroId(membroId);
            if (congrObr.Count() > 0)
            {
                return Json(new { result = "ERRO", mensagem = $"Membro vinculado na Congregação {congrObr.FirstOrDefault().CongregacaoNome}" });
            }

            var membro = _membroAppService.GetById(membroId, UserAppContext.Current.Usuario.Id);
            Estado estado;
            if (Enum.TryParse<Estado>(membro.Endereco.Estado, true, out estado))
                membro.Endereco.Estado = Convert.ToInt64(estado).ToString();

            return Json(new { result = "OK", membro });
        }

        public FileStreamResult DownloadArquivo([FromServices] ILogger<PresencaController> logger, int id, string nomeArquivo)
        {
            try
            {
                var arquivo = _appService.DownloadArquivo(id, nomeArquivo).Result;

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

        private List<ArquivoVM> DeParaArquivoVM(List<ArquivoAzure> arquivos)
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
             int id, string nomeArquivo)
        {
            try
            {
                await _appService.DeleteFilesAsync(id, nomeArquivo);
                var arqs = DeParaArquivoVM(_appService.ListaArquivos(id));

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

        public JsonResult ArquivoJaExiste(int id, string nomeArquivo)
        {
            try
            {
                var arqs = _appService.ListaArquivos(id);
                return Json(new { status = "OK", data = arqs.Any(a => a.Nome == nomeArquivo) });
            }
            catch (Exception ex)
            {
                return Json(new { status = "ERRO", mensagem = ex.Message });
            }
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 1209715200)]
        [RequestSizeLimit(1209715200)]
        public JsonResult UploadArquivo([FromServices] ILogger<PresencaController> logger,
            IList<IFormFile> files, int id)
        {
            try
            {
                foreach (var file in files)
                {
                    //if (file.Length > 7340032)
                    //    throw new Erro($"Arquivo maior que 7 MB. Tamanho do arquivo selecionado: {file.Length}");

                    var ret = _appService.UploadFileToStorage(id, file);
                    var arqs = DeParaArquivoVM(_appService.ListaArquivos(id));
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

        private FileStreamResult TratarException(Exception exc, ILogger<PresencaController> logger)
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _appService = null;
                _grupoAppService = null;
            }
            base.Dispose(disposing);
        }
    }
}
