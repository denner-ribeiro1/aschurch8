using ASChurchManager.Domain.Types;
using System;

namespace ASChurchManager.Domain.Entities
{
    public class SituacaoMembro : BaseEntity
    {
        public int MembroId { get; set; }
        public MembroSituacao Situacao { get; set; }
        public DateTimeOffset? Data { get; set; }
        public string Observacao { get; set; }
    }
}