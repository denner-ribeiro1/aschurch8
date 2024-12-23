using ASChurchManager.Domain.Types;
using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Congregado
{

    public class CongregadoViewModel : EntityViewModelBase
    {
        public CongregadoViewModel()
        {
            this.ViewModelTitle = "Congregados";
            this.Endereco = new EnderecoViewModel();
        }

        [Key,
        Required(ErrorMessage = "O campo Código de Registro é obrigatório"),
        Display(Name = "Código Registro"),
        Remote("VerificaCodigoRegistroDuplicado", "Membro", "Secretaria", ErrorMessage = "Código de Registro já existe")]
        public override long Id { get; set; }

        [Required(ErrorMessage = "O campo Código da Congregação é obrigatório"),
        Display(Name = "Cód. Congregação")]
        public long CongregacaoId { get; set; }

        [Required(ErrorMessage = "O campo Nome da Congregação é obrigatório"),
        Display(Name = "Nome Congregação")]
        public string CongregacaoNome { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório"),
         Display(Name = "Nome"),
         StringLength(100)]
        public string Nome { get; set; }

        [Display(Name = "Data de Nascimento"),
        Required(ErrorMessage = "O campo Data de Nascimento é obrigatório"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataNascimento { get; set; }

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

        public EnderecoViewModel Endereco { get; set; }

        [Display(Name = "Tipo")]
        public TipoMembro TipoMembro { get; set; }

        public Acao Acao { get; set; }
        [Display(Name = "Congregado Ativo?")]
        public bool Ativo { get; set; }
    }
}