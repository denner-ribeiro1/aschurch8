using System;

namespace ASChurchManager.Domain.Entities
{
    public class HistoricoCartas  : BaseEntity
    {
        public string CongregacaoOrigem { get; set; }

        public string CongregacaoDestino { get; set; }

        public DateTimeOffset DataDaTransferencia { get; set; }
    }
}
