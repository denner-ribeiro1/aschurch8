using ASChurchManager.Domain.Types;
using System;

namespace ASChurchManager.Domain.Entities
{
    public class PresencaDatas
    {
        public Acao Acao { get; set; }
        public int Id { get; set; }
        public int EventoId { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public StatusPresenca Status { get; set; }
    }

}
