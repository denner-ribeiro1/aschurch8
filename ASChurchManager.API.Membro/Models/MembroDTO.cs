using System;

namespace ASChurchManager.API.Membro.Models;

public class MembroDTO
{


    public int rm { get; set; }
    public string? nome { get; set; }
    public string? email { get; set; }
    public string? congregacao { get; set; }
    public string? cargo { get; set; }
    public bool atualizarSenha { get; set; }
    public string? foto { get; set; }
    public bool membroAtualizado { get; set; }


}
