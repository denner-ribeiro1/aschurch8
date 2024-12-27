using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.API.Membro.Models;

public class LoginDTO
{
    [Required(ErrorMessage = "CPF é de preenchimento obrigatorio ")]
    public string? cpf { get; set; }

    [Required(ErrorMessage = "Senha é de preenchimento obrigatorio ")]
    public string? senha { get; set; }
}
