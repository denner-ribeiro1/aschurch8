using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.Domain.Types
{
    public enum Estado : byte
    {
        [Display(Name = "São Paulo")]
        SP = 1,
        [Display(Name = "Acre")]
        AC = 2,
        [Display(Name = "Alagoas")]
        AL = 3,
        [Display(Name = "Amapá")]
        AP = 4,
        [Display(Name = "Amazonas")]
        AM = 5,
        [Display(Name = "Bahia")]
        BA = 6,
        [Display(Name = "Ceará")]
        CE = 7,
        [Display(Name = "Distrito Federal")]
        DF = 8,
        [Display(Name = "Espírito Santo")]
        ES = 9,
        [Display(Name = "Goiás")]
        GO = 10,
        [Display(Name = "Maranhão")]
        MA = 11,
        [Display(Name = "Mato Grosso")]
        MT = 12,
        [Display(Name = "Mato Grosso do Sul")]
        MS = 13,
        [Display(Name = "Minas Gerais")]
        MG = 14,
        [Display(Name = "Paraná")]
        PR = 15,
        [Display(Name = "Paraíba")]
        PB = 16,
        [Display(Name = "Pará")]
        PA = 17,
        [Display(Name = "Pernambuco")]
        PE = 18,
        [Display(Name = "Piauí")]
        PI = 19,
        [Display(Name = "Rio de Janeiro")]
        RJ = 20,
        [Display(Name = "Rio Grande do Norte")]
        RN = 21,
        [Display(Name = "Rio Grande do Sul")]
        RS = 22,
        [Display(Name = "Rondônia")]
        RO = 23,
        [Display(Name = "Roraima")]
        RR = 24,
        [Display(Name = "Santa Catarina")]
        SC = 25,
        [Display(Name = "Sergipe")]
        SE = 26,
        [Display(Name = "Tocantins")]
        TO = 27,
        [Display(Name = "Exterior")]
        EX = 28
    }
}
