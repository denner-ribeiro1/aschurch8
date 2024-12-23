
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    public enum TipoMembro
    {
        [Display(Name = " ")]
        NaoDefinido = 0,

        [Display(Name = "Congregado")]
        Congregado = 1,

        [Display(Name = "Candidato ao Batismo")]
        Batismo = 2,

        [Display(Name = "Membro")]
        Membro = 3
    }
}
