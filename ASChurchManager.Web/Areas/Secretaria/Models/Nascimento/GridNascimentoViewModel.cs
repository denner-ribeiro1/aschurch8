using ASChurchManager.Domain.Types;
using ASChurchManager.Web.ViewModels.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Nascimento
{
    public class GridNascimentoViewModel : EntityViewModelBase
    {
        public GridNascimentoViewModel()
        {
            ViewModelTitle = "Nascimento";
        }

        [Key]
        public override long Id { get; set; }

        [Display(Name = "Data de Apresentação"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset DataApresentacao { get; set; }

        [Display(Name = "Nome do Pai")]
        public string NomePai { get; set; }

        [Display(Name = "Nome da Mãe")]
        public string NomeMae { get; set; }

        [Display(Name = "Nome da Criança")]
        public string Crianca { get; set; }

        [Display(Name = "Sexo")]
        public Sexo Sexo { get; set; }
    }
}