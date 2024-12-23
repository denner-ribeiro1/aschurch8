using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    public enum SimNao
    {
        [Display(Name = "Não")]
        Nao,
        [Display(Name = "Sim")]
        Sim,
    }
}
