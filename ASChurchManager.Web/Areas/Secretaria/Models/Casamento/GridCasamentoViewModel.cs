using System;
using System.ComponentModel.DataAnnotations;
using ASChurchManager.Web.ViewModels.Shared;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Casamento
{
    public class GridCasamentoViewModel : EntityViewModelBase
    {
        public GridCasamentoViewModel()
        {
            ViewModelTitle = "Casamentos";
        }

        [Key]
        public override long Id { get; set; }

        [Display(Name = "Congregação")]
        public string CongregacaoNome{ get; set; }

        [Display(Name = "Data do Casamento"),
         DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset DataCasamento { get; set; }

        [Display(Name = "Hora Inicio")]
        public string HoraInicio { get; set; }

        [Display(Name = "Nome do Noivo")]
        public string NoivoNome { get; set; }

        [Display(Name = "Nome do Noiva")]
        public string NoivaNome { get; set; }

    }
}