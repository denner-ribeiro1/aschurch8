using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.API.Membro.Models;

public class LoginDTO
{
    [Required(ErrorMessage = "CPF é de preenchimento obrigatorio ")]
    public string? Cpf { get; set; }

    [Required(ErrorMessage = "Senha é de preenchimento obrigatorio ")]
    public string? Senha { get; set; }
}
