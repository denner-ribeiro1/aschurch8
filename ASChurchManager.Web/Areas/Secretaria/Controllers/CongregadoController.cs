using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Batismo;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Congregado;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro;
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
using System.Linq;

namespace ASChurchManager.Web.Areas.Secretaria.Controllers
{
    [Rotina(Area.Secretaria), ControllerAuthorize("Congregado")]
    public class CongregadoController : BaseController
    {
        private IMembroAppService _membroAppService;
        private readonly IBatismoAppService _batismoAppService;
        private readonly ICongregacaoAppService _congrAppService;
        private readonly IMapper _mapper;

        public CongregadoController(IMembroAppService appService
                                    , IBatismoAppService batismoAppService
                                    , ICongregacaoAppService congrAppService
                                    , IMapper mapper
                                    , IMemoryCache cache
                                    , IUsuarioLogado usuLog
                                    , IConfiguration _configuration
                                    , IRotinaAppService _rotinaAppService)
                    : base(cache, usuLog, _configuration, _rotinaAppService)
        {
            _membroAppService = appService;
            _batismoAppService = batismoAppService;
            _congrAppService = congrAppService;
            _mapper = mapper;
        }

        [HttpPost]
        public JsonResult GetList([FromServices] ILogger<CongregadoController> logger,
            string filtro = "", string conteudo = "", string status = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                jtSorting = jtSorting == "Congregacao" ? jtSorting : jtSorting.Replace("Congregacao", "CongregacaoId");
                if (!Enum.TryParse(status, out Status statusSel))
                    statusSel = Status.NaoDefinido;

                var membros = _membroAppService.ListarMembroPaginado(jtPageSize, jtStartIndex, out int qtdRows, jtSorting, filtro, conteudo, UserAppContext.Current.Usuario.Id, TipoMembro.Congregado, statusSel).ToList();
                var membrosVM = new List<GridMembroItem>();
                membros.ForEach(p => membrosVM.Add(new GridMembroItem()
                {
                    Id = p.Id,
                    Congregacao = p.Congregacao.Nome,
                    Nome = p.Nome,
                    NomeMae = p.NomeMae,
                    Status = p.Status == Status.Inativo ? "Inativo" : "Ativo"
                }));

                return Json(new { Result = "OK", Records = membrosVM, TotalRecordCount = qtdRows });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { Result = "ERROR", ex.Message });
            }
        }

        // GET: Secretaria/Membro
        [Action(Menu.Congregado)]
        public ActionResult Index()
        {
            var membroVM = new IndexCongregadoVM();
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

            var batismos = _batismoAppService.GetAll(UserAppContext.Current.Usuario.Id)
               .Where(b => b.Status == StatusBatismo.EmAberto &&
                           (UserAppContext.Current.Usuario.PermiteCadBatismoAposDataMaxima || b.DataMaximaCadastro.Date >= DateTimeOffset.Now.Date));

            membroVM.EnviarBatismo = batismos.Count() > 0;

            return View(membroVM);
        }

        public ActionResult Details(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Congregado_Details&valor=0");

            var congregado = _membroAppService.GetById(id, UserAppContext.Current.Usuario.Id);

            if (congregado == null || congregado.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var congregadoVm = _mapper.Map<CongregadoViewModel>(congregado);
            congregadoVm.IsReadOnly = true;
            congregadoVm.IsReadOnly = true;
            congregadoVm.Endereco.IsReadOnly = true;
            congregadoVm.Acao = Acao.Read;
            congregadoVm.Ativo = !(congregado.Status == Status.Inativo);

            if (congregado.Endereco.Pais != "Brasil")
            {
                congregadoVm.Endereco.BairroEstrangeiro = congregado.Endereco.Bairro;
                congregadoVm.Endereco.CidadeEstrangeiro = congregado.Endereco.Cidade;
                congregadoVm.Endereco.Provincia = congregado.Endereco.Estado;
                congregadoVm.Endereco.CodigoPostal = congregado.Endereco.Cep;
                congregadoVm.TelefoneCelularEstrangeiro = congregado.TelefoneCelular;
                congregadoVm.TelefoneComercialEstrangeiro = congregado.TelefoneComercial;
                congregadoVm.TelefoneResidencialEstrangeiro = congregado.TelefoneResidencial;
                congregadoVm.TelefoneCelular = string.Empty;
                congregadoVm.TelefoneComercial = string.Empty;
                congregadoVm.TelefoneResidencial = string.Empty;
            }
            else
            {
                congregadoVm.TelefoneCelular = congregado.TelefoneCelular;
                congregadoVm.TelefoneComercial = congregado.TelefoneComercial;
                congregadoVm.TelefoneResidencial = congregado.TelefoneResidencial;
            }
            return View(congregadoVm);
        }

        public ActionResult Create()
        {
            // Inicializa o ViewModel para não dar erro na view por causa do titulo da VM
            var congregadoVm = new CongregadoViewModel();
            if (UserAppContext.Current.Usuario.Congregacao.Id > 0)
            {
                congregadoVm.CongregacaoId = UserAppContext.Current.Usuario.Congregacao.Id;
                congregadoVm.CongregacaoNome = UserAppContext.Current.Usuario.Congregacao.Nome;
            }

            congregadoVm.TipoMembro = TipoMembro.Congregado;
            congregadoVm.Acao = Acao.Create;


            var paises = _membroAppService.ConsultarPaises();
            congregadoVm.Endereco.SelectPaises = paises.Select(e => new SelectListItem
            {
                Value = e.Nome,
                Text = e.Nome
            });

            return View(congregadoVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult Create([FromServices] ILogger<CongregadoController> logger,
            CongregadoViewModel membroVm)
        {
            try
            {
                if (membroVm.Endereco.Pais == "Brasil")
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
                    return AddUpdateCongregado(membroVm);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                this.ShowMessage("Congregados", $"Falha ao salvar o Congregado. Erro: {ex.Message}", AlertType.Error);
            }

            if (UserAppContext.Current.Usuario.Congregacao.Id > 0)
            {
                membroVm.CongregacaoId = UserAppContext.Current.Usuario.Congregacao.Id;
                membroVm.CongregacaoNome = UserAppContext.Current.Usuario.Congregacao.Nome;
            }
            membroVm.TipoMembro = TipoMembro.Congregado;
            membroVm.Acao = Acao.Create;
            var paises = _membroAppService.ConsultarPaises();
            membroVm.Endereco.SelectPaises = paises.Select(e => new SelectListItem
            {
                Value = e.Nome,
                Text = e.Nome
            });
            return View(membroVm);
        }

        public ActionResult Edit(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Congregado_Edit&valor=0");

            var membro = _membroAppService.GetById(id, UserAppContext.Current.Usuario.Id);

            if (membro == null || membro.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var congregadoVm = _mapper.Map<CongregadoViewModel>(membro);
            congregadoVm.Acao = Acao.Update;
            congregadoVm.Ativo = !(membro.Status == Status.Inativo);

            if (membro.Endereco.Pais != "Brasil")
            {
                congregadoVm.Endereco.BairroEstrangeiro = membro.Endereco.Bairro;
                congregadoVm.Endereco.CidadeEstrangeiro = membro.Endereco.Cidade;
                congregadoVm.Endereco.Provincia = membro.Endereco.Estado;
                congregadoVm.Endereco.CodigoPostal = membro.Endereco.Cep;
                congregadoVm.TelefoneCelularEstrangeiro = membro.TelefoneCelular;
                congregadoVm.TelefoneComercialEstrangeiro = membro.TelefoneComercial;
                congregadoVm.TelefoneResidencialEstrangeiro = membro.TelefoneResidencial;
                congregadoVm.TelefoneCelular = string.Empty;
                congregadoVm.TelefoneComercial = string.Empty;
                congregadoVm.TelefoneResidencial = string.Empty;
            }
            else
            {
                congregadoVm.TelefoneCelular = membro.TelefoneCelular;
                congregadoVm.TelefoneComercial = membro.TelefoneComercial;
                congregadoVm.TelefoneResidencial = membro.TelefoneResidencial;
            }

            var paises = _membroAppService.ConsultarPaises();
            congregadoVm.Endereco.SelectPaises = paises.Select(e => new SelectListItem
            {
                Value = e.Nome,
                Text = e.Nome
            });
            return View(congregadoVm);
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public ActionResult Edit([FromServices] ILogger<CongregadoController> logger,
            CongregadoViewModel membroVm)
        {
            try
            {
                if (membroVm.Endereco.Pais == "Brasil")
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
                    return AddUpdateCongregado(membroVm, true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                this.ShowMessage("Congregados", $"Falha ao salvar o Congregado. Erro: {ex.Message}", AlertType.Error);
            }

            membroVm.Acao = Acao.Update;
            var paises = _membroAppService.ConsultarPaises();
            membroVm.Endereco.SelectPaises = paises.Select(e => new SelectListItem
            {
                Value = e.Nome,
                Text = e.Nome
            });

            return View(membroVm);
        }

        public ActionResult AddUpdateCongregado(CongregadoViewModel membroVm, bool update = false)
        {
            var membro = _mapper.Map<Membro>(membroVm);
            membro.Status = Status.Ativo;
            if (!membroVm.Ativo && update)
                membro.Status = Status.Inativo;
            membro.TipoMembro = TipoMembro.Congregado;
            membro.Congregacao.Id = membroVm.CongregacaoId;
            membro.CriadoPorId = UserAppContext.Current.Usuario.Id;
            membro.Conjuge = new Membro();
            membro.Mae = new Membro();
            membro.Pai = new Membro();

            membro.Endereco.Pais = membroVm.Endereco.Pais;
            if (!string.IsNullOrWhiteSpace(membroVm.Endereco.Pais) && membroVm.Endereco.Pais != "Brasil")
            {
                membro.Endereco.Cep = membroVm.Endereco.CodigoPostal;
                membro.Endereco.Bairro = membroVm.Endereco.BairroEstrangeiro;
                membro.Endereco.Cidade = membroVm.Endereco.CidadeEstrangeiro;
                membro.Endereco.Estado = membroVm.Endereco.Provincia;
                membro.TelefoneCelular = membroVm.TelefoneCelularEstrangeiro;
                membro.TelefoneComercial = membroVm.TelefoneComercialEstrangeiro;
                membro.TelefoneResidencial = membroVm.TelefoneResidencialEstrangeiro;
            }
            else
            {
                membro.Endereco.Cep = membroVm.Endereco.Cep;
                membro.Endereco.Bairro = membroVm.Endereco.Bairro;
                membro.Endereco.Cidade = membroVm.Endereco.Cidade;
                membro.Endereco.Estado = membroVm.Endereco.Estado.ToString();
            }

            _membroAppService.Add(membro);

            var msgAlert = $"Congregado(a) {membroVm.Nome} incluído com sucesso! Código: {membro.Id}";
            if (update)
                msgAlert = $"Congregado(a) {membroVm.Nome} atualizado com sucesso! Código: {membro.Id}";

            this.ShowMessage("Sucesso", msgAlert, AlertType.Success);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ServiceFilter(typeof(LogAuditoriaAttribute))]
        public JsonResult Delete([FromServices] ILogger<CongregadoController> logger, long id)
        {
            try
            {
                _membroAppService.Delete(id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new { status = "Erro", mensagem = ex.Message, url = "" });
            }
            this.ShowMessage("Sucesso", "Congregado excluído com sucesso!", AlertType.Success);
            return Json(new { status = "OK", mensagem = "Congregado  excluído com sucesso!", url = Url.Action("Index", "Congregado") });
        }

        public ActionResult EnviarBatismo(long id)
        {
            if (id == 0)
                return Redirect($"/Auth/BadRequest?url=Congregado_EnviarBatismo&valor=0");

            var membro = _membroAppService.GetById(id, UserAppContext.Current.Usuario.Id);
            if (membro == null || membro.Id == 0)
                return Redirect("/Auth/NaoAutorizado");

            var batismoVM = _mapper.Map<BatismoViewModel>(membro);
            batismoVM.Acao = Acao.Create;
            batismoVM.TipoMembro = TipoMembro.Batismo;

            batismoVM.Pessoa.SelectEstadoCivil = Enum.GetValues(typeof(EstadoCivil))
                 .Cast<EstadoCivil>()
                 .Select(e => new SelectListItem
                 {
                     Value = ((int)e).ToString(),
                     Text = e.GetDisplayAttributeValue()
                 });

            if (!string.IsNullOrWhiteSpace(membro.NaturalidadeEstado) && Enum.IsDefined(typeof(Estado), membro.NaturalidadeEstado))
                batismoVM.Pessoa.NaturalidadeEstado = (Estado)Enum.Parse(typeof(Estado), membro.NaturalidadeEstado);

            var batismos = _batismoAppService.GetAll(UserAppContext.Current.Usuario.Id)
                   .Where(b => b.Status == StatusBatismo.EmAberto &&
                       (UserAppContext.Current.Usuario.PermiteCadBatismoAposDataMaxima || b.DataMaximaCadastro.Date >= DateTimeOffset.Now.Date));

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
        public JsonResult EnviarParaBatismo([FromServices] ILogger<CongregadoController> logger, BatismoViewModel membroVm)
        {
            try
            {
                ValidarMembro(membroVm);
                var membro = CriarMembroFromViewModel(membroVm);
                _membroAppService.Update(membro);

                membroVm.Id = membro.Id;
                this.ShowMessage("Envio para o Batismo", $"Membro ({membroVm.Pessoa.Nome}) enviado para o Batismo com sucesso!");
                return Json(new
                {
                    status = "OK",
                    membroid = membroVm.Id,
                    msg = $"Membro {membroVm.Pessoa.Nome} enviado para o Batismo com sucesso! Código: {membroVm.Id}",
                    url = Url.Action("Index", "Congregado", new { Area = "Secretaria" })
                });

            }
            catch (Erro ex)
            {
                return Json(new
                {
                    status = "Erro",
                    membroid = membroVm?.Id,
                    msg = $"Falha ao enviar o Membro para o Batismo. Erro: {ex.Message}",
                    url = Url.Action("Index", "Congregado", new { Area = "Secretaria" })
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Usuário - {HttpContext.User.Identity.Name}");
                return Json(new
                {
                    status = "Erro",
                    membroid = membroVm?.Id,
                    msg = $"Falha ao enviar o Membro para o Batismo. Erro: {ex.Message}",
                    url = Url.Action("Index", "Congregado", new { Area = "Secretaria" })
                });
            }
        }

        private void ValidarMembro(BatismoViewModel membroVm)
        {
            if (membroVm == null || membroVm.Endereco == null)
                throw new Erro("Membro deve ser preenchido.");

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
                throw new InvalidOperationException("Data de Nascimento é de preenchimento obrigatório.");

            if (membroVm.Endereco.Pais != "Brasil")
            {
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

        private Membro CriarMembroFromViewModel(BatismoViewModel membroVM)
        {
            var membro = _mapper.Map<Membro>(membroVM);

            membro.Status = Status.Ativo;
            membro.TipoMembro = TipoMembro.Batismo;
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
            membro.DataPrevistaBatismo = membroVM.DataPrevistaBatismo;
            membro.BatismoId = membroVM.BatismoId;

            membro.Endereco.Pais = membroVM.Endereco.Pais;
            if (!string.IsNullOrWhiteSpace(membroVM.Endereco.Pais) && membroVM.Endereco.Pais != "Brasil")
            {
                membro.Endereco.Cep = membroVM.Endereco.CodigoPostal;
                membro.Endereco.Cidade = membroVM.Endereco.CidadeEstrangeiro;
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _membroAppService = null;
            }
            base.Dispose(disposing);
        }
    }
}