using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Lib;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Models.Externo
{
    public class DadosMembroVM
    {
        public DadosMembroVM()
        {
            DataNascimento = null;
        }
        public long Id { get; set; }

        [Required(ErrorMessage = "Nome é de preenchimento obrigatório"),
         Display(Name = "Nome"),
         StringLength(100)]
        public string Nome { get; set; }

        [Display(Name = "CPF"),
         Cpf(ErrorMessage = "CPF Inválido")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "RG é de preenchimento obrigatório"),
         Display(Name = "RG")]
        public string Rg { get; set; }

        [Display(Name = "Orgão Emissor")]
        public string OrgaoEmissor { get; set; }

        [Display(Name = "Data de Nascimento"),
        Required(ErrorMessage = "Data de Nascimento é de preenchimento obrigatório"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataNascimento { get; set; }

        [Display(Name = "Estado Civil")]
        public EstadoCivil? EstadoCivil { get; set; }

        public IEnumerable<SelectListItem> SelectEstadoCivil { get; set; }


        [Display(Name = "Escolaridade")]
        public Escolaridade? Escolaridade { get; set; }

        [Display(Name = "Sexo")]
        public Sexo? Sexo { get; set; }

        [Display(Name = "Nome do Pai")]
        public string NomePai { get; set; }

        [Display(Name = "Nome da Mãe"),
        Required(ErrorMessage = "Nome da Mãe é de preenchimento obrigatório")]
        public string NomeMae { get; set; }

        public string Nacionalidade { get; set; } = "Brasileira";

        [Display(Name = "Natural Estado")]
        public ASChurchManager.Domain.Types.Estado? NaturalidadeEstado { get; set; }

        [Display(Name = "Natural Cidade")]
        public string NaturalidadeCidade { get; set; }

        [Display(Name = "Profissão")]
        public string Profissao { get; set; }

        [Display(Name = "Tel.Residencial")]
        public string TelefoneResidencial { get; set; }

        [Display(Name = "Tel.Celular")]
        public string TelefoneCelular { get; set; }

        [Display(Name = "Tel.Comercial")]
        public string TelefoneComercial { get; set; }

        [Display(Name = "E-mail"),
        DataType(DataType.EmailAddress),
        EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        public string Nacionalidade1 { get => Nacionalidade; set => Nacionalidade = value; }

        [StringLength(100),
       Required(ErrorMessage = "Logradouro é de preenchimento obrigatório")]
        public string Logradouro { get; set; }

        [Display(Name = "Número"),
        Required(ErrorMessage = "Número é de preenchimento obrigatório"),
        StringLength(5)]
        public string Numero { get; set; }

        [StringLength(50)]
        public string Complemento { get; set; }

        [StringLength(50),
        Required(ErrorMessage = "Bairro é de preenchimento obrigatório")]
        public string Bairro { get; set; }

        [StringLength(50),
        Required(ErrorMessage = "Cidade é de preenchimento obrigatório")]
        public string Cidade { get; set; }

        [Display(Name = "Estado"),
        Required(ErrorMessage = "Estado é de preenchimento obrigatório")]
        public ASChurchManager.Domain.Types.Estado? Estado { get; set; }

        private string _pais = "Brasil";
        [Display(Name = "País"),
        Required(ErrorMessage = "País é de preenchimento obrigatório")]
        public string Pais
        {
            get { return _pais; }
            set { _pais = value; }
        }

        [Display(Name = "CEP"),
        StringLength(10),
        Required(ErrorMessage = "CEP é de preenchimento obrigatório")]
        public string Cep { get; set; }

        public bool AceitaTermos { get; set; }

        public string DataTermo { get; set; }

        public string SiteKey { get; set; }
        public string IP { get; set; }
    }
}
