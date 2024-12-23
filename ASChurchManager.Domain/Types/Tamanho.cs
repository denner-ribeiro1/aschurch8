using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ASChurchManager.Domain.Types
{
    public enum Tamanho
    {
        [Display(Name = "")]
        Nulo,
        [Display(Name = "PP")]
        PP, //Extra Pequeno
        [Display(Name = "P")]
        P, // Pequeno
        [Display(Name = "M")]
        M, // Médio
        [Display(Name = "G")]
        G, // Grande
        [Display(Name = "GG")]
        GG // Extra Grande
    }
}
