using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Secretaria.Models.Congregacao;
using ASChurchManager.Web.Lib;
using ASChurchManager.Web.ViewModels.Shared;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Congregacao
{
    public class CongregacaoVM : EntityViewModelBase
    {
        public CongregacaoVM()
        {
            ViewModelTitle = "Congregação";
            Endereco = new EnderecoViewModel();
            Grupo = new GrupoVM();
            Grupos = new List<GrupoVM>();
            Obreiro = new ObreiroVM();
            Obreiros = new List<ObreiroVM>();
            Observacao = new ObservacaoVM();
            Observacoes = new List<ObservacaoVM>();
            Arquivos = new List<ArquivoVM>();
        }

        public Acao Acao { get; set; }

        [Required(ErrorMessage = "O campo Código da Congregação Responsável é obrigatório"),
        Display(Name = "Cód. Congregação Resp.")]
        public long? CongregacaoResponsavelId { get; set; }

        [Required(ErrorMessage = "O campo Nome da Congregação Responsável é obrigatório"),
        Display(Name = "Congregação Responsável")]
        public string CongregacaoResponsavelNome { get; set; }

        /// <summary>
        /// Nome da Congregação
        /// </summary>
        [Display(Name = "Nome Congregação"),
        Required(ErrorMessage = "Nome da Congregação é obrigatório"),
        StringLength(50)]
        public string Nome { get; set; }

        /// <summary>
        /// Define se é Congregação Sede
        /// Não deve ser setada diretamente
        /// </summary>
        [ReadOnly(true)]
        public bool Sede { get; set; }

        [Display(Name = "CNPJ"),
        Cnpj(ErrorMessage = "CNPJ Inválido")]
        public string CNPJ { get; set; }
        /// <summary>
        /// ViewModel de Endereço
        /// </summary>
        [UIHint("Endereco")]
        public EnderecoViewModel Endereco { get; set; }

        /*----------------- GRUPO ---------------*/
        public GrupoVM Grupo { get; set; }
        public List<GrupoVM> Grupos { get; set; }
        public string Grp
        {
            get
            {
                return JsonConvert.SerializeObject(Grupos);
            }
            set
            {
                Grupos = JsonConvert.DeserializeObject<List<GrupoVM>>(value);
            }
        }
        /*----------------- GRUPO ---------------*/

        [Required(ErrorMessage = "O campo Código do Pastor Responsável é obrigatório"),
         Display(Name = "Cód. Pastor Resp.")]
        public long? PastorResponsavelId { get; set; }

        [Required(ErrorMessage = "O campo Nome do Pastor Responsável é obrigatório"),
         Display(Name = "Pastor")]
        public string PastorResponsavelNome { get; set; }

        /*----------------- OBREIRO ---------------*/
        public ObreiroVM Obreiro { get; set; }
        public List<ObreiroVM> Obreiros { get; set; }
        public string Obr
        {
            get
            {
                return JsonConvert.SerializeObject(Obreiros);
            }
            set
            {
                Obreiros = JsonConvert.DeserializeObject<List<ObreiroVM>>(value);
            }
        }
        /*----------------- OBREIRO ---------------*/

        /*----------------- OBSERVACAO ---------------*/
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
        /*----------------- OBSERVACAO ---------------*/

        /*********************ARQUIVO ***************/
        [DataType(DataType.Upload)]
        [Display(Name = "Arquivos permitidos: Word (*.doc e *.docx), Excel (*.xls e *.xlsx), Compactados - (*.zip) e PDF")]
        public string Arquivo { get; set; }

        public List<ArquivoVM> Arquivos { get; set; }
        /*********************ARQUIVO ***************/
    }
}