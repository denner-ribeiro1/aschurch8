using ASChurchManager.Web.ViewModels.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Admin.ViewModels.Template
{
    public class GridTemplateViewModel : ViewModelBase
    {
        public GridTemplateViewModel()
        {
            ViewModelTitle = "Templates";
        }

        [Key]
        public long Id { get; set; }

        [Display(Name = "Descrição")]
        public string Nome { get; set; }

        [Display(Name = "Data Criação"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTimeOffset DataCriacao { get; set; }

        [Display(Name = "Data Alteração"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTimeOffset DataAlteracao { get; set; }
    }
}