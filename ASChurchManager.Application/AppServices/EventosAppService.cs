using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using System;
using System.Collections.Generic;

namespace ASChurchManager.Application.AppServices
{
    public class EventosAppService : BaseAppService<Evento>, IEventosAppService
    {
        private readonly IEventosRepository _eventosService;

        public EventosAppService(IEventosRepository eventosService)
            : base(eventosService)
        {
            _eventosService = eventosService;
        }

        public int Delete(long id, bool excluirVinc = false)
        {
            return _eventosService.Delete(id, excluirVinc);
        }

        public IEnumerable<Evento> GetEventos(int ano, int mes, int tipoPesquisa, int codigoCongr, out List<Feriado> feriados)
        {
            return _eventosService.GetEventos(ano, mes, tipoPesquisa, codigoCongr, out feriados);
        }

        public IEnumerable<Evento> ListarEventosObrigatorio(DateTimeOffset dataHoraInicio, DateTimeOffset dataHoraFim, int congregacao,
            Evento.TipoFrequencia frequencia, int quantidade)
        {
            return _eventosService.ListarEventosObrigatorio(dataHoraInicio, dataHoraFim, congregacao, frequencia, quantidade);
        }

        public IEnumerable<Evento> ListarEventosPorData(DateTimeOffset dataHoraInicio, DateTimeOffset dataHoraFim, out List<Feriado> feriados)
        {
            return _eventosService.ListarEventosPorData(dataHoraInicio, dataHoraFim, out feriados);
        }
    }
}
