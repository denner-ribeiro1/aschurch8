using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities.Relatorios.Secretaria;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Types;
using System;
using System.Collections.Generic;

namespace ASChurchManager.Application.AppServices
{
    public class RelatoriosSecretariaAppService : IRelatoriosSecretariaAppService
    {
        private readonly IRelatoriosSecretariaRepository _relatorioService;
        public RelatoriosSecretariaAppService(IRelatoriosSecretariaRepository relatorioService)
        {
            _relatorioService = relatorioService;
        }

        public IEnumerable<Aniversariantes> Aniversariantes(DateTimeOffset dataInicio, DateTimeOffset dataFinal, long congregacaoId, long usuarioID, TipoMembro tipoMembro)
        {
            return _relatorioService.Aniversariantes(dataInicio, dataFinal, congregacaoId, usuarioID, tipoMembro);
        }

        public IEnumerable<Batismo> CandidatosBatismo(long batismoId, DateTimeOffset dataBatismo, long congregacaoId, SituacaoCandidatoBatismo situacao, long usuarioID)
        {
            return _relatorioService.CandidatosBatismo(batismoId, dataBatismo, congregacaoId, situacao, usuarioID);
        }

        public IEnumerable<Transferencia> Transferencia(DateTimeOffset dataInicio, DateTimeOffset dataFinal, long congregacaoId, long usuarioID)
        {
            return _relatorioService.Transferencia(dataInicio, dataFinal, congregacaoId, usuarioID);
        }

        public IEnumerable<Nascimentos> Nascimento(DateTimeOffset dataInicio, DateTimeOffset dataFinal, long congregacaoId, long usuarioID)
        {
            return _relatorioService.Nascimento(dataInicio, dataFinal, congregacaoId, usuarioID);
        }

        public IEnumerable<Congregacoes> Congregacao(long congregacaoId, long usuarioID)
        {
            return _relatorioService.Congregacao(congregacaoId, usuarioID);
        }


        public IEnumerable<Obreiros> Obreiros(long congregacaoId, long usuarioID)
        {
            return _relatorioService.Obreiros(congregacaoId, usuarioID);
        }

        public IEnumerable<RelatorioMensal> RelatorioMensal(string mes, int ano, long congregacaoId, long usuarioID)
        {
            return _relatorioService.RelatorioMensal(mes, ano, congregacaoId, usuarioID);
        }

        public IEnumerable<RelatorioMembros> RelatorioMembros(long congrecacaoId, Status status, TipoMembro tipoMembro, EstadoCivil estadoCivil, bool abedabe,
            bool filtrarConf, bool ativosConf, long usuarioId)
        {
            return _relatorioService.RelatorioMembros(congrecacaoId, status, tipoMembro, estadoCivil, abedabe, filtrarConf, ativosConf, usuarioId);
        }

        public IEnumerable<CursoMembro> CursosMembro(long congregacaoId, int cursoId, long usuarioID)
        {
            return _relatorioService.CursosMembro(congregacaoId, cursoId, usuarioID);
        }

        public IEnumerable<Casamento> Casamento(DateTimeOffset dataInicio, DateTimeOffset dataFinal, long congregacaoId, long usuarioID)
        {
            return _relatorioService.Casamento(dataInicio, dataFinal, congregacaoId, usuarioID);
        }

        public long Add(Aniversariantes entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public long Update(Aniversariantes entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public int Delete(Aniversariantes entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public int Delete(long id, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public Aniversariantes GetById(long id, long usuarioID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Aniversariantes> GetAll(long usuarioID)
        {
            throw new NotImplementedException();
        }

        public EventosFeriados RelatorioEventos(long congregacaoId, int mes, int ano, Domain.Entities.Evento.TipoEvento tipoEvento)
        {
            return _relatorioService.RelatorioEventos(congregacaoId, mes, ano, tipoEvento);
        }

        public List<PresencaLista> RelatorioPresencaLista(int idPresenca, int idData, long usuarioId = 0)
        {
            return _relatorioService.RelatorioPresencaLista(idPresenca, idData, usuarioId);
        }

        public List<PresencaLista> RelatorioInscricoes(int idPresenca, int idCongregacao, int tipo, long usuarioID = 0)
        {
            return _relatorioService.RelatorioInscricoes(idPresenca, idCongregacao, tipo, usuarioID);
        }

        public RelatorioFrequencia RelatorioFrequencia(int congregacaoId, DateTime dataInicial, DateTime dataFinal, string cargos, string tipoEvento)
        {
            return _relatorioService.RelatorioFrequencia(congregacaoId, dataInicial, dataFinal, cargos, tipoEvento);
        }

        public IEnumerable<Membro> MembrosGrid(bool gridBatismo, List<string> congregacoes, List<string> cargos, int idBatismo, bool imprimirObrVinc)
        {
            return _relatorioService.MembrosGrid(gridBatismo, congregacoes, cargos, idBatismo, imprimirObrVinc);
        }

    }
}
