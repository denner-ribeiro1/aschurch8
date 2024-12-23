using ASChurchManager.Web.ViewModels.Shared;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.PastorCelebrante
{
    public class GridPastorCelebranteViewModel : EntityViewModelBase
    {
        public GridPastorCelebranteViewModel()
        {
            ViewModelTitle = "Pastor Celebrante";
        }
        [Key]
        public override long Id { get; set; }

        public long MembroId { get; set; }

        [Display(Name = "Nome do Pastor Celebrante")]
        public string Nome { get; set; }

        [Display(Name = "Pastor Presidente")]
        public string PastorPresidente { get; set; }
    }
}