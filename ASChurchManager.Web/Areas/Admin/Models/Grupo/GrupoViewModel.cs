
using ASChurchManager.Web.ViewModels.Shared;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Admin.ViewModels.Grupo
{
    public class GrupoViewModel : EntityViewModelBase
    {
        public GrupoViewModel()
        {
            ViewModelTitle = "Grupos";
        }

        [Key]
        public override long Id { get; set; }

        [Display(Name = "Descrição Grupo"),
        Required(ErrorMessage = "Grupo é obrigatório"),
        StringLength(100)]
        public string Descricao { get; set; }
    }
}