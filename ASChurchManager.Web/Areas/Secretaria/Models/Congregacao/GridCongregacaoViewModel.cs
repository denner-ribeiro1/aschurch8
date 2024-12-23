using ASChurchManager.Web.ViewModels.Shared;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Congregacao
{
    public class GridCongregacaoViewModel : EntityViewModelBase
    {
        /// <summary>
        /// Id da Congregação
        /// Obs: Sobrescrito para que seja a primeira coluna no Grid.Mvc
        /// </summary>
        [Key]
        public override long Id { get; set; }

        /// <summary>
        /// Nome da Congregação
        /// </summary>
        [Display(Name = "Nome Congregação")]
        public string Nome { get; set; }

        //[UIHint("SimNao")]
        //public bool Sede { get; set; }

        [Display(Name = "Congregação Responsável")]
        public string CongregacaoResponsavelNome { get; set; }
    }
}