using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.API.Membro.Models;

public class SenhaDTO
{
    [Required(ErrorMessage = "RM é de preenchimento obrigatorio ")]
    public int id { get; set; }

    [Required(ErrorMessage = "Senha é de preenchimento obrigatorio ")]
    public required string senhaAtual { get; set; }

    [Required(ErrorMessage = "Nova senha é de preenchimento obrigatorio ")]
    public required string novaSenha { get; set; }
}
