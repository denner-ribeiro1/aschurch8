using System.ComponentModel.DataAnnotations;
using ASChurchManager.Web.ViewModels.Shared;
using System;
using ASChurchManager.Domain.Types;

namespace ASChurchManager.Web.Areas.Admin.ViewModels.Cargo
{
    public class GridCargosViewModel : EntityViewModelBase
    {
        public GridCargosViewModel()
        {
            ViewModelTitle = "Cargos";
        }

        [Key]
        public override long Id { get; set; }

        [Display(Name = "Descrição Cargo"),
        StringLength(100, ErrorMessage = "Cargo deve ter até 100 carácteres"),
        Required(ErrorMessage = "Descrição do Cargo é obrigatório")]
        public string Descricao { get; set; }

        [Display(Name = "Obreiro")]
        public bool Obreiro { get; set; }

        [Display(Name = "Líder")]
        public bool Lider { get; set; }

        [Display(Name = "Data do Cargo"),
         DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataCargo;

        public TipoCarteirinha TipoCarteirinha { get; set; }

        public string LocalConsagracao { get; set; }

    }
}