using ASChurchManager.Domain.Entities;
using System;
using System.Collections.Generic;

namespace ASChurchManager.Web.Models.Shared
{
    [Serializable]
    public class EventosDashBoardVM
    {
        public EventosDashBoardVM()
        {
            UltimaAtualizacao = DateTime.Now;
        }

        public Dashboard Dashboard { get; set; }

        public IEnumerable<Evento> Eventos { get; set; }

        public List<Feriado> Feriados { get; set; }

        public DateTime UltimaAtualizacao { get; set; }
    }
}
