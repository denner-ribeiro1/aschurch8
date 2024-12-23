using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Lib;
using ASChurchManager.Web.ViewModels.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Membro
{
    public class HistoricoMembroVM : EntityViewModelBase
    {
        public HistoricoMembroVM()
        {
        }

        [Display(Name = "Nome Atual")]
        public string Nome { get; set; }
        
        [Display(Name = "Nome Anterior")]
        public string NomeAnterior { get; set; }
        
        [Display(Name = "CPF Atual")]
        public string Cpf { get; set; }
        
        [Display(Name = "CPF Anterior")]
        public string CpfAnterior { get; set; }
        
        [Display(Name = "RG Atual")]
        public string Rg { get; set; }
        
        [Display(Name = "RG Anterior")]
        public string RgAnterior { get; set; }
        
        [Display(Name = "Orgão Emissor Atual")]
        public string OrgaoEmissor { get; set; }
        
        [Display(Name = "Orgão Emissor Anterior")]
        public string OrgaoEmissorAnterior { get; set; }
        
        [Display(Name = "Dt.Nasc. Atual")]
        public string DataNascimento { get; set; }
        
        [Display(Name = "Dt.Nasc. Anterior")]
        public string DataNascimentoAnterior { get; set; }
        
        [Display(Name = "Estado Civil Atual")]
        public string EstadoCivil { get; set; }
        
        [Display(Name = "Estado Civil Anterior")]
        public string EstadoCivilAnterior { get; set; }
        
        [Display(Name = "Escolaridade Atual")]
        public string Escolaridade { get; set; }
        
        [Display(Name = "Escolaridade Anterior")]
        public string EscolaridadeAnterior { get; set; }
        
        [Display(Name = "Sexo Atual")]
        public string Sexo { get; set; }
        
        [Display(Name = "Sexo Anterior")]
        public string SexoAnterior { get; set; }
        
        [Display(Name = "Nome Pai Atual")]
        public string NomePai { get; set; }
        
        [Display(Name = "Nome Pai Anterior")]
        public string NomePaiAnterior { get; set; }
        
        [Display(Name = "Nome Mãe Atual")]
        public string NomeMae { get; set; }
        
        [Display(Name = "Nome Mãe Anterior")]
        public string NomeMaeAnterior { get; set; }

        [Display(Name = "Natural Cidade Atual")]
        public string NaturalidadeCidade { get; set; }

        [Display(Name = "Natural Cidade Anterior")]
        public string NaturalidadeCidadeAnterior { get; set; }

        [Display(Name = "Natural Estado Atual")]
        public string NaturalidadeEstado { get; set; }
        
        [Display(Name = "Natural Estado Anterior")]
        public string NaturalidadeEstadoAnterior { get; set; }

        [Display(Name = "Nacionalidade Atual")]
        public string Nacionalidade { get; set; }
        
        [Display(Name = "Nacionalidade Anterior")]
        public string NacionalidadeAnterior { get; set; }
        
        [Display(Name = "Profissão Atual")]
        public string Profissao { get; set; }
        
        [Display(Name = "Profissão Anterior")]
        public string ProfissaoAnterior { get; set; }
        
        [Display(Name = "Tel.Res.Atual")]
        public string TelefoneResidencial { get; set; }
        
        [Display(Name = "Tel.Res.Anterior")]
        public string TelefoneResidencialAnterior { get; set; }
        
        [Display(Name = "Tel.Cel.Atual")]
        public string TelefoneCelular { get; set; }
        
        [Display(Name = "Tel.Cel.Anterior")]
        public string TelefoneCelularAnterior { get; set; }
        
        [Display(Name = "Tel.Com.Atual")]
        public string TelefoneComercial { get; set; }
        
        [Display(Name = "Tel.Com.Anterior")]
        public string TelefoneComercialAnterior { get; set; }
        
        [Display(Name = "E-mail Atual")]
        public string Email { get; set; }
        
        [Display(Name = "E-mail Anterior")]
        public string EmailAnterior { get; set; }
        
        [Display(Name = "Logradouro Atual")]
        public string Logradouro { get; set; }
        
        [Display(Name = "Logradouro Anterior")]
        public string LogradouroAnterior { get; set; }
        
        [Display(Name = "Número Atual")]
        public string Numero { get; set; }
        
        [Display(Name = "Número Anterior")]
        public string NumeroAnterior { get; set; }
        
        [Display(Name = "Complemento Atual")]
        public string Complemento { get; set; }
        
        [Display(Name = "Complemento Anterior")]
        public string ComplementoAnterior { get; set; }
        
        [Display(Name = "Bairro Atual")]
        public string Bairro { get; set; }
        
        [Display(Name = "Bairro Anterior")]
        public string BairroAnterior { get; set; }
        
        [Display(Name = "Cidade Atual")]
        public string Cidade { get; set; }
        
        [Display(Name = "Cidade Anterior")]
        public string CidadeAnterior { get; set; }
        
        [Display(Name = "Estado Atual")]
        public string Estado { get; set; }
        
        [Display(Name = "Estado Anterior")]
        public string EstadoAnterior { get; set; }
        
        [Display(Name = "Pais Atual")]
        public string Pais { get; set; }
        
        [Display(Name = "Pais Anterior")]
        public string PaisAnterior { get; set; }
        
        [Display(Name = "CEP Atual")]
        public string Cep { get; set; }
        
        [Display(Name = "CEP Anterior")]
        public string CepAnterior { get; set; }

        public string IpConfirmado { get; set; }

        public bool MembroConfirmado { get; set; }
    }
}
