using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    public enum MembroSituacao : byte
    {
        [Display(Name = " ")]
        NaoDefinido = 0,

        [Display(Name = "Comunhão")]
        Comunhao = 1,

        [Display(Name = "Disciplinado")]
        Disciplinado = 2,

        [Display(Name = "Desviado")]
        Desviado = 3,

        [Display(Name = "Mudou-se")]
        Mudou = 4,

        [Display(Name = "Falecido")]
        Falecido = 5,
        
        [Display(Name = "Abandono")]
        Abandono = 6

    }
}