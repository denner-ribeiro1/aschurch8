using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    public enum StatusEmail
    {
        [Display(Name = "")]
        Nulo = 0,

        [Display(Name = "Aguardando Envio")]
        AguardandoEnvio = 1,

        [Display(Name = "Enviado")]
        Enviado = 2,

        [Display(Name = "Falha no Envio")]
        FalhaEnvio = 3
    }
}
