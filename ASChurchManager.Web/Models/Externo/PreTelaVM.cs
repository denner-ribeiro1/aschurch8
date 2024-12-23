using ASChurchManager.Web.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASChurchManager.Web.Models.Externo
{
    public class PreTelaVM
    {
        public long Id { get; set; }
        [Display(Name = "CPF"), Required(ErrorMessage = "CPF é de preenchimento obrigatório"),
         Cpf(ErrorMessage = "CPF Inválido")]
        public string Cpf { get; set; }

        public string CpfDigitado { get; set; }

        [Display(Name = "Primeiro Nome da Mãe"),
         Required(ErrorMessage = "Primeiro Nome da Mãe é de preenchimento obrigatório")]
        public string NomeMae { get; set; }

        [Display(Name = "Data de Nascimento"),
         Required(ErrorMessage = "Data de Nascimento é de preenchimento obrigatório"),
         DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataNascimento { get; set; }
    }
}
