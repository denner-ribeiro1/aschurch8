using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Admin.ViewModels.Usuario
{
    public class GridUsuarioViewModel
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Nome { get; set; }

        [StringLength(30, ErrorMessage = "O campo Usuário deve ter até 30 caracteres."),
         Display(Name = "Usuário"), Required(ErrorMessage = "Usuário é de preenchimento obrigatório")]
        public string Username { get; set; }

        [Required(ErrorMessage = "E-mail inválido")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Congregação")]
        public string Congregacao { get; set; }
    }
}