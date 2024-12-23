using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    public enum Meses : byte
    {
        [Display(Name = "Janeiro")]
        JAN = 1,
        [Display(Name = "Fevereiro")]
        FEV = 2,
        [Display(Name = "Março")]
        MAR = 3,
        [Display(Name = "Abril")]
        ABR = 4,
        [Display(Name = "Maio")]
        MAI = 5,
        [Display(Name = "Junho")]
        JUN = 6,
        [Display(Name = "Julho")]
        JUL = 7,
        [Display(Name = "Agosto")]
        AGO = 8,
        [Display(Name = "Setembro")]
        SET = 9,
        [Display(Name = "Outubro")]
        OUT = 10,
        [Display(Name = "Novembro")]
        NOV = 11,
        [Display(Name = "Dezembro")]
        DEZ = 12
    }
}