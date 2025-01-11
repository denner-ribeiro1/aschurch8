using System;
using ASChurchManager.Domain.Types;

namespace ASChurchManager.Domain.Entities;

public class Email
{
    public int Id { get; set; }
    public int MembroId { get; set; }
    public string EmailAddress { get; set; }
    public string Assunto { get; set; }
    public string Corpo { get; set; }
    public StatusEmail Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
