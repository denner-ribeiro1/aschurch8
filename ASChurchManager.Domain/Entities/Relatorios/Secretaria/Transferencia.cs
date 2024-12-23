using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASChurchManager.Domain.Types;

namespace ASChurchManager.Domain.Entities.Relatorios.Secretaria
{
    public class Transferencia
    {
        public string Nome { get; set; }

        public string CongregacaoOrigem { get; set; }

        public string CongregacaoDestino { get; set; }

        public string TipoCarta { get; set; }

        public string StatusCarta { get; set; }

        public DateTimeOffset DataDaTransferencia { get; set; }
    }
}
