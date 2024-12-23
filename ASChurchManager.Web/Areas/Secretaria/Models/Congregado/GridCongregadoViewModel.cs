using System.ComponentModel.DataAnnotations;
using ASChurchManager.Web.ViewModels.Shared;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Congregado
{
    public class GridCongregadoViewModel : EntityViewModelBase
    {
        public GridCongregadoViewModel()
        {
            this.ViewModelTitle = "Congregados";
        }

        /// <summary>
        /// Id da Congregação
        /// Obs: Sobrescrito para que seja a primeira coluna no Grid.Mvc
        /// </summary>
        [Key]
        public override long Id { get; set; }


        /// <summary>
        /// Nome do Membro
        /// </summary>
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Congregação
        /// </summary>
        [Display(Name = "Congregacao")]
        public string Congregacao { get; set; }

        public bool EnviarBatismo { get; set; }
    }
}