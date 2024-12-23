using System;
using System.ComponentModel.DataAnnotations;
using ASChurchManager.Web.ViewModels.Shared;

namespace ASChurchManager.Web.Areas.Admin.ViewModels.TipoDeEvento
{
    public class GridTipoEventoViewModel : ViewModelBase
    {
        public GridTipoEventoViewModel()
        {
            ViewModelTitle = "Tipo de Eventos";
        }
        [Key]
        public long Id { get; set; }

        [Display(Name = "Descrição"),
        StringLength(100),
        Required(ErrorMessage = "Tipo de Eventos é obrigatório")]
        public string Descricao { get; set; }

        [Display(Name = "Data Criação"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTimeOffset DataCriacao { get; set; }

        [Display(Name = "Data Alteração"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTimeOffset DataAlteracao { get; set; }
    }
}