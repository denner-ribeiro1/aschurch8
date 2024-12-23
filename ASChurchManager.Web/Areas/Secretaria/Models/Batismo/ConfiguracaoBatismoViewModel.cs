using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.PastorCelebrante;
using ASChurchManager.Web.ViewModels.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Batismo
{

    public class ConfiguracaoBatismoViewModel : EntityViewModelBase
    {

        public ConfiguracaoBatismoViewModel()
        {
            PastoresCelebrantes = new List<GridPastorCelebranteViewModel>();
        }
        [Display(Name = "Data do Batismo"),
        Required(ErrorMessage = "O campo Data do Batismo é obrigatório"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset DataBatismo { get; set; }

        [Display(Name = "Horário Previsto"),
        DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true),
        Required(ErrorMessage = "Horário Previsto do Batismo é de preenchimento obrigatório")]
        public TimeSpan? HoraBatismo { get; set; }

        [Display(Name = "Data máxima para o cadastro"),
        Required(ErrorMessage = "O campo Data Máxima para o cadastro do Batismo é obrigatório"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset DataMaximaCadastro { get; set; }

        [Display(Name = "Cód. Pastor Celebrante.")]
        public long? MembroId { get; set; }

        [Display(Name = "Idade Mínima"),
         Required(ErrorMessage = "O campo Idade Mínima é de preenchimento obrigatório"),
         Range(typeof(int), "1", "100", ErrorMessage = "Idade mínima deve ser maior que Zero")]
        public int? IdadeMinima { get; set; }

        [Display(Name = "Pastor Celebrante")]
        public string Nome { get; set; }
        public List<GridPastorCelebranteViewModel> PastoresCelebrantes { get; set; }

        public string PastoresCelebrantesJSON
        {
            get
            {
                return JsonConvert.SerializeObject(PastoresCelebrantes);
            }
            set
            {
                PastoresCelebrantes = JsonConvert.DeserializeObject<List<GridPastorCelebranteViewModel>>(value);
            }
        }
        public Domain.Types.StatusBatismo Status { get; set; }

        public Acao Acao { get; set; }
    }
}