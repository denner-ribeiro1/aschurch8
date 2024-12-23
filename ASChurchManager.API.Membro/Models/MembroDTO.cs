using System;

namespace ASChurchManager.API.Membro.Models;

public class MembroDTO
{
    public int RM { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Congregacao { get; set; }
    public string? Cargo { get; set; }

}
