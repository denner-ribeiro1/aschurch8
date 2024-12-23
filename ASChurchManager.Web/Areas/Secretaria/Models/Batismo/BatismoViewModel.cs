using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro;
using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Batismo
{
    public class BatismoViewModel : EntityViewModelBase
    {
        public BatismoViewModel()
        {
            ViewModelTitle = "Dados Pessoais";
            Pessoa = new PessoaViewModel();
            Endereco = new EnderecoViewModel();
            DataPrevistaBatismo = null;
            ListaDatasBatismo = new List<SelectListItem>();
            Observacao = new ObservacaoVM();
            Observacoes = new List<ObservacaoVM>();
            SelectTamanhoCapa = Enum.GetValues(typeof(Tamanho))
                    .Cast<Tamanho>()
                    .Select(e => new SelectListItem
                    {
                        Value = ((int)e).ToString(),
                        Text = e.GetDisplayAttributeValue()
                    });
            CursoMembro = new ArquivoMembroVM();
            ArquivosMembro = new List<ArquivoMembroVM>();
        }

        [Required(ErrorMessage = "O campo Código da Congregação é obrigatório"),
        Display(Name = "Cód. Congregação")]
        public long CongregacaoId { get; set; }

        [Display(Name = "Data Prevista p/Batismo"),
         Required(ErrorMessage = "O campo Data Prevista p/Batismo é obrigatório"),
         DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataPrevistaBatismo { get; set; }

        public List<SelectListItem> ListaDatasBatismo { get; set; }

        [Display(Name = "Data Prevista p/Batismo"),
            Required(ErrorMessage = "O campo Data Prevista p/Batismo é obrigatório")]
        public int BatismoId { get; set; }

        [Required(ErrorMessage = "O campo Nome da Congregação é obrigatório"),
        Display(Name = "Nome Congregação")]
        public string CongregacaoNome { get; set; }

        public PessoaViewModel Pessoa { get; set; }

        public EnderecoViewModel Endereco { get; set; }

        [Display(Name = "Batismo Espírito Santo")]
        public bool BatimoEspiritoSanto { get; set; }

        [Display(Name = "Tipo")]
        public TipoMembro TipoMembro { get; set; }

        public Acao Acao { get; set; }

        public bool EnviarBatismo { get; set; }

        [Display(Name = "Capa Tamanho")]
        public Tamanho TamanhoCapa { get; set; }

        public IEnumerable<SelectListItem> SelectTamanhoCapa { get; set; }

        /*-----------------OBSERVACAO---------------*/
        public bool PermiteExcluirObservacaoMembro { get; set; }

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

        /*-----------------ARQUIVOS---------------*/
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

        public List<ArquivoMembroVM> ArquivosMembro { get; set; }
        /*-----------------ARQUIVOS---------------*/
    }
}