using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Batismo;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.PastorCelebrante;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Filters.Menu;
using ASChurchManager.Web.Lib;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ASChurchManager.Web.Areas.Secretaria.Controllers
{
    [Rotina(Area.Secretaria), ControllerAuthorize("Batismo")]
    public class BatismoController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IMembroAppService _membroAppService;
        private readonly IBatismoAppService _batismoAppService;
        private readonly ICongregacaoAppService _congrAppService;
        private readonly IArquivosAzureAppService _arqAzureService;
        private ICursoArquivoMembroAppService _arquivoAppService;

        public BatismoController(IMembroAppService appService
                                , IBatismoAppService batismoAppService
                                , ICongregacaoAppService congrAppService
                                , IMapper mapper
                                , IMemoryCache cache
                                , IUsuarioLogado usuLog
                                , IConfiguration _configuration
                                , IRotinaAppService _rotinaAppService
                                , IArquivosAzureAppService arquivosAzureAppService
                                , ICursoArquivoMembroAppService arquivoAppService)
            : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _membroAppService = appService;
            _batismoAppService = batismoAppService;
            _congrAppService = congrAppService;
            _mapper = mapper;
            _arqAzureService = arquivosAzureAppService;
            _arquivoAppService = arquivoAppService;
        }

        private BatismoViewModel DeParaModelToVM(Membro membro, Acao acao, bool readOnly = false)
        {
            var membroVm = _mapper.Map<BatismoViewModel>(membro);
            membroVm.TipoMembro = TipoMembro.Batismo;
            membroVm.Acao = acao;
            membroVm.IsReadOnly = readOnly;
            membroVm.Pessoa.IsReadOnly = readOnly;
            membroVm.Endereco.IsReadOnly = readOnly;
            membroVm.Observacao.IsReadOnly = readOnly;
            membroVm.CursoMembro.IsReadOnly = readOnly;

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
                membroVm.Pessoa.NaturalidadeEstado = (Estado)Enum.Parse(typeof(Estado), membro.NaturalidadeEstado);
            membroVm.Pessoa.SelectEstadoCivil = Enum.GetValues(typeof(EstadoCivil))
                     .Cast<EstadoCivil>()
                     .Select(e => new SelectListItem
                     {
                         Value = ((int)e).ToString(),
                         Text = e.GetDisplayAttributeValue()
                     });

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

            membroVm.PermiteExcluirObservacaoMembro = UserAppContext.Current.Usuario.PermiteExcluirObservacaoMembro;
            return membroVm;
        }

        private Membro DeParaVmToModel(BatismoViewModel model)
        {
            var membro = _mapper.Map<Membro>(model);

            membro.Status = Status.Ativo;
            membro.TipoMembro = TipoMembro.Batismo;
            membro.CriadoPorId = UserAppContext.Current.Usuario.Id;
            membro.Congregacao.Id = model.CongregacaoId;
            membro.Conjuge = new Membro
            {
                Id = model.Pessoa.IdConjuge != null ? (long)model.Pessoa.IdConjuge : 0,
                Nome = model.Pessoa.NomeConjuge
            };

            membro.Pai = new Membro()
            {
                Id = model.Pessoa.PaiId != null ? (long)model.Pessoa.PaiId : 0,
                Nome = model.Pessoa.NomePai
            };

            membro.Mae = new Membro()
            {
                Id = model.Pessoa.MaeId != null ? (long)model.Pessoa.MaeId : 0,
                Nome = model.Pessoa.NomeMae
            };

            membro.DataPrevistaBatismo = model.DataPrevistaBatismo;
            membro.BatismoId = model.BatismoId;

            if (model.Observacoes.Any())
            {
                model.Observacoes.ForEach(o => membro.Observacoes.Add(new ObservacaoMembro()
                {
                    Observacao = o.Observacao,
                    DataCadastro = Convert.ToDateTime(o.DataCadastroObs),
                    Usuario = new Usuario() { Id = o.IdResponsavelObs }
                }));
            }

            membro.Endereco.Pais = model.Endereco.Pais;
            if (!string.IsNullOrWhiteSpace(model.Endereco.Pais) && model.Endereco.Pais != "Brasil")
            {
                membro.Endereco.Cep = model.Endereco.CodigoPostal;
                membro.Endereco.Bairro = model.Endereco.BairroEstrangeiro;
                membro.Endereco.Cidade = model.Endereco.CidadeEstrangeiro;
                membro.Endereco.Estado = model.Endereco.Provincia;
                membro.TelefoneCelular = model.Pessoa.TelefoneCelularEstrangeiro;
                membro.TelefoneComercial = model.Pessoa.TelefoneComercialEstrangeiro;
                membro.TelefoneResidencial = model.Pessoa.TelefoneResidencialEstrangeiro;
            }
            else
            {
                membro.Endereco.Cep = model.Endereco.Cep;
                membro.Endereco.Bairro = model.Endereco.Bairro;
                membro.Endereco.Cidade = model.Endereco.Cidade;
                membro.Endereco.Estado = model.Endereco.Estado.ToString();
            }

            return membro;
        }

        // GET: Secretaria/Membro
        [Action(Menu.Batismo, Menu.CadastroBatismo)]
        public ActionResult Index()
        {
            var batismoVm = new IndexBatismoVM();

            var batismos = _batismoAppService.GetAll(UserAppContext.Current.Usuario.Id)
                                .Where(b => b.Status == StatusBatismo.EmAberto)
                                .OrderBy(d => d.DataBatismo)
                                .ToList();


            if (batismos.Count > 1)
            {
                batismoVm.Filtros.Add(new SelectListItem() { Text = "Data do Batismo", Value = "DataBatismo" });
                batismos.ForEach(b => batismoVm.ListaDatasBatismo.Add(
                           new SelectListItem()
                           {
                               Text = b.DataBatismo.Date.ToShortDateString(),
                               Value = b.Id.ToString()
                           })
                       );
            }

            _congrAppService.GetAll(UserAppContext.Current.Usuario.Id).OrderBy(p => p.Nome).ToList()
                            .ForEach(cong => batismoVm.ListaCongregacoes.Add(
                                new SelectListItem()
                                {
                                    Text = cong.Nome,
                                    Value = cong.Id.ToString()
                                }));

            return View(batismoVm);
        }

        [HttpPost]
        public JsonResult GetList([FromServices] ILogger<BatismoController> logger,
            string filtro = "", string conteudo = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                if (jtSorting != null && jtSorting.IndexOf("DataBatismo") >= 0)
                    jtSorting = jtSorting.Replace("DataBatismo", "BatismoId");

                if (filtro == "DataBatismo")
                    filtro = "BatismoId";

                var membros = _batismoAppService.ListarCandidatosBatismoPaginada(jtPageSize, jtStartIndex, jtSorting, filtro, conteudo, UserAppContext.Current.Usuario.Id, out int qtdRows).ToList();
                var batismos = _batismoAppService.GetAll(UserAppContext.Current.Usuario.Id)
                                .Where(b => b.Status == StatusBatismo.EmAberto)
                                .OrderBy(d => d.DataBatismo)
                                .ToList();
                var membrosVM = new List<CandidatosBatismoVM>();
                membros.ForEach(p => membrosVM.Add(new CandidatosBatismoVM()
                {
                    Id = p.Id,
                    CongregacaoNome = p.Congregacao.Nome,
                    Nome = p.Nome,
                    NomeMae = p.NomeMae,
                    Cpf = p.Cpf,
                    DataNascimento = p.DataNascimento,
                    DataBatismo = batismos.FirstOrDefault(b => b.Id == p.BatismoId).DataBatismo.Date.ToShortDateString()
                }));

                return Json(new { Result = "OK", Records = membrosVM, TotalRecordCount = qtdRows });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        // GET: Secretaria/Membro/Details/5
        public ActionResult Details(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Batismo_Details&valor=0");
            var membro = _membroAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (membro == null || membro.Id == 0)
                return Redirect("/Auth/NaoAutorizado");
            return View(DeParaModelToVM(membro, Acao.Read, true));
        }

        // GET: Secretaria/Membro/Create
        public ActionResult Create()
        {
            var batismos = _batismoAppService.GetAll(UserAppContext.Current.Usuario.Id)
                        .Where(b => b.Status == StatusBatismo.EmAberto &&
                                    (UserAppContext.Current.Usuario.PermiteCadBatismoAposDataMaxima || b.DataMaximaCadastro.Date >= DateTimeOffset.Now.Date));

            if (batismos.Count() > 0)
            {
                // Inicializa o ViewModel para não dar erro na view por causa do titulo da VM
                var batismoVM = new BatismoViewModel
                {
                    Acao = Acao.Create,
                    TipoMembro = TipoMembro.Batismo
                };

                if (UserAppContext.Current.Usuario.Congregacao.Id > 0)
                {
                    batismoVM.CongregacaoId = UserAppContext.Current.Usuario.Congregacao.Id;
                    batismoVM.CongregacaoNome = UserAppContext.Current.Usuario.Congregacao.Nome;
                    batismoVM.PermiteExcluirObservacaoMembro = UserAppContext.Current.Usuario.PermiteExcluirObservacaoMembro;
                }
                batismoVM.Pessoa.SelectEstadoCivil = Enum.GetValues(typeof(EstadoCivil))
                     .Cast<EstadoCivil>()
                     .Select(e => new SelectListItem
                     {
                         Value = ((int)e).ToString(),
                         Text = e.GetDisplayAttributeValue()
                     });

                if (batismos.Count() == 1)
                {
                    batismoVM.DataPrevistaBatismo = batismos.FirstOrDefault().DataBatismo.Date;
                    batismoVM.BatismoId = (int)batismos.FirstOrDefault().Id;
                }
                else
                {
                    batismos.OrderBy(d => d.DataBatismo)
                        .ToList()
                        .ForEach(b => batismoVM.ListaDatasBatismo.Add(
                            new SelectListItem()
                            {
                                Text = b.DataBatismo.Date.ToShortDateString(),
                                Value = b.Id.ToString()
                            })
                        );
                }

                var paises = _membroAppService.ConsultarPaises();
                batismoVM.Endereco.SelectPaises = paises.Select(e => new SelectListItem
                {
                    Value = e.Nome,
                    Text = e.Nome
                });

                return View(batismoVM);
            }
            else
                this.ShowMessage("Batismo - Novo", "Não existe data cadastrada para o Batismo. Favor entrar em contato com a Sede.");

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Create([FromServices] ILogger<BatismoController> logger, BatismoViewModel membroVm)
        {
            try
            {
                ValidarMembro(membroVm);
                var membro = DeParaVmToModel(membroVm);
                _membroAppService.Add(membro);
                membroVm.Id = membro.Id;

                this.ShowMessage("Cadastro de Batismo", $"Membro '{membroVm.Pessoa.Nome}' cadastrado para o Batismo com sucesso! Código: {membroVm.Id}");
                return Json(new
                {
                    status = "OK",
                    membroid = membroVm.Id,
                    msg = $"Membro '{membroVm.Pessoa.Nome}' cadastrado para o Batismo com sucesso! Código: {membroVm.Id}",
                    url = Url.Action("Index", "Batismo", new { Area = "Secretaria" })
                });

            }
            catch (Erro ex)
            {
                return Json(new
                {
                    status = "Erro",
                    membroid = membroVm?.Id,
                    msg = $"Falha ao cadastrar o Membro para o Batismo. Erro: {ex.Message}",
                    url = Url.Action("Index", "Batismo", new { Area = "Secretaria" })
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new
                {
                    status = "Erro",
                    membroid = membroVm?.Id,
                    msg = $"Falha ao cadastrar o Membro para o Batismo. Erro: {ex.Message}",
                    url = Url.Action("Index", "Batismo", new { Area = "Secretaria" })
                });
            }
        }

        public ActionResult Edit(long id, bool enviar)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Batismo_Edit&valor=0");
            var membro = _membroAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (membro == null || membro.Id == 0)
                return Redirect("/Auth/NaoAutorizado");
            var batismoVM = DeParaModelToVM(membro, Acao.Update);
            batismoVM.EnviarBatismo = enviar;

            var batismos = _batismoAppService.GetAll(UserAppContext.Current.Usuario.Id)
                .Where(b => b.Status == StatusBatismo.EmAberto &&
                            (UserAppContext.Current.Usuario.PermiteCadBatismoAposDataMaxima || b.DataMaximaCadastro.Date >= DateTimeOffset.Now.Date));
            batismoVM.Pessoa.SelectEstadoCivil = Enum.GetValues(typeof(EstadoCivil))
                     .Cast<EstadoCivil>()
                     .Select(e => new SelectListItem
                     {
                         Value = ((int)e).ToString(),
                         Text = e.GetDisplayAttributeValue()
                     });

            if (batismos.Count() == 1)
            {
                batismoVM.DataPrevistaBatismo = batismos.FirstOrDefault().DataBatismo.Date;
                batismoVM.BatismoId = (int)batismos.FirstOrDefault().Id;
            }
            else
            {
                batismos.OrderBy(d => d.DataBatismo)
                .ToList()
                .ForEach(b => batismoVM.ListaDatasBatismo.Add(
                    new SelectListItem()
                    {
                        Text = b.DataBatismo.Date.ToShortDateString(),
                        Value = b.Id.ToString()
                    })
                );
            }

            var paises = _membroAppService.ConsultarPaises();
            batismoVM.Endereco.SelectPaises = paises.Select(e => new SelectListItem
            {
                Value = e.Nome,
                Text = e.Nome
            });
            return View(batismoVM);
        }


        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult Edit([FromServices] ILogger<BatismoController> logger, BatismoViewModel membroVm)
        {
            try
            {
                ValidarMembro(membroVm);
                var membro = DeParaVmToModel(membroVm);
                _membroAppService.Update(membro);

                membroVm.Id = membro.Id;
                this.ShowMessage("Cadastro de Batismo", $"Membro ({membroVm.Pessoa.Nome}) atualizado com sucesso!");
                return Json(new
                {
                    status = "OK",
                    membroid = membroVm.Id,
                    msg = $"Membro ({membroVm.Pessoa.Nome}) atualizado com sucesso!",
                    url = Url.Action("Index", "Batismo", new { Area = "Secretaria" })
                });

            }
            catch (Erro ex)
            {
                return Json(new
                {
                    status = "Erro",
                    membroid = membroVm?.Id,
                    msg = $"Falha ao atualizar o Membro para o Batismo. Erro: {ex.Message}",
                    url = Url.Action("Index", "Batismo", new { Area = "Secretaria" })
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new
                {
                    status = "Erro",
                    membroid = membroVm?.Id,
                    msg = $"Falha ao atualizar o Membro para o Batismo. Erro: {ex.Message}",
                    url = Url.Action("Index", "Batismo", new { Area = "Secretaria" })
                });
            }
        }

        private void ValidarMembro(BatismoViewModel membroVm)
        {
            if (membroVm == null || membroVm.Endereco == null)
                throw new Erro("Dados inválidos.");

            if (string.IsNullOrEmpty(membroVm.Pessoa.Nome))
                throw new Erro("Nome é de preenchimento obrigatório");

            if (string.IsNullOrEmpty(membroVm.Pessoa.NomeMae))
                throw new Erro("Nome da Mãe é de preenchimento obrigatório");

            if (string.IsNullOrEmpty(membroVm.Pessoa.Cpf))
                throw new Erro("CPF é de preenchimento obrigatório");

            if (string.IsNullOrEmpty(membroVm.Pessoa.Rg))
                throw new Erro("RG é de preenchimento obrigatório");

            var cpf = _membroAppService.ExisteCPFDuplicado(membroVm.Id, membroVm.Pessoa.Cpf);
            if (cpf != null && cpf.Id > 0)
                throw new Erro($"CPF já cadastrado para um Membro na Congregação {cpf.Congregacao.Nome} com o Código {cpf.Id}.");

            var dadosBatismo = _batismoAppService.GetById(membroVm.BatismoId, UserAppContext.Current.Usuario.Id);
            membroVm.DataPrevistaBatismo = dadosBatismo.DataBatismo.Date;

            if (!UserAppContext.Current.Usuario.Congregacao.Sede)
            {
                if (membroVm.Pessoa.DataNascimento != null && membroVm.Pessoa.DataNascimento != DateTime.MinValue)
                {

                    if (dadosBatismo.IdadeMinima > 0)
                    {
                        var dataref = dadosBatismo.DataBatismo.AddYears((dadosBatismo.IdadeMinima * -1));
                        if (membroVm.Pessoa.DataNascimento.Value.Date > dataref.Date)
                            throw new Erro($"A idade do membro é menor que a Idade mínima({dadosBatismo.IdadeMinima} anos) permitida");
                    }
                }
                else
                    throw new Erro($"Data de Nascimento é de preenchimento obrigatório.");
            }

            if (string.IsNullOrEmpty(membroVm.Endereco.Logradouro))
                throw new Erro($"Logradouro é de preenchimento obrigatório");

            if (string.IsNullOrEmpty(membroVm.Endereco.Pais))
                throw new Erro($"Pais é de preenchimento obrigatório");

            if (membroVm.Endereco.Pais != "Brasil")
            {
                if (string.IsNullOrEmpty(membroVm.Endereco.BairroEstrangeiro))
                    throw new Erro($"Bairro é de preenchimento obrigatório");

                if (string.IsNullOrEmpty(membroVm.Endereco.CidadeEstrangeiro))
                    throw new Erro($"Cidade é de preenchimento obrigatório");

                if (string.IsNullOrEmpty(membroVm.Endereco.Provincia))
                    throw new Erro($"Província/Estado é de preenchimento obrigatório");
            }
            else
            {
                if (string.IsNullOrEmpty(membroVm.Endereco.Bairro))
                    throw new Erro($"Bairro é de preenchimento obrigatório");

                if (string.IsNullOrEmpty(membroVm.Endereco.Cidade))
                    throw new Erro($"Cidade é de preenchimento obrigatório");

                if (string.IsNullOrEmpty(membroVm.Endereco.Estado.ToString()))
                    throw new Erro($"Estado é de preenchimento obrigatório");
            }
        }


        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Delete([FromServices] ILogger<BatismoController> logger, long id)
        {
            try
            {
                _membroAppService.DeleteAndDeleteFilesAsync(id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "Erro", mensagem = ex.Message, url = "" });
            }
            this.ShowMessage("Sucesso", "Candidato ao Bastismo excluído com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Candidato ao Bastismo excluído com sucesso!", url = Url.Action("Index", "Batismo") });
        }


        [Action(Menu.Batismo, Menu.Chamada)]
        public ActionResult ListaBatismo()
        {
            /*Dados Batismo*/
            var dadosBatismo = _batismoAppService.SelecionaUltimoDataBatismo();

            var listaVM = new ListaBatismoVM()
            {
                DataBatismo = dadosBatismo.DataBatismo.DateTime,
                Id = dadosBatismo.Id
            };

            _batismoAppService.GetAll(UserAppContext.Current.Usuario.Id)
                               .Where(b => b.Status == StatusBatismo.EmAberto)
                               .OrderBy(d => d.DataBatismo)
                               .ToList()
                               .ForEach(i => listaVM.ListaDatasBatismo.Add(
               new SelectListItem()
               {
                   Text = i.DataBatismo.Date.ToShortDateString(),
                   Value = i.Id.ToString()
               })
           );


            return View(listaVM);
        }

        [HttpPost]
        public JsonResult ListaBatismo([FromServices] ILogger<BatismoController> logger,
            int idBatismo, List<CandidatosBatismoVM> candidatos)
        {
            try
            {
                var membros = new List<BatismoMembro>();
                candidatos.ForEach(m =>
                    membros.Add(new BatismoMembro()
                    {
                        MembroId = m.MembroId,
                        SituacaoCandBatismo = m.Situacao ? SituacaoCandidatoBatismo.Presente : SituacaoCandidatoBatismo.Ausente
                    }));

                _batismoAppService.AtualizarMembroBatismo(membros, idBatismo);
                this.ShowMessage("Lista de Presença", "Lista de Presença salva com Sucesso.");
                var batismoAberto = _batismoAppService.GetAll(UserAppContext.Current.Usuario.Id)
                                                      .Where(b => b.Status == StatusBatismo.EmAberto).Count();
                var url = Url.Action("Index", "Batismo", new { Area = "Secretaria" });
                if (batismoAberto > 0)
                    url = Url.Action("ListaBatismo", "Batismo", new { Area = "Secretaria" });
                return Json(new { status = "OK", url });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "Erro", msg = $"Falha ao salvar a Lista de Presença. Erro: {ex.Message}", url = "" });
            }
        }

        #region ConfiguracaoBatismo
        [Action(Menu.Batismo, Menu.ConfuguracaoBatismo)]
        public ActionResult IndexConfiguracaoBatismo()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetListConfiguracao([FromServices] ILogger<BatismoController> logger,
            int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                var conf = _batismoAppService.ListarBatismoPaginada(jtPageSize, jtStartIndex, jtSorting, out int qtdRows).ToList();

                return Json(new { Result = "OK", Records = conf, TotalRecordCount = qtdRows });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { Result = "ERROR", ex.Message });
            }

        }

        // GET: Secretaria/Membro/Create
        public ActionResult CreateConfiguracaoBatismo()
        {
            // Inicializa o ViewModel para não dar erro na view por causa do titulo da VM
            var ConfBatVm = new ConfiguracaoBatismoViewModel
            {
                DataMaximaCadastro = DateTime.Now.Date,
                DataBatismo = DateTime.Now.Date,
                Status = StatusBatismo.EmAberto,
                Acao = Acao.Create,
                PastoresCelebrantes = new List<GridPastorCelebranteViewModel>()
            };
            return View(ConfBatVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult CreateConfiguracaoBatismo([FromServices] ILogger<BatismoController> logger, ConfiguracaoBatismoViewModel confBatVM)
        {
            return AddUpdateConfBatismo(logger, confBatVM);
        }
        public ActionResult DetailsConfiguracaoBatismo(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Batismo_DetailsConfiguracaoBatismo&valor=0");
            var batismo = _batismoAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (batismo == null || batismo.Id == 0)
                return Redirect("/Auth/NaoAutorizado");
            return View(DePara(batismo, Acao.Read, true));
        }

        public ActionResult EditConfiguracaoBatismo(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Batismo_EditConfiguracaoBatismo&valor=0");
            var batismo = _batismoAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (batismo == null || batismo.Id == 0)
                return Redirect("/Auth/NaoAutorizado");
            return View(DePara(batismo, Acao.Update, false));
        }

        private ConfiguracaoBatismoViewModel DePara(Batismo batismo, Acao acao, bool isReadOnly)
        {
            var pastorCeleb = new List<GridPastorCelebranteViewModel>();
            int i = 1;
            _batismoAppService.ListarPastorCelebrante(batismo.Id).ToList().ForEach(p => pastorCeleb.Add(
               new GridPastorCelebranteViewModel()
               {
                   Id = i++,
                   MembroId = p.Id,
                   Nome = p.Nome
               }));

            var batismoVm = new ConfiguracaoBatismoViewModel()
            {
                Acao = acao,
                Id = batismo.Id,
                DataBatismo = batismo.DataBatismo,
                DataMaximaCadastro = batismo.DataMaximaCadastro.Date,
                HoraBatismo = batismo.DataBatismo.TimeOfDay,
                IdadeMinima = batismo.IdadeMinima,
                Status = batismo.Status,
                DataAlteracao = batismo.DataAlteracao,
                DataCriacao = batismo.DataCriacao,
                IsReadOnly = isReadOnly
            };
            batismoVm.PastoresCelebrantes = pastorCeleb;
            batismoVm.PastoresCelebrantes.ForEach(a => a.IsReadOnly = isReadOnly);

            return batismoVm;
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult EditConfiguracaoBatismo([FromServices] ILogger<BatismoController> logger, ConfiguracaoBatismoViewModel confBatVM)
        {
            return AddUpdateConfBatismo(logger, confBatVM);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult DeleteConfiguracaoBatismo([FromServices] ILogger<BatismoController> logger,
            long id)
        {
            try
            {
                _batismoAppService.Delete(id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "Erro", mensagem = ex.Message, url = "" });
            }
            this.ShowMessage("Sucesso", "Configuração de Batismo excluída com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Configuração de Batismo excluída com sucesso!", url = Url.Action("IndexConfiguracaoBatismo", "Batismo", new { Area = "Secretaria" }) });
        }

        private JsonResult AddUpdateConfBatismo(ILogger<BatismoController> logger,
            ConfiguracaoBatismoViewModel confBatVM)
        {
            try
            {
                var batismo = ViewModelParaModel(confBatVM);
                _batismoAppService.Add(batismo, UserAppContext.Current.Usuario.Id);
                if (batismo.StatusRetorno == TipoStatusRetorno.OK)
                {
                    var msgAlert = $"Configuração de Batismo incluída com sucesso!";
                    if (confBatVM.Acao == Acao.Update)
                        msgAlert = $"Configuração de Batismo atualizada com sucesso!";
                    this.ShowMessage("Sucesso", msgAlert, AlertType.Success);

                    return Json(new { status = "OK", msg = msgAlert, url = Url.Action("IndexConfiguracaoBatismo", "Batismo", new { Area = "Secretaria" }) });
                }
                else
                    throw new Erro(string.Join(Environment.NewLine, batismo.ErrosRetorno.Select(e => e.Mensagem)));

            }
            catch (Erro ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                var msgAlert = $"Falha ao incluir a Configuração de Batismo - Erro: {ex.Message}";
                if (confBatVM.Acao == Acao.Update)
                    msgAlert = $"Falha ao atualizar a Configuração de Batismo - Erro: {ex.Message}";
                return Json(new { status = "Erro", membroid = confBatVM?.Id, msg = msgAlert, url = Url.Action("IndexConfiguracaoBatismo", "Batismo", new { Area = "Secretaria" }) });
            }
            catch (Exception ex)
            {
                var msgAlert = $"Falha ao incluir a Configuração de Batismo - Erro: {ex.Message}";
                if (confBatVM.Acao == Acao.Update)
                    msgAlert = $"Falha ao atualizar a Configuração de Batismo - Erro: {ex.Message}";
                return Json(new { status = "Erro", membroid = confBatVM?.Id, msg = msgAlert, url = Url.Action("IndexConfiguracaoBatismo", "Batismo", new { Area = "Secretaria" }) });
            }
        }

        private Batismo ViewModelParaModel(ConfiguracaoBatismoViewModel vmConfBatismo)
        {
            var batismo = new Batismo
            {
                Id = vmConfBatismo.Id,
                DataBatismo = vmConfBatismo.DataBatismo.Add(((TimeSpan)vmConfBatismo.HoraBatismo)),
                DataMaximaCadastro = vmConfBatismo.DataMaximaCadastro,
                Status = vmConfBatismo.Status,
                IdadeMinima = vmConfBatismo.IdadeMinima.GetValueOrDefault(0)
            };
            vmConfBatismo.PastoresCelebrantes.ForEach(p => batismo.Celebrantes.Add(new Membro { Id = p.MembroId }));
            return batismo;
        }

        public JsonResult BuscarMembro([FromServices] ILogger<BatismoController> logger, long membroId)
        {
            try
            {
                var _mem = _membroAppService.GetById(membroId, UserAppContext.Current.Usuario.Id);
                if (_mem != null && !string.IsNullOrEmpty(_mem.Nome))
                {
                    if (_mem.Status != Status.Ativo)
                        throw new Erro("Membro não Ativo!");
                    else if (_mem.Cargos.Count() == 0)
                        throw new Erro("Membro não é Pastor!");
                    return Json(new { _mem.Id, _mem.Nome, Status = "OK" });
                }

                throw new Erro("Membro não cadastrado.");
            }
            catch (Erro ex)
            {
                return Json(new { Status = "ERRO", Msg = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { Status = "ERRO", Msg = ex.Message });
            }
        }

        public JsonResult BuscarBatismo(long id)
        {
            try
            {
                var batismo = new Batismo();
                if (id == 0)
                {
                    batismo = _batismoAppService.GetAll(UserAppContext.Current.Usuario.Id)
                               .Where(b => b.Status == StatusBatismo.EmAberto)
                               .OrderBy(d => d.DataBatismo)
                               .First();
                    if (batismo != null)
                    {
                        return Json(new { batismo, Status = "OK" });
                    }
                }
                else
                {
                    batismo = _batismoAppService.GetById(id, UserAppContext.Current.Usuario.Id);
                    if (batismo != null)
                    {
                        return Json(new { batismo, Status = "OK" });
                    }
                }
                throw new Exception("Batismo não localizado.");
            }
            catch (Exception ex)
            {
                return Json(new { Status = "ERRO", Msg = ex.Message });
            }
        }

        public JsonResult BatismosEmAberto()
        {
            var listaBatismo = _batismoAppService.GetAll(UserAppContext.Current.Usuario.Id).ToList();
            var qtd = listaBatismo.Count(b => b.Status == StatusBatismo.EmAberto);
            if (qtd > 0)
            {
                if (qtd == 1)
                {
                    var data = listaBatismo.Where(b => b.Status == StatusBatismo.EmAberto).FirstOrDefault().DataBatismo;
                    return Json(new { Status = "ERRO", Msg = $"Já existe um batismo cadastrado para o data de {data.Date.ToShortDateString()} às {data.TimeOfDay.Hours}:{data.TimeOfDay.Minutes.ToString().PadLeft(2, '0')}.<br /> Deseja cadastrar uma nova data?" });
                }
                else
                    return Json(new { Status = "ERRO", Msg = $"Já existem {qtd} batismos cadastrados.<br />Deseja cadastrar uma nova data?" });
            }
            return Json(new { Status = "OK", Msg = $"" });
        }

        public ActionResult ListaPastorBatismo(int idBatismo)
        {
            /*Celebrantes*/
            var pastores = new List<GridPastorCelebranteViewModel>();
            _batismoAppService.ListarPastorCelebrante(idBatismo).ToList().
                ForEach(p => pastores.Add(new GridPastorCelebranteViewModel()
                {
                    Nome = p.Nome,
                    Id = p.Id,
                    MembroId = p.Id,
                    IsReadOnly = true
                }));
            return PartialView("_Pastores", pastores);
        }

        public ActionResult ListaCandidatosBatismo(int idBatismo)
        {
            /*Candidatos ao Batismo*/
            var membros = new List<CandidatosBatismoVM>();
            _batismoAppService.SelecionaMembrosParaBatismo(idBatismo, StatusBatismo.EmAberto, UserAppContext.Current.Usuario.Id).ToList().
                ForEach(p => membros.Add(new CandidatosBatismoVM()
                {
                    Id = p.Id,
                    MembroId = (int)p.Id,
                    Nome = p.Nome,
                    CongregacaoId = (int)p.Congregacao.Id,
                    CongregacaoNome = p.Congregacao.Nome,
                    DataNascimento = p.DataNascimento,
                    IsReadOnly = true
                }));

            return PartialView("_Candidatos", membros);
        }
        #endregion
    }
}