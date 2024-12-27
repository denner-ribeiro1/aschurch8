using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.API.Membro.Models;

public class InscricaoDTO
{

    [Required(ErrorMessage = "Cpf é de preenchimento obrigatorio ")]
    public string cpf { get; set; }

    [Required(ErrorMessage = "Primeiro nome da mãe é de preenchimento obrigatorio ")]
    public string nomeMae { get; set; }

    [Required(ErrorMessage = "Data de nascimento é de preenchimento obrigatorio ")]
    public DateTime dataNascimento { get; set; }

    [Required(ErrorMessage = "E-mail é de preenchimento obrigatorio ")]
    public string email { get; set; }


}
