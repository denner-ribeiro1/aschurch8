using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Lib;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.ViewModels.Shared
{
    public class PessoaViewModel : ViewModelBase
    {
        public PessoaViewModel()
        {
            ViewModelTitle = "Pessoa";
            DataNascimento = null;
        }

        public string FotoPath { get; set; }

        public string FotoUrl { get; set; }

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

        public bool PaiMembro { get; set; }

        [Display(Name = "Cód.Reg")]
        public long? PaiId { get; set; }

        [Display(Name = "Nome do Pai")]
        public string NomePai { get; set; }

        public bool MaeMembro { get; set; }

        [Display(Name = "Cód.Reg")]
        public long? MaeId { get; set; }

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

        [Display(Name = "Tel.Residencial")]
        public string TelefoneResidencialEstrangeiro { get; set; }

        [Display(Name = "Tel.Celular")]
        public string TelefoneCelular { get; set; }

        [Display(Name = "Tel.Celular")]
        public string TelefoneCelularEstrangeiro { get; set; }

        [Display(Name = "Tel.Comercial")]
        public string TelefoneComercial { get; set; }

        [Display(Name = "Tel.Comercial")]
        public string TelefoneComercialEstrangeiro { get; set; }

        [Display(Name = "E-mail"),
        DataType(DataType.EmailAddress),
        EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        [Display(Name = "Tit.Eleitor Nº")]
        public string TituloEleitorNumero { get; set; }

        [Display(Name = "Tit.Eleitor Zona")]
        public string TituloEleitorZona { get; set; }

        [Display(Name = "Tit.Eleitor Seção")]
        public string TituloEleitorSecao { get; set; }

        public bool ConjugeMembro { get; set; }

        [Display(Name = "Cód.Reg.Cônjuge")]
        public long? IdConjuge { get; set; }

        [Display(Name = "Nome Cônjuge")]
        public string NomeConjuge { get; set; }
        public string Nacionalidade1 { get => Nacionalidade; set => Nacionalidade = value; }
    }
}