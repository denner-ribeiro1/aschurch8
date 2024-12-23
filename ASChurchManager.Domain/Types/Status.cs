using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    [Serializable]
    public enum Status : byte
    {
        [Display(Name = " ")]
        NaoDefinido = 0,

        [Display(Name = "Ativo")]
        Ativo = 1,

        [Display(Name = "Inativo")]
        Inativo = 2,

        [Display(Name = "Pendente Aprovação")]
        PendenteAprovacao = 3,

        [Display(Name = "Não Aprovado")]
        NaoAprovado = 4,

        [Display(Name = "Mudou-se")]
        Mudou = 5,

        [Display(Name = "Falecido")]
        Falecido = 6
    }
}