using ASChurchManager.Domain.Types;
using ASChurchManager.Web.ViewModels.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro
{
    public class MembroViewModel : EntityViewModelBase
    {
        public MembroViewModel()
        {
            ViewModelTitle = "Dados Pessoais";
            Pessoa = new PessoaViewModel();
            Endereco = new EnderecoViewModel();
            DataRecepcao = null;
            DataBatismoAguas = null;
            Cargo = new CargoVM();
            Cargos = new List<CargoVM>();
            Observacao = new ObservacaoVM();
            Observacoes = new List<ObservacaoVM>();
            Situacao = new SituacaoVM();
            Situacoes = new List<SituacaoVM>();
            HistCartasViewModel = new HistoricoCartasViewModel();
            Historico = new List<GridHistoricoCartasViewModel>();
            ArquivosMembro = new List<ArquivoMembroVM>();
            CursoMembro = new ArquivoMembroVM();
        }

        [Required(ErrorMessage = "Código da Congregação é de preenchimento obrigatório"),
        Display(Name = "Cód. Congregação")]
        public long CongregacaoId { get; set; }

        [Required(ErrorMessage = "Nome da Congregação é de preenchimento obrigatório"),
        Display(Name = "Nome Congregação")]
        public string CongregacaoNome { get; set; }

        public PessoaViewModel Pessoa { get; set; }

        [Display(Name = "Status")]
        public Status Status { get; set; }

        [Display(Name = "Status")]
        public string StatusDescr
        {
            get
            {
                return Status.GetDisplayAttributeValue();
            }
        }

        [Display(Name = "Tipo")]
        public TipoMembro TipoMembro { get; set; }

        [Display(Name = "Tipo")]
        public string TipoMembroDescr
        {
            get
            {
                return TipoMembro.GetDisplayAttributeValue();
            }
        }

        [Required(ErrorMessage = "Recebido Por é de preenchimento obrigatório"), Display(Name = "Recebido Por")]
        public MembroRecebidoPor RecebidoPor { get; set; }

        [Display(Name = "Data Recepção"),
        Required(ErrorMessage = "Data Recepção é de preenchimento obrigatório"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataRecepcao { get; set; }

        [Display(Name = "Data Batismo Águas"),
        Required(ErrorMessage = "Data Batismo Águas é de preenchimento obrigatório"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataBatismoAguas { get; set; }

        [Display(Name = "Batismo Espírito Santo?")]
        public bool BatimoEspiritoSanto { get; set; }

        public long BatismoId { get; set; }

        public DateTimeOffset? DataPrevistaBatismo { get; set; }

        [Display(Name = "Membro Associado?")]
        public bool ABEDABE { get; set; }

        [Display(Name = "Usuário Criação")]
        public long CriadoPorId { get; set; }

        [Display(Name = "Usuário Aprovação")]
        public long AprovadoPorId { get; set; }

        [Display(Name = "Usuário Criação")]
        public string CriadoPorNome { get; set; }

        [Display(Name = "Usuário Aprovação")]
        public string AprovadoPorNome { get; set; }

        public EnderecoViewModel Endereco { get; set; }

        /*-----------------SITUACAO---------------*/
        public SituacaoVM Situacao { get; set; }
        public List<SituacaoVM> Situacoes { get; set; }
        public string Sit
        {
            get
            {
                return JsonConvert.SerializeObject(Situacoes);
            }
            set
            {
                Situacoes = JsonConvert.DeserializeObject<List<SituacaoVM>>(value);
            }
        }
        /*-----------------SITUACAO---------------*/

        /*-----------------CARGO---------------*/
        public CargoVM Cargo { get; set; }
        public List<CargoVM> Cargos { get; set; }
        public string Carg
        {
            get
            {
                return JsonConvert.SerializeObject(Cargos);
            }
            set
            {
                Cargos = JsonConvert.DeserializeObject<List<CargoVM>>(value);
            }
        }
        /*-----------------CARGO---------------*/

        /*-----------------OBSERVACAO---------------*/
        public ObservacaoVM Observacao { get; set; }
        public List<ObservacaoVM> Observacoes { get; set; }
        public string Obs
        {
            get
            {
                return JsonConvert.SerializeObject(Observacoes);
            }
            set
            {
                Observacoes = JsonConvert.DeserializeObject<List<ObservacaoVM>>(value);
            }
        }
        /*-----------------OBSERVACAO---------------*/


        /*-----------------CURSOS---------------*/
        public ArquivoMembroVM CursoMembro { get; set; }
        public List<ArquivoMembroVM> Cursos { get; set; }
        public string Curso
        {
            get
            {
                return JsonConvert.SerializeObject(Cursos);
            }
            set
            {
                Cursos = JsonConvert.DeserializeObject<List<ArquivoMembroVM>>(value);
            }
        }
        /*-----------------OBSERVACAO---------------*/
        [Display(Name = "Responsável")]
        public string UsuarioReprovacao { get; set; }

        [Display(Name = "Motivo")]
        public string MotivoReprovacao { get; set; }

        public HistoricoCartasViewModel HistCartasViewModel { get; set; }

        public List<GridHistoricoCartasViewModel> Historico { get; set; }

        public Acao? Acao { get; set; }

        [Display(Name = "Descrição do Arquivo")]
        public string DescricaoArquivo { get; set; }

        [DataType(DataType.Upload)]
        public string Arquivo { get; set; }

        public List<ArquivoMembroVM> ArquivosMembro { get; set; }

        public bool PermiteExcluirCargoMembro { get; set; }
        public bool PermiteExcluirObservacaoMembro { get; set; }
        public bool PermiteExcluirSituacaoMembro { get; set; }
        public bool MembroConfirmado { get; set; }

    }

    public class ReprovarViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Motivo de Reprovação é de preenchimento obrigatório"),
        Display(Name = "Motivo de Reprovação")]
        public string MotivoReprovacao { get; set; }
    }
}