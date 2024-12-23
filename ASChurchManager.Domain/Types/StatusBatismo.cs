using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    public enum StatusBatismo
    {
        [Display(Name = "")]
        Nulo = 0,
        [Display(Name = "Em Aberto")]
        EmAberto = 1,
        [Display(Name = "Finalizado")]
        Finalizado = 2,
        [Display(Name = "Cancelado")]
        Cancelado = 3
    }

    public enum SituacaoCandidatoBatismo
    {
        [Display(Name = "")]
        Nulo = 0,
        [Display(Name = "Presente")]
        Presente = 1,
        [Display(Name = "Ausente")]
        Ausente = 2,
    }
}
