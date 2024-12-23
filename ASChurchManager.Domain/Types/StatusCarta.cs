using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.Domain.Types
{
    public enum StatusCarta
    {
        [Display(Name = "Não Definido")]
        NaoDefinido = 0,
        [Display(Name = "Aguardando Recebimento")]
        AguardandoRecebimento = 1,
        [Display(Name = "Finalizado")]
        Finalizado = 2,
        [Display(Name = "Cancelado")]
        Cancelado = 3
    }
}
