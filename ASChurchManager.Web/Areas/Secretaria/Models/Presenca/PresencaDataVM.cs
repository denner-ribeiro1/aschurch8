using ASChurchManager.Domain.Types;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Presenca
{
    public class PresencaDataVM
    {
        public Acao Acao { get; set; }

        public int Id { get; set; }

        [Display(Name = "Data"),
         DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true),
         Required(ErrorMessage = "Data é de preenchimento obrigatório")]
        //public DateTime? Data { get; set; }
        public string Data { get; set; }

        [DataType(DataType.Time),
         Display(Name = "Hora Inicio"),
         DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true),
         Required(ErrorMessage = "Hora Inicio é de preenchimento obrigatório")]
        //public TimeSpan HoraInicio { get; set; }
        public string HoraInicio { get; set; }

        [DataType(DataType.Time),
         Display(Name = "Hora Final"),
         DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true),
         Required(ErrorMessage = "Hora Final é de preenchimento obrigatório")]
        //public TimeSpan HoraFinal { get; set; }
        public string HoraFinal { get; set; }

        public StatusPresenca Status { get; set; }

    }
}
