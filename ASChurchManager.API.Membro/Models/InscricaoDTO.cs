using System.ComponentModel.DataAnnotations;
using ASChurchManager.API.Membro.Infra;
using Microsoft.AspNetCore.Mvc;

namespace ASChurchManager.API.Membro.Models;

[Serializable]
public class InscricaoDTO
{

    [Required(ErrorMessage = "Cpf é de preenchimento obrigatorio ")]
    public required string cpf { get; set; }

    [Required(ErrorMessage = "Primeiro nome da mãe é de preenchimento obrigatorio ")]
    public required string nomeMae { get; set; }

    [Required(ErrorMessage = "Data de nascimento é de preenchimento obrigatorio ")]
    [ModelBinder(BinderType = typeof(DateTimeModelBinder))]
    public DateTime dataNascimento { get; set; }



}
