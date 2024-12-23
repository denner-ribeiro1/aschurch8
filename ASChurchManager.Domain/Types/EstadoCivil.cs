using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    public enum EstadoCivil : byte
    {
        [Display(Name = " ")]
        NaoDefinido = 0,
        [Display(Name = "Solteiro")]
        Solteiro = 1,
        [Display(Name = "Casado")]
        Casado = 2,
        [Display(Name = "Divorciado")]
        Divorciado = 3,
        [Display(Name = "Viúvo")]
        Viuvo = 4
    }
}