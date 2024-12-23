using System.ComponentModel.DataAnnotations;
using ASChurchManager.Web.ViewModels.Shared;

namespace ASChurchManager.Web.Areas.Admin.ViewModels.TipoDeEvento
{
    public class TipoEventoViewModel : EntityViewModelBase
    {
        public TipoEventoViewModel()
        {
            ViewModelTitle = "Tipos de Evento";
        }


        [Display(Name = "Descrição do Tipo de Evento"),
        Required(ErrorMessage = "Tipo de Evento é obrigatório"),
        StringLength(100)]
        public string Descricao { get; set; }

    }
}