using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    public enum StatusPresenca
    {
        [Display(Name = "")]
        Nulo = 0,
        [Display(Name = "Em Aberto")]
        EmAberto = 1,
        [Display(Name = "Andamento")]
        Andamento = 2,
        [Display(Name = "Finalizado")]
        Finalizado = 3
    }

    public enum SituacaoPresenca
    {
        [Display(Name = "")]
        Nulo = 0,
        [Display(Name = "Presente")]
        Presente = 1,
        [Display(Name = "Ausente")]
        Ausente = 2,
    }

    public enum TipoRegistro
    {
        [Display(Name = "")]
        Nulo = 0,
        [Display(Name = "Manual")]
        Manual = 1,
        [Display(Name = "Automática")]
        Automatica = 2,
    }
}
