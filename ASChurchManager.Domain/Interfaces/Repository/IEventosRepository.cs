using ASChurchManager.Domain.Entities;
using System;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface IEventosRepository : IRepositoryDAO<Evento>
    {
        IEnumerable<Evento> GetEventos(int ano, int mes, int tipoPesquisa, int codigoCongr, out List<Feriado> feriados);
        IEnumerable<Evento> ListarEventosObrigatorio(DateTimeOffset dataHoraInicio, DateTimeOffset dataHoraFim, int congregacao,
            Evento.TipoFrequencia frequencia, int quantidade);
        int Delete(long id, bool excluirVinc = false);
        IEnumerable<Evento> ListarEventosPorData(DateTimeOffset dataHoraInicio, DateTimeOffset dataHoraFim, out List<Feriado> feriados);
    }
}
