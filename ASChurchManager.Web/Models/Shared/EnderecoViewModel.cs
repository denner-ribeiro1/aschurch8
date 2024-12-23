using ASChurchManager.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.ViewModels.Shared
{
    public class EnderecoViewModel : ViewModelBase
    {
        public EnderecoViewModel()
        {
            this.ViewModelTitle = "Endereço";
        }

        [StringLength(100),
        Required(ErrorMessage = "Logradouro é de preenchimento obrigatório")]
        public string Logradouro { get; set; }

        [Display(Name = "Número"),
        Required(ErrorMessage = "Número é de preenchimento obrigatório"),
        StringLength(10)]
        public string Numero { get; set; }

        [StringLength(100)]
        public string Complemento { get; set; }

        [StringLength(100),
        Required(ErrorMessage = "Bairro é de preenchimento obrigatório")]
        public string Bairro { get; set; }

        [StringLength(100),
        Required(ErrorMessage = "Cidade é de preenchimento obrigatório")]
        public string Cidade { get; set; }

        [Display(Name = "Estado"),
        Required(ErrorMessage = "Estado é de preenchimento obrigatório")]
        public ASChurchManager.Domain.Types.Estado? Estado { get; set; }

        [Display(Name = "País"),
        Required(ErrorMessage = "País é de preenchimento obrigatório")]
        public string Pais { get; set; }

        [Display(Name = "CEP"),
        StringLength(10),
        Required(ErrorMessage = "CEP é de preenchimento obrigatório")]
        public string Cep { get; set; }

        [Display(Name = "Código Postal"),
        Required(ErrorMessage = "Código Postal é de preenchimento obrigatório"),
        StringLength(15)]
        public string CodigoPostal { get; set; }

        [Display(Name = "Bairro"),
        StringLength(100)]
        public string BairroEstrangeiro { get; set; }

        [Display(Name = "Cidade"),
        StringLength(100),
        Required(ErrorMessage = "Cidade é de preenchimento obrigatório")]
        public string CidadeEstrangeiro { get; set; }

        [Display(Name = "Estado/Província"),
        Required(ErrorMessage = "Estado/Província é de preenchimento obrigatório"),
        StringLength(30)]
        public string Provincia { get; set; }

        public IEnumerable<SelectListItem> SelectPaises { get; set; }
    }
}