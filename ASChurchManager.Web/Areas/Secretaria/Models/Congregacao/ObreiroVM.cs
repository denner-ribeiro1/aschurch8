using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Congregacao
{
    public class ObreiroVM
    {
        public bool IsReadOnly { get; set; }

        [Display(Name = "Cód.Membro")]
        public long? ObreiroId { get; set; }

        [Display(Name = "Nome")]
        public string ObreiroNome { get; set; }

        [Display(Name = "Cargo")]
        public string ObreiroCargo { get; set; }
    }
}