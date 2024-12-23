using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    public enum Escolaridade : byte
    {
        [Display(Name = "Fundamental")]
        Fundamental = 1,
        [Display(Name = "Médio")]
        Medio = 2,
        [Display(Name = "Superior")]
        Superior = 3,
        [Display(Name = "Pós-graduação")]
        Posgraduacao = 4
    }
}
