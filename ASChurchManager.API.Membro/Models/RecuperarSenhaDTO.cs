using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.API.Membro.Models;

public class RecuperarSenhaDTO
{
    [Required(ErrorMessage = "CPF é de preenchimento obrigatorio ")]
    public string? cpf { get; set; }


}
