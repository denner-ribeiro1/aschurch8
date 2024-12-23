using ASChurchManager.Web.ViewModels.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.PastorCelebrante
{
    public class PastorCelebranteViewModel : EntityViewModelBase
    {
        public PastorCelebranteViewModel()
        {
            GrupoPastorCelebrante = new List<GridPastorCelebranteViewModel>();
        }
        [Key]
        public override long Id { get; set; }

        public long MembroId { get; set; }

        [Display(Name = "Nome do Pastor Celebrante")]
        public string Nome { get; set; }

        public List<GridPastorCelebranteViewModel> GrupoPastorCelebrante { get; set; }

    }
}