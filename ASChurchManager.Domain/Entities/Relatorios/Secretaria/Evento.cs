using System;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Entities.Relatorios.Secretaria
{
    public class Evento
    {
        public int Id { get; set; }

        public DateTime DataHoraInicio { get; set; }

        public DateTime DataHoraFim { get; set; }

        public Entities.Evento.TipoEvento TipoEvento { get; set; }

        public string Descricao { get; set; }

        public string Congregacao { get; set; }
    }

    public class Feriado
    {
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
    }

    public class EventosFeriados
    {
        public EventosFeriados()
        {
            Feriados = new List<Feriado>();
            Eventos = new List<Evento>();
        }

        public List<Feriado> Feriados { get; set; }
        public List<Evento> Eventos { get; set; }
    }
}
