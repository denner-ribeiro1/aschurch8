using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    public enum TipoCarteirinha : byte
    {
        [Display(Name = "Membro")]
        Membro = 0,
        [Display(Name = "Diácono")]
        Diacono = 1,
        [Display(Name = "Presbítero")]
        Presbitero = 2,
        [Display(Name = "Evangelista")]
        Evangelista = 3,
        [Display(Name = "Pastor")]
        Pastor = 4,
        [Display(Name = "Cooperador")]
        Cooperador = 5
    }
}
