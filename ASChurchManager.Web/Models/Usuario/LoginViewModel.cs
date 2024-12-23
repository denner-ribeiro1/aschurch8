using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.ViewModels.Usuario
{
    public class LoginViewModel
    {
        [StringLength(30, ErrorMessage = "O campo Usuário deve ter até 30 caracteres."), 
         Display(Name = "Usuário"), Required(ErrorMessage = "Usuário é de preenchimento obrigatório")]
        public string Username { get; set; }

        [Display(Name = "Senha"), Required(ErrorMessage = "Senha é de preenchimento obrigatório"), DataType(DataType.Password)]
        public string Senha { get; set; }
    }
}