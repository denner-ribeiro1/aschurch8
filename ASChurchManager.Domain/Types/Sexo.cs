
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    public enum Sexo : byte
    {
        [Display(Name = "Masculino")]
        Masculino = 1,
        [Display(Name = "Feminino")]
        Feminino = 2
    }
}
