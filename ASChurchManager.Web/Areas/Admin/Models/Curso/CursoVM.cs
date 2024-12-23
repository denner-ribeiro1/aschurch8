using ASChurchManager.Domain.Types;
using ASChurchManager.Web.ViewModels.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ASChurchManager.Web.Areas.Admin.ViewModels.Curso
{
    public class CursoVM : EntityViewModelBase
    {
        [Key, Display(Name = "Código")]
        public override long Id { get; set; }

        [Display(Name = "Descrição"),
         Required(ErrorMessage = "Descrição é obrigatório"),
         StringLength(100)]
        public string Descricao { get; set; }

        [Display(Name = "Data de Início"),
         Required(ErrorMessage = "Data de Início é obrigatória"),
         DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataInicio { get; set; }

        public string DisplayedDataInicio
        {
            get
            {
                return DataInicio.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }

        [Display(Name = "Data de Encerramento"),
         Required(ErrorMessage = "Data de Encerramento é obrigatória"),
         DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataEncerramento { get; set; }

        public string DisplayedDataEncerramento
        {
            get
            {
                return DataEncerramento.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }

        [Display(Name = "Carga Horária (Em Horas)"),
         Required(ErrorMessage = "Carga horária é obrigatória")]
        public int? CargaHoraria { get; set; }

        public Acao Acao { get; set; }
    }
}