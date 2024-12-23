using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Secretaria.Models.Membro;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Filters.Menu;
using ASChurchManager.Web.Lib;
using ASChurchManager.Web.Models.Shared;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ASChurchManager.Web.Areas.Secretaria.Controllers
{
    [Rotina(Area.Secretaria), ControllerAuthorize("Membro")]
    public class MembroController : BaseController
    {
        private readonly IMapper _mapper;
        private IMembroAppService _membroAppService;
        private ICargoAppService _cargoAppService;
        private ICursoArquivoMembroAppService _arquivoAppService;
        private ICursoAppService _cursoAppService;
        private ICongregacaoAppService _congrAppService;
        private IWebHostEnvironment _env;
        private readonly IArquivosAzureAppService _arqAzureService;

        public MembroController(IMembroAppService membroAppService,
                                ICargoAppService cargoAppService,
                                ICursoArquivoMembroAppService arquivoAppService,
                                ICursoAppService cursoAppService,
                                ICongregacaoAppService congrAppService,
                                IMapper mapper,
                                IWebHostEnvironment env,
                                IMemoryCache cache,
                                IUsuarioLogado usuLog
                                , IConfiguration _configuration
                                , IRotinaAppService _rotinaAppService
                                , IArquivosAzureAppService arquivosAzureAppService)
            : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _membroAppService = membroAppService;
            _cargoAppService = cargoAppService;
            _arquivoAppService = arquivoAppService;
            _cursoAppService = cursoAppService;
            _congrAppService = congrAppService;
            _mapper = mapper;
            _env = env;
            _arqAzureService = arquivosAzureAppService;
        }

        private MembroViewModel DeParaModelToVM(Membro membro, Acao acao, bool readOnly = false)
        {
            var membroVm = _mapper.Map<MembroViewModel>(membro);
            membroVm.Acao = acao;
            membroVm.IsReadOnly = readOnly;
            membroVm.Pessoa.IsReadOnly = readOnly;
            membroVm.Endereco.IsReadOnly = readOnly;

            membroVm.Cargo.IsReadOnly = readOnly;
            membroVm.Observacao.IsReadOnly = readOnly;
            membroVm.Situacao.IsReadOnly = readOnly;
            membroVm.CursoMembro.IsReadOnly = readOnly;

            membroVm.Historico.ForEach(a => a.IsReadOnly = readOnly);
            membroVm.HistCartasViewModel.IsReadOnly = readOnly;
            membroVm.MembroConfirmado = membro.MembroConfirmado == 1;

            if (membro.Pai != null && membro.Pai.Id > 0)
            {
                membroVm.Pessoa.PaiMembro = true;
                membroVm.Pessoa.PaiId = membro.Pai.Id;
                membroVm.Pessoa.NomePai = membro.Pai.Nome;
            }
            else
            {
                membroVm.Pessoa.PaiMembro = false;
                membroVm.Pessoa.NomePai = membro.NomePai;
            }

            if (membro.Mae != null && membro.Mae.Id > 0)
            {
                membroVm.Pessoa.MaeMembro = true;
                membroVm.Pessoa.MaeId = membro.Mae.Id;
                membroVm.Pessoa.NomeMae = membro.Mae.Nome;
            }
            else
            {
                membroVm.Pessoa.MaeMembro = false;
                membroVm.Pessoa.NomeMae = membro.NomeMae;
            }

            if (membro.Conjuge != null && membro.Conjuge.Id > 0)
            {
                membroVm.Pessoa.ConjugeMembro = true;
                membroVm.Pessoa.IdConjuge = membro.Conjuge.Id;
                membroVm.Pessoa.NomeConjuge = membro.Conjuge.Nome;
            }
            else
            {
                membroVm.Pessoa.ConjugeMembro = false;
                membroVm.Pessoa.NomeConjuge = membro.Conjuge.Nome;
            }

            membroVm.Pessoa.SelectEstadoCivil = Enum.GetValues(typeof(EstadoCivil))
                 .Cast<EstadoCivil>()
                 .Select(e => new SelectListItem
                 {
                     Value = ((int)e).ToString(),
                     Text = e.GetDisplayAttributeValue()
                 });

            if (membro.Endereco.Pais != "Brasil")
            {
                membroVm.Endereco.BairroEstrangeiro = membro.Endereco.Bairro;
                membroVm.Endereco.CidadeEstrangeiro = membro.Endereco.Cidade;
                membroVm.Endereco.Provincia = membro.Endereco.Estado;
                membroVm.Endereco.CodigoPostal = membro.Endereco.Cep;
                membroVm.Pessoa.TelefoneCelularEstrangeiro = membro.TelefoneCelular;
                membroVm.Pessoa.TelefoneComercialEstrangeiro = membro.TelefoneComercial;
                membroVm.Pessoa.TelefoneResidencialEstrangeiro = membro.TelefoneResidencial;
            }

            if (!string.IsNullOrWhiteSpace(membro.NaturalidadeEstado) && Enum.IsDefined(typeof(Estado), membro.NaturalidadeEstado))
                membroVm.Pessoa.NaturalidadeEstado = membro.NaturalidadeEstado.ToEnum<Estado>();

            /*Situacao*/
            membro.Situacoes.OrderByDescending(o => o.Data).ToList().ForEach(
                s => membroVm.Situacoes.Add(new SituacaoVM()
                {
                    IdSit = s.Situacao,
                    DescricaoSit = s.Situacao.GetDisplayAttributeValue(),
                    DataSit = s.Data.GetValueOrDefault().UtcDateTime.ToShortDateString(),
                    ObservacaoSit = s.Observacao?.Replace("\"", "")
                }));

            /*Cargo*/
            membro.Cargos.OrderByDescending(o => o.DataCargo).ToList().ForEach(
                c => membroVm.Cargos.Add(new CargoVM()
                {
                    CargoId = c.CargoId,
                    DescricaoCargo = c.Descricao?.Replace("\"", ""),
                    LocalConsagracao = c.LocalConsagracao,
                    DataCargo = c.DataCargo.GetValueOrDefault().UtcDateTime.ToShortDateString(),
                    Confradesp = c.Confradesp,
                    CGADB = c.CGADB
                }));

            /*Observacao*/
            membro.Observacoes.OrderByDescending(p => p.DataCadastro).ToList().ForEach(
                o => membroVm.Observacoes.Add(new ObservacaoVM()
                {
                    Observacao = o.Observacao?.Replace("\"", ""),
                    ResponsavelObs = o.Usuario.Nome,
                    IdResponsavelObs = Convert.ToInt32(o.Usuario.Id),
                    DataCadastroObs = o.DataCadastro.UtcDateTime.ToShortDateString()
                }));

            var arq = _arquivoAppService.GetArquivoByMembro(membro.Id).ToList();
            arq.ForEach(a => membroVm.ArquivosMembro.Add(new ArquivoMembroVM()
            {
                Id = a.Id,
                NomeCurso = a.NomeCurso,
                Local = a.Local,
                DataInicioCurso = a.DataInicioCurso,
                DataEncerramentoCurso = a.DataEncerramentoCurso,
                CargaHoraria = a.CargaHoraria,
                CursoId = a.CursoId,
                MembroId = a.MembroId,
                DescricaoArquivo = a.Descricao,
                NomeArmazenado = a.NomeArmazenado,
                NomeOriginal = a.NomeOriginal,
                Tamanho = a.Tamanho,
                Cadastrado = a.CursoId > 0 ? SimNao.Sim : SimNao.Nao,
                IsCurso = a.TipoArquivo == TipoArquivoMembro.Curso,
                Index = (int)a.Id,
                IsSave = true
            }));
            return membroVm;
        }

        private Membro DeParaVmToModel(MembroViewModel membroVM)
        {
            var membro = _mapper.Map<Membro>(membroVM);
            membro.Status = Status.PendenteAprovacao;
            membro.TipoMembro = TipoMembro.Membro;
            membro.CriadoPorId = UserAppContext.Current.Usuario.Id;
            membro.Congregacao.Id = membroVM.CongregacaoId;
            membro.Conjuge = new Membro
            {
                Id = membroVM.Pessoa.IdConjuge != null ? (long)membroVM.Pessoa.IdConjuge : 0,
                Nome = membroVM.Pessoa.NomeConjuge
            };

            membro.Pai = new Membro()
            {
                Id = membroVM.Pessoa.PaiId != null ? (long)membroVM.Pessoa.PaiId : 0,
                Nome = membroVM.Pessoa.NomePai
            };

            membro.Mae = new Membro()
            {
                Id = membroVM.Pessoa.MaeId != null ? (long)membroVM.Pessoa.MaeId : 0,
                Nome = membroVM.Pessoa.NomeMae
            };

            if (UserAppContext.Current.Usuario.Congregacao.Sede)
            {
                membro.Status = Status.Ativo;
            }

            if (membroVM.Situacoes.Any())
            {
                membroVM.Situacoes.ForEach(s => membro.Situacoes.Add(new SituacaoMembro()
                {
                    Situacao = s.IdSit.GetValueOrDefault(),
                    Observacao = s.ObservacaoSit,
                    Data = Convert.ToDateTime(s.DataSit)
                }));
            }

            if (membroVM.Cargos.Any())
            {
                membroVM.Cargos.ForEach(c => membro.Cargos.Add(new CargoMembro()
                {
                    CargoId = (int)c.CargoId,
                    Descricao = c.DescricaoCargo,
                    LocalConsagracao = c.LocalConsagracao,
                    DataCargo = Convert.ToDateTime(c.DataCargo),
                    Confradesp = c.Confradesp,
                    CGADB = c.CGADB
                }));
            }

            if (membroVM.Observacoes.Any())
            {
                membroVM.Observacoes.ForEach(o => membro.Observacoes.Add(new ObservacaoMembro()
                {
                    Observacao = o.Observacao,
                    DataCadastro = Convert.ToDateTime(o.DataCadastroObs),
                    Usuario = new Usuario() { Id = o.IdResponsavelObs }
                }));
            }

            membro.Endereco.Pais = membroVM.Endereco.Pais;
            if (!string.IsNullOrWhiteSpace(membroVM.Endereco.Pais) && membroVM.Endereco.Pais != "Brasil")
            {
                membro.Endereco.Cep = membroVM.Endereco.CodigoPostal;
                membro.Endereco.Cidade = membroVM.Endereco.CidadeEstrangeiro;
                membro.Endereco.Bairro = membroVM.Endereco.BairroEstrangeiro;
                membro.Endereco.Estado = membroVM.Endereco.Provincia;
                membro.TelefoneCelular = membroVM.Pessoa.TelefoneCelularEstrangeiro;
                membro.TelefoneComercial = membroVM.Pessoa.TelefoneComercialEstrangeiro;
                membro.TelefoneResidencial = membroVM.Pessoa.TelefoneResidencialEstrangeiro;
            }
            else
            {
                membro.Endereco.Cep = membroVM.Endereco.Cep;
                membro.Endereco.Bairro = membroVM.Endereco.Bairro;
                membro.Endereco.Cidade = membroVM.Endereco.Cidade;
                membro.Endereco.Estado = membroVM.Endereco.Estado.ToString();
            }
            return membro;
        }

        [Action(Menu.Membro)]
        public ActionResult Index(Status status = Status.NaoDefinido)
        {
            var membroVM = new IndexMembroVM();
            var _congregacoes = new List<SelectListItem>();
            foreach (var cong in _congrAppService.GetAll(UserAppContext.Current.Usuario.Id).OrderBy(p => p.Nome))
            {
                _congregacoes.Add(new SelectListItem()
                {
                    Text = cong.Nome,
                    Value = cong.Id.ToString()
                });
            }
            membroVM.ListaCongregacoes = _congregacoes;

            IEnumerable<Status> lStatus = Enum.GetValues(typeof(Status))
                                                       .Cast<Status>();
            var selectlst = (from item in lStatus
                             select new SelectListItem
                             {
                                 Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                 Value = ((int)item).ToString()
                             }).ToList();

            membroVM.ListaStatusMembro = selectlst.ToList();
            membroVM.StatusMembroSelecionado = Convert.ToInt16(status);
            return View(membroVM);
        }

        [HttpPost]
        public JsonResult GetList([FromServices] ILogger<MembroController> logger, string filtro = "", string conteudo = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                jtSorting = jtSorting == "Congregacao" ? jtSorting : jtSorting.Replace("Congregacao", "CongregacaoId");

                if (filtro == "Status" && conteudo == "0")
                    conteudo = string.Empty;

                var membros = _membroAppService.ListarMembroPaginado(jtPageSize, jtStartIndex, out int qtdRows, jtSorting, filtro, conteudo, UserAppContext.Current.Usuario.Id, TipoMembro.Membro).ToList();
                var membrosVM = new List<GridMembroItem>();
                membros.ForEach(p => membrosVM.Add(new GridMembroItem()
                {
                    Id = p.Id,
                    Congregacao = p.Congregacao.Nome,
                    Nome = p.Nome,
                    NomeMae = p.NomeMae,
                    Cpf = p.Cpf,
                    Status = p.Status.GetDisplayAttributeValue(),
                    Sede = UserAppContext.Current.Usuario.Congregacao.Sede,
                    PermiteAprovarMembro = UserAppContext.Current.Usuario.PermiteAprovarMembro,
                    PermiteImprimirCarteirinha = UserAppContext.Current.Usuario.PermiteImprimirCarteirinha
                }));

                return Json(new { Result = "OK", Records = membrosVM, TotalRecordCount = qtdRows });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        public ActionResult Details(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Membro_Details&valor=0");

            var membro = _membroAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (membro == null || membro.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var membroVm = DeParaModelToVM(membro, Acao.Read, true);
            var paises = _membroAppService.ConsultarPaises();
            membroVm.Endereco.SelectPaises = paises.Select(e => new SelectListItem
            {
                Value = e.Nome,
                Text = e.Nome
            });
            return View(membroVm);
        }

        public ActionResult Create()
        {
            var membroVm = new MembroViewModel();

            if (UserAppContext.Current.Usuario.Congregacao.Id > 0)
            {
                membroVm.CongregacaoId = UserAppContext.Current.Usuario.Congregacao.Id;
                membroVm.CongregacaoNome = UserAppContext.Current.Usuario.Congregacao.Nome;
            }
            membroVm.Cargo.SelectCargos = _cargoAppService.GetAll(UserAppContext.Current.Usuario.Id).
                ToSelectList("Id", "Descricao");
            membroVm.CursoMembro.SelectCursos = _cursoAppService.GetAll(UserAppContext.Current.Usuario.Id).
                ToSelectList("Id", "Descricao");
            membroVm.Acao = Acao.Create;

            membroVm.Situacao.SelectMembroSituacao = Enum.GetValues(typeof(MembroSituacao))
                      .Cast<MembroSituacao>()
                      .Where(e => e != MembroSituacao.NaoDefinido)
                      .Select(e => new SelectListItem
                      {
                          Value = ((int)e).ToString(),
                          Text = e.GetDisplayAttributeValue()
                      });

            membroVm.Pessoa.SelectEstadoCivil = Enum.GetValues(typeof(EstadoCivil))
                     .Cast<EstadoCivil>()
                     .Select(e => new SelectListItem
                     {
                         Value = ((int)e).ToString(),
                         Text = e.GetDisplayAttributeValue()
                     });

            membroVm.PermiteExcluirCargoMembro = true;
            membroVm.PermiteExcluirObservacaoMembro = true;
            membroVm.PermiteExcluirSituacaoMembro = true;

            var paises = _membroAppService.ConsultarPaises();
            membroVm.Endereco.SelectPaises = paises.Select(e => new SelectListItem
            {
                Value = e.Nome,
                Text = e.Nome
            });

            return View(membroVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Create([FromServices] IMemoryCache _cache,
            [FromServices] ILogger<MembroController> logger,
            [FromServices] IDashboardAppService _dashboardApp,
            [FromServices] IEventosAppService _eventoAppService,
            MembroViewModel membroVm)
        {
            return AddUpdateMembro(logger, membroVm, _cache, _dashboardApp, _eventoAppService);
        }

        public ActionResult Edit(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Membro_Edit&valor=0");

            var membro = _membroAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (membro == null || membro.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var membroVm = DeParaModelToVM(membro, Acao.Update);
            membroVm.Cargo.SelectCargos = _cargoAppService.GetAll(UserAppContext.Current.Usuario.Id).
                ToSelectList("Id", "Descricao");
            membroVm.CursoMembro.SelectCursos = _cursoAppService.GetAll(UserAppContext.Current.Usuario.Id).
                ToSelectList("Id", "Descricao");

            membroVm.Situacao.SelectMembroSituacao = Enum.GetValues(typeof(MembroSituacao))
                       .Cast<MembroSituacao>()
                       .Where(e => e != MembroSituacao.NaoDefinido)
                       .Select(e => new SelectListItem
                       {
                           Value = ((int)e).ToString(),
                           Text = e.ToString()
                       });

            membroVm.Pessoa.SelectEstadoCivil = Enum.GetValues(typeof(EstadoCivil))
                     .Cast<EstadoCivil>()
                     .Select(e => new SelectListItem
                     {
                         Value = ((int)e).ToString(),
                         Text = e.GetDisplayAttributeValue()
                     });
            membroVm.PermiteExcluirCargoMembro = UserAppContext.Current.Usuario.PermiteExcluirCargoMembro;
            membroVm.PermiteExcluirObservacaoMembro = UserAppContext.Current.Usuario.PermiteExcluirObservacaoMembro;
            membroVm.PermiteExcluirSituacaoMembro = UserAppContext.Current.Usuario.PermiteExcluirSituacaoMembro;

            if (!string.IsNullOrWhiteSpace(membro.FotoUrl))
            {
                var arquivo = _arqAzureService.DownloadFromUrl(membro.FotoUrl);
                if (arquivo.StatusRetorno == TipoStatusRetorno.OK)
                {
                    arquivo.BlobStream.Position = 0;
                    byte[] bytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        arquivo.BlobStream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }
                    var tipo = "data:image/jpeg;base64,";
                    if (membro.FotoUrl.IndexOf("png") > 0)
                        tipo = "data:image/png;base64,";
                    string base64 = tipo + Convert.ToBase64String(bytes);
                    membroVm.Pessoa.FotoPath = base64;
                }
            }

            var paises = _membroAppService.ConsultarPaises();
            membroVm.Endereco.SelectPaises = paises.Select(e => new SelectListItem
            {
                Value = e.Nome,
                Text = e.Nome
            });

            return View(membroVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Edit([FromServices] IMemoryCache _cache,
            [FromServices] IDashboardAppService _dashboardApp,
            [FromServices] IEventosAppService _eventoAppService,
            [FromServices] ILogger<MembroController> logger,
            MembroViewModel membroVm)
        {
            return AddUpdateMembro(logger, membroVm, _cache, _dashboardApp, _eventoAppService);
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
        private JsonResult AddUpdateMembro(ILogger<MembroController> logger, MembroViewModel membroVm, IMemoryCache _cache,
            IDashboardAppService _dashboardApp, IEventosAppService _eventoAppService)
        {
            try
            {
                var msgErro = new List<string>();
                if (string.IsNullOrEmpty(membroVm.Pessoa.Nome))
                    msgErro.Add($"Nome é de preenchimento");

                if (string.IsNullOrEmpty(membroVm.Pessoa.NomeMae))
                    msgErro.Add($"Nome da Mãe é de preenchimento obrigatório");

                if (!membroVm.Situacoes.Any())
                    msgErro.Add("O membro deve ter pelo menos uma Situação incluida.");
                else
                {
                    var ultSit = membroVm.Situacoes.OrderByDescending(p => Convert.ToDateTime(p.DataSit)).First();
                    if (ultSit.IdSit == MembroSituacao.Comunhao || membroVm.Acao == Acao.Create)
                    {
                        if (string.IsNullOrEmpty(membroVm.Pessoa.Cpf))
                            msgErro.Add($"CPF é de preenchimento obrigatório");

                        if (string.IsNullOrEmpty(membroVm.Pessoa.Rg))
                            msgErro.Add($"RG é de preenchimento obrigatório");
                    }
                }

                if (!string.IsNullOrWhiteSpace(membroVm.Pessoa.Cpf))
                {
                    var cpf = _membroAppService.ExisteCPFDuplicado(membroVm.Id, membroVm.Pessoa.Cpf);
                    if (cpf != null && cpf.Id > 0)
                        msgErro.Add($"CPF já cadastrado para um Membro na Congregação {cpf.Congregacao.Nome} com o Código {cpf.Id}.");
                }

                if (!string.IsNullOrWhiteSpace(membroVm.Pessoa.Cpf) && !ValidarCPF(membroVm.Pessoa.Cpf))
                    msgErro.Add($"CPF {membroVm.Pessoa.Cpf} é incorreto ou inválido.");


                if (membroVm.Pessoa.DataNascimento == null || membroVm.Pessoa.DataNascimento == DateTime.MinValue)
                    msgErro.Add("Data de Recepção é de preenchimento obrigatório");

                if (string.IsNullOrEmpty(membroVm.Endereco.Logradouro))
                    msgErro.Add($"Logradouro é de preenchimento obrigatório");

                if (string.IsNullOrEmpty(membroVm.Endereco.Pais))
                    msgErro.Add($"Pais é de preenchimento obrigatório");

                if (membroVm.Endereco.Pais != "Brasil")
                {
                    if (string.IsNullOrEmpty(membroVm.Endereco.CidadeEstrangeiro))
                        msgErro.Add($"Cidade é de preenchimento obrigatório");

                    if (string.IsNullOrEmpty(membroVm.Endereco.Provincia))
                        msgErro.Add($"Província/Estado é de preenchimento obrigatório");
                }
                else
                {
                    if (string.IsNullOrEmpty(membroVm.Endereco.Bairro))
                        msgErro.Add($"Bairro é de preenchimento obrigatório");

                    if (string.IsNullOrEmpty(membroVm.Endereco.Cidade))
                        msgErro.Add($"Cidade é de preenchimento obrigatório");

                    if (string.IsNullOrEmpty(membroVm.Endereco.Estado.ToString()))
                        msgErro.Add($"Estado é de preenchimento obrigatório");
                }

                if (membroVm.DataRecepcao == null || membroVm.DataRecepcao == DateTime.MinValue)
                    msgErro.Add("Data de Recepção é de preenchimento obrigatório");

                if (membroVm.DataBatismoAguas == null || membroVm.DataBatismoAguas == DateTime.MinValue)
                    msgErro.Add("Data de Batismo nas Águas é de preenchimento obrigatório");

                if (msgErro.Count > 0)
                    throw new Erro(string.Join("<br/>", msgErro));

                var membro = DeParaVmToModel(membroVm);
                membroVm.Id = _membroAppService.Add(membro);

                membroVm.Id = membro.Id;
                var msgAlert = $"Membro {membroVm.Pessoa.Nome} incluído com sucesso! Código: {membroVm.Id}";
                if (membroVm.Acao == Acao.Update)
                    msgAlert = $"Membro {membroVm.Pessoa.Nome} atualizado com sucesso! Código: {membroVm.Id}";


                /*Atualizando o cache com o Dashboard*/
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                var dashs = _dashboardApp.RetornaDadosDashboard((int)UserAppContext.Current.Usuario.Id);

                var dash = new EventosDashBoardVM
                {
                    Dashboard = dashs,
                    Eventos = _eventoAppService.ListarEventosPorData(DateTime.Now.Date, DateTime.Now.AddDays(30).Date, out List<Feriado> feriados),
                    Feriados = feriados
                };
                _cache.Set($"Dashboard_{UserAppContext.Current.Usuario.Username}", dash, cacheEntryOptions);

                this.ShowMessage("Sucesso", msgAlert, AlertType.Success);

                return Json(new { status = "OK", membroid = membroVm.Id, msg = msgAlert, url = Url.Action("Index", "Membro", new { Area = "Secretaria" }) });
            }
            catch (Erro ex)
            {
                var msgAlert = $"Falha ao incluir o Membro:<br/><strong>{ex.Message}</strong>";
                if (membroVm.Acao == Acao.Update)
                    msgAlert = $"Falha ao atualizar o Membro:{membroVm?.Id}<br/><strong>{ex.Message}</strong>";

                return Json(new { status = "Erro", membroid = membroVm?.Id, msg = msgAlert, url = Url.Action("Index", "Membro", new { Area = "Secretaria" }) });
            }
            catch (Exception ex)
            {
                var msgAlert = $"Falha ao incluir o Membro:<br/><strong>{ex.Message}</strong>";
                if (membroVm.Acao == Acao.Update)
                    msgAlert = $"Falha ao atualizar o Membro:{membroVm?.Id}<br/><strong>{ex.Message}</strong>";
                logger.LogError(ex, $"Usuario - {HttpContext.User.Identity.Name}");
                return Json(new { status = "Erro", membroid = membroVm?.Id, msg = msgAlert, url = Url.Action("Index", "Membro", new { Area = "Secretaria" }) });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public async Task<JsonResult> DeleteAsync([FromServices] ILogger<MembroController> logger,
            [FromServices] ICongregacaoAppService _congregService,
            long id)
        {
            try
            {
                if (!UserAppContext.Current.Usuario.PermiteExcluirMembros)
                    throw new Erro("Usuário sem permissão para excluir Membros.");

                var congregacao = _congregService.ListarObreirosCongregacaoPorMembroId(id);
                if (congregacao != null && congregacao.Count() > 0)
                {
                    throw new Erro($"Não é possível excluir o Membro, pois o mesmo está cadastrado como Obreiro na Congregação {congregacao.FirstOrDefault().CongregacaoNome}");
                }
                await _membroAppService.DeleteAndDeleteFilesAsync(id);
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
            this.ShowMessage("Sucesso", "Membro excluído com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Membro excluído com sucesso!", url = Url.Action("Index", "Membro") });
        }


        public ActionResult VerificaCodigoRegistroDuplicado(long Id = 0)
        {
            return Json(!((Id == 0) || (_membroAppService.ExisteCodigoRegistro(Id))));
        }

        [HttpPost]
        public JsonResult BuscarMembro(long membroId)
        {
            var membro = _membroAppService.GetById(membroId, UserAppContext.Current.Usuario.Id);

            Estado estado;
            if (Enum.TryParse<Estado>(membro.Endereco.Estado, true, out estado))
                membro.Endereco.Estado = Convert.ToInt64(estado).ToString();

            return Json(membro);
        }

        [HttpPost]
        public JsonResult BuscarMembroGeral(long membroId)
        {
            var membro = _membroAppService.GetById(membroId, 0);

            if (Enum.TryParse(membro.Endereco.Estado, true, out Estado estado))
                membro.Endereco.Estado = Convert.ToInt64(estado).ToString();

            return Json(membro);
        }

        public JsonResult VerificaCpfDuplicado(string cpf, long? id)
        {
            var membro = _membroAppService.ExisteCPFDuplicado(Convert.ToInt64(id), cpf);
            if (membro != null && membro.Id > 0)
                return Json(new { status = "ERRO", mensagem = $"CPF já cadastrado para um Membro na Congregação {membro.Congregacao.Nome} com o Código {membro.Id}." });
            return Json(new { status = "OK", mensagem = "" });
        }

        public ActionResult AprovarReprovar(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Membro_AprovarReprovar&valor=0");
            if (UserAppContext.Current.Usuario.PermiteAprovarMembro)
            {
                var membro = _membroAppService.GetById(id, UserAppContext.Current.Usuario.Id);
                if (membro == null || membro.Id == 0)
                    return Redirect("/Auth/NaoAutorizado");

                return View(DeParaModelToVM(membro, Acao.Read, true));
            }
            return RedirectToAction("Edit", "Membro", new { Area = "Secretaria", Id = id });
        }

        public ActionResult ReprovarMembroMotivo(long id)
        {
            var reprova = new ReprovarViewModel();
            reprova.Id = id;
            return View(reprova);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult AprovarMembro([FromServices] IMemoryCache _cache,
            [FromServices] IDashboardAppService _dashboardApp,
            [FromServices] IEventosAppService _eventoAppService,
            [FromServices] ILogger<MembroController> logger,
            long id)
        {
            try
            {
                _membroAppService.AprovarReprovaMembro(id, UserAppContext.Current.Usuario.Id, Status.Ativo, "");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "Erro", mensagem = ex.Message, url = "" });
            }

            /*Atualizando o cache com o Dashboard*/
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            var dashs = _dashboardApp.RetornaDadosDashboard((int)UserAppContext.Current.Usuario.Id);

            var dash = new EventosDashBoardVM
            {
                Dashboard = dashs,
                Eventos = _eventoAppService.ListarEventosPorData(DateTime.Now.Date, DateTime.Now.AddDays(30).Date, out List<Feriado> feriados),
                Feriados = feriados
            };
            _cache.Set($"Dashboard_{UserAppContext.Current.Usuario.Username}", dash, cacheEntryOptions);

            /*Carregando o cache com o Dashboard*/
            var url = Url.Action("Index", "Membro", new { Area = "Secretaria" });
            var qtdPend = dashs.SituacaoMembro.Where(s => s.Status == Status.PendenteAprovacao).FirstOrDefault();
            if (qtdPend != null && qtdPend.Quantidade > 0)
                url = Url.Action("Index", "Membro", new { Area = "Secretaria", Status = 3 });

            this.ShowMessage("Sucesso", "Membro Aprovado com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Membro Aprovado com sucesso!", url });
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult ReprovarMembro([FromServices] IMemoryCache _cache,
            [FromServices] IDashboardAppService _dashboardApp,
            [FromServices] IEventosAppService _eventoAppService,
            [FromServices] ILogger<MembroController> logger,
            long id,
            string motivoReprovacao)
        {
            try
            {
                _membroAppService.AprovarReprovaMembro(id, UserAppContext.Current.Usuario.Id, Status.NaoAprovado, motivoReprovacao);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { erro = ex.Message });
            }

            /*Atualizando o cache com o Dashboard*/
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            var dashs = _dashboardApp.RetornaDadosDashboard((int)UserAppContext.Current.Usuario.Id);

            var dash = new EventosDashBoardVM
            {
                Dashboard = dashs,
                Eventos = _eventoAppService.ListarEventosPorData(DateTime.Now.Date, DateTime.Now.AddDays(30).Date, out List<Feriado> feriados),
                Feriados = feriados
            };
            _cache.Set($"Dashboard_{UserAppContext.Current.Usuario.Username}", dash, cacheEntryOptions);

            /*Carregando o cache com o Dashboard*/
            var url = Url.Action("Index", "Membro", new { Area = "Secretaria" });
            var qtdPend = dashs.SituacaoMembro.Where(s => s.Status == Status.PendenteAprovacao).FirstOrDefault();
            if (qtdPend != null && qtdPend.Quantidade > 0)
                url = Url.Action("Index", "Membro", new { Area = "Secretaria", Status = 3 });

            this.ShowMessage("Atenção", "Membro reprovado. A congregação será notificada!", AlertType.Success);
            return Json(new { erro = "", url });
        }


        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public FileStreamResult CarteirinhaMembros([FromServices] IImpressaoMembroAppService carteirinhaAppService,
                                                   [FromServices] ILogger<MembroController> logger,
                                                   int id,
                                                   bool atualizaValidade)
        {
            try
            {
                var membro = new List<int>() { id };
                var carts = _membroAppService.ListarCarteirinhaMembros(membro.ToArray());

                if (carts.Any())
                {
                    var doc = carteirinhaAppService.GerarCarteirinha(carts, atualizaValidade);

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
        
        public JsonResult VerificaVencimentoCarteirinha(long id)
        {
            var cart = _membroAppService.CarteirinhaMembro(id).FirstOrDefault();

            if (cart.TipoCarteirinha != TipoCarteirinha.Membro &&
                !string.IsNullOrWhiteSpace(cart.DataValidadeCarteirinha))
            {
                return Json(new { DataValidade = cart.DataValidadeCarteirinha });
            }
            return Json(new { DataValidade = "" });
        }

        public JsonResult DadosCargos()
        {
            return Json(_cargoAppService.GetAll(UserAppContext.Current.Usuario.Id));
        }

        public JsonResult DadosCursos()
        {
            return Json(_cursoAppService.GetAll(UserAppContext.Current.Usuario.Id));
        }
        public FileStreamResult FichaMembro([FromServices] IImpressaoMembroAppService fichaAppService,
                                            [FromServices] ILogger<MembroController> logger, 
                                            int id,
                                            bool imprimirCurso = false)
        {
            try
            {
                var document = fichaAppService.GerarFichaMembro(id, imprimirCurso);

                var filename = $"FichaMembro_{id}.pdf";
                var stream = new MemoryStream();
                document.Save(stream, false);
                return GerarArquivoDownload(stream.ToArray(), "application/pdf", filename, logger);
            }
            catch (Erro ex)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                string messages = System.Text.Json.JsonSerializer.Serialize(new
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

        
        private FileStreamResult GerarArquivoDownload(
            byte[] relatorio, string mimeType, string filename,
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

        public FileStreamResult DownloadArquivo([FromServices] ILogger<MembroController> logger, int id)
        {
            try
            {
                var cursoMembro = _arquivoAppService.GetById(id, UserAppContext.Current.Usuario.Id);
                var arq = _arquivoAppService.DownloadFileAsync(cursoMembro).Result;
                if (arq.StatusRetorno == TipoStatusRetorno.OK)
                {
                    var fileStreamResult = new FileStreamResult(arq.BlobStream, arq.ContentType)
                    {
                        FileDownloadName = cursoMembro.NomeOriginal
                    };
                    Response.StatusCode = StatusCodes.Status200OK;
                    return fileStreamResult;
                }
                else
                    throw new Erro("Não foi possível fazer o download do arquivo selecionado.");
            }
            catch (Erro ex)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                string messages = System.Text.Json.JsonSerializer.Serialize(new
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

        public async Task<JsonResult> SalvarExcluirArquivosAsync([FromServices] ILogger<MembroController> logger,
            IList<IFormFile> files, string model)
        {
            var arquivo = new ArquivoMembroVM();
            try
            {
                arquivo = JsonConvert.DeserializeObject<ArquivoMembroVM>(model,
                       new JsonSerializerSettings
                       {
                           DateFormatString = "dd/MM/yyyy",
                           NullValueHandling = NullValueHandling.Ignore
                       });

                if (arquivo.IsDelete && arquivo.IsSave)
                {
                    await _arquivoAppService.DeleteFileAsync(new CursoArquivoMembro() { Id = arquivo.Id.Value });
                }
                else if (!arquivo.IsDelete && !arquivo.IsSave)
                {
                    foreach (IFormFile file in files)
                    {
                        var format = file.FileName.Trim('\"');

                        if (file.Length > 0)
                        {
                            arquivo.ArquivoUpload = file.OpenReadStream();
                            arquivo.Tamanho = file.Length;
                            arquivo.MimeType = file.ContentType;
                        }
                    }

                    _arquivoAppService.UploadFile(
                        new CursoArquivoMembro()
                        {
                            Id = 0,
                            MembroId = arquivo.MembroId.Value,
                            TipoArquivo = arquivo.IsCurso ? TipoArquivoMembro.Curso : TipoArquivoMembro.Arquivo,
                            Descricao = arquivo.DescricaoArquivo,
                            CursoId = (int)arquivo.CursoId.GetValueOrDefault(0),
                            NomeCurso = arquivo.CursoId.GetValueOrDefault(0) == 0 ? arquivo.NomeCurso : "",
                            Local = arquivo.CursoId.GetValueOrDefault(0) == 0 ? arquivo.Local : "",
                            DataInicioCurso = arquivo.CursoId.GetValueOrDefault(0) == 0 ? arquivo.DataInicioCurso.GetValueOrDefault() : DateTimeOffset.MinValue,
                            DataEncerramentoCurso = arquivo.CursoId.GetValueOrDefault(0) == 0 ? arquivo.DataEncerramentoCurso.GetValueOrDefault() : DateTimeOffset.MinValue,
                            CargaHoraria = arquivo.CursoId.GetValueOrDefault(0) == 0 ? arquivo.CargaHoraria.GetValueOrDefault(0) : 0,
                            NomeOriginal = arquivo.NomeOriginal,
                            Tamanho = arquivo.Tamanho,
                            Arquivo = arquivo.ArquivoUpload,
                            ContentType = arquivo.MimeType
                        });

                }
                if (arquivo.IsCurso)
                {
                    if (arquivo.IsDelete && arquivo.IsSave)
                        return Json(new { status = "OK", msg = $"Curso '<b>{arquivo.NomeCurso}</b>' excluído com sucesso.", index = arquivo.Index });
                    return Json(new { status = "OK", msg = $"Curso '<b>{arquivo.NomeCurso}</b>' salvo com sucesso.", index = arquivo.Index });
                }
                else
                {
                    if (arquivo.IsDelete)
                        return Json(new { status = "OK", msg = $"Arquivo '<b>{arquivo.DescricaoArquivo}</b>' excluído com sucesso.", index = arquivo.Index });
                    return Json(new { status = "OK", msg = $"Arquivo '<b>{arquivo.DescricaoArquivo}</b>' salvo com sucesso.", index = arquivo.Index });
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                if (arquivo.IsCurso)
                    return Json(new { status = "Erro", msg = $"Falha ao salvar {arquivo?.NomeCurso}. Erro: {ex.Message}", index = arquivo.Index });
                else
                    return Json(new { status = "Erro", msg = $"Falha ao salvar {arquivo?.DescricaoArquivo}. Erro: {ex.Message}", index = arquivo.Index });
            }
        }

        public ActionResult MembroConfirmado(string id)
        {
            var membro = _membroAppService.GetMembroConfirmado(Convert.ToInt32(id));
            var membroVM = DeParaModelToVMMembroConfirmado(membro);

            return View(membroVM);
        }

        private HistoricoMembroVM DeParaModelToVMMembroConfirmado(Dictionary<string, Membro> membros)
        {

            var membro = membros["H"];
            var membroVm = new HistoricoMembroVM
            {
                BairroAnterior = membro.Endereco.Bairro,
                CepAnterior = membro.Endereco.Cep,
                CidadeAnterior = membro.Endereco.Cidade,
                ComplementoAnterior = membro.Endereco.Complemento,
                CpfAnterior = membro.Cpf,
                DataAlteracao = membro.DataAlteracao,
                EmailAnterior = membro.Email,
                EstadoAnterior = membro.Endereco.Estado,
                IpConfirmado = membro.IpConfirmado,
                LogradouroAnterior = membro.Endereco.Logradouro,
                MembroConfirmado = membro.MembroConfirmado == 1,
                NacionalidadeAnterior = membro.Nacionalidade,
                NaturalidadeCidadeAnterior = membro.NaturalidadeCidade,
                NaturalidadeEstadoAnterior = membro.NaturalidadeEstado,
                NomeAnterior = membro.Nome,
                NomeMaeAnterior = membro.NomeMae,
                NomePaiAnterior = membro.NomePai,
                NumeroAnterior = membro.Endereco.Numero,
                OrgaoEmissorAnterior = membro.OrgaoEmissor,
                PaisAnterior = membro.Endereco.Pais,
                ProfissaoAnterior = membro.Profissao,
                RgAnterior = membro.RG,
                TelefoneCelularAnterior = membro.TelefoneCelular,
                TelefoneComercialAnterior = membro.TelefoneComercial,
                TelefoneResidencialAnterior = membro.TelefoneResidencial,
                DataNascimentoAnterior = membro.DataNascimento.HasValue ? membro.DataNascimento.Value.UtcDateTime.ToShortDateString() : string.Empty
            };
            if (membro.EstadoCivil != EstadoCivil.NaoDefinido)
                membroVm.EstadoCivilAnterior = membro.EstadoCivil.GetDisplayAttributeValue();
            if (Convert.ToInt16(membro.Escolaridade) > 0)
                membroVm.EscolaridadeAnterior = membro.Escolaridade.GetDisplayAttributeValue();
            if (Convert.ToInt16(membro.Sexo) > 0)
                membroVm.SexoAnterior = membro.Sexo.GetDisplayAttributeValue();

            membro = membros["A"];
            membroVm.Id = membro.Id;
            membroVm.Bairro = membro.Endereco.Bairro;
            membroVm.Cep = membro.Endereco.Cep;
            membroVm.Cidade = membro.Endereco.Cidade;
            membroVm.Complemento = membro.Endereco.Complemento;
            membroVm.Cpf = membro.Cpf;
            membroVm.Email = membro.Email;
            membroVm.Estado = membro.Endereco.Estado;
            membroVm.Logradouro = membro.Endereco.Logradouro;
            membroVm.MembroConfirmado = membro.MembroConfirmado == 1;
            membroVm.Nacionalidade = membro.Nacionalidade;
            membroVm.NaturalidadeCidade = membro.NaturalidadeCidade;
            membroVm.NaturalidadeEstado = membro.NaturalidadeEstado;
            membroVm.Nome = membro.Nome;
            membroVm.NomeMae = membro.NomeMae;
            membroVm.NomePai = membro.NomePai;
            membroVm.Numero = membro.Endereco.Numero;
            membroVm.OrgaoEmissor = membro.OrgaoEmissor;
            membroVm.Pais = membro.Endereco.Pais;
            membroVm.Profissao = membro.Profissao;
            membroVm.Rg = membro.RG;
            membroVm.TelefoneCelular = membro.TelefoneCelular;
            membroVm.TelefoneComercial = membro.TelefoneComercial;
            membroVm.TelefoneResidencial = membro.TelefoneResidencial;
            membroVm.DataNascimento = membro.DataNascimento.HasValue ? membro.DataNascimento.Value.UtcDateTime.ToShortDateString() : null;
            if (membro.EstadoCivil != EstadoCivil.NaoDefinido)
                membroVm.EstadoCivil = membro.EstadoCivil.GetDisplayAttributeValue();
            if (Convert.ToInt16(membro.Escolaridade) > 0)
                membroVm.Escolaridade = membro.Escolaridade.GetDisplayAttributeValue();
            if (Convert.ToInt16(membro.Sexo) > 0)
                membroVm.Sexo = membro.Sexo.GetDisplayAttributeValue();

            return membroVm;
        }

        public ActionResult RestaurarMembro(long membroId, string campos)
        {
            try
            {
                _membroAppService.RestaurarMembroConfirmado(membroId, campos, UserAppContext.Current.Usuario.Id);
                this.ShowMessage("Membro", "Membro restaurado com Sucesso!");
                return Json(new { status = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "ERRO", mensagem = ex.Message });
            }
        }
    }
}
