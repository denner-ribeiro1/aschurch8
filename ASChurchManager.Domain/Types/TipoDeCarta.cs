using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    public enum TipoDeCarta : byte
    {
        [Display(Name = "Transferência")]
        Transferencia = 1,
        [Display(Name = "Mudança")]
        Mudanca = 2,
        [Display(Name = "Recomendação")]
        Recomendacao = 3
    }
}
