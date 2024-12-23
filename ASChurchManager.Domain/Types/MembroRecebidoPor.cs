using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    public enum MembroRecebidoPor : byte
    {
        [Display(Name = " ")]
        NaoDefinido = 0,

        [Display(Name = "Aclamação")]
        Aclamacao = 1,

        [Display(Name = "Batismo")]
        Batismo = 2,

        [Display(Name = "Mudança")]
        Mudanca = 3
    }
}