using ASChurchManager.Domain.Types;
using ASChurchManager.Web.ViewModels.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Batismo
{
    public class GridConfiguracaoBatismoViewModel : EntityViewModelBase
    {
        [Key]
        public override long Id { get; set; }

        [Display(Name = "Data do Batismo"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataBatismo { get; set; }

        [Display(Name = "Data Máxima para o cadastro do Batismo"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataMaximaCadastro { get; set; }

        [Display(Name = "Status")]
        public StatusBatismo Status { get; set; }

        [Display(Name = "Status")]
        public string StatusDescr
        {
            get
            {
                return Status.GetDisplayAttributeValue();
            }
        }
    }
}