using ASChurchManager.Web.ViewModels.Shared;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Admin.ViewModels.Usuario
{
    public class AlteracaoSenhaUsuarioViewModel : EntityViewModelBase
    {
        [Required]
        public string Nome { get; set; }

        [Display(Name = "Usuário")]
        public string Username { get; set; }

        [Display(Name = "Senha"),
        Required(ErrorMessage = "Senha é de preenchimento obrigatório"),
        DataType(DataType.Password)]
        public string Senha { get; set; }

        [DataType(DataType.Password),
        Compare("Senha", ErrorMessage = "As senhas não são idênticas"),
        Display(Name = "Redigite sua senha")]
        public string RedigiteSenha { get; set; }

        public bool AlterarSenhaProxLogin { get; set; }
    }
}