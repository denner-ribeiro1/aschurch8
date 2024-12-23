using System.ComponentModel.DataAnnotations;
using ASChurchManager.Web.ViewModels.Shared;
using ASChurchManager.Domain.Types;

namespace ASChurchManager.Web.Areas.Admin.ViewModels.Cargo
{
    public class CargoViewModel : EntityViewModelBase
    {
        public CargoViewModel()
        {
            ViewModelTitle = "Cargos";
        }
        [Key]
        public override long Id { get; set; }

        [Display(Name = "Descrição Cargo"),
        Required(ErrorMessage = "Cargo é obrigatório"),
        StringLength(100)]
        public string Descricao { get; set; }

        [Display(Name = "Obreiro")]
        public bool Obreiro { get; set; }

        [Display(Name = "Líder")]
        public bool Lider { get; set; }

        [Display(Name = "Tipo de Carteirinha")]
        public TipoCarteirinha TipoCarteirinha { get; set; }

        [Display(Name = "Habilitar Campo CONFRADESP Nº")]
        public bool Confradesp { get; set; }

        [Display(Name = "Habilitar Campo CGADB Nº")]
        public bool CGADB { get; set; }
    }
}