using ASChurchManager.Domain.Entities.Relatorios.Secretaria;
using ASChurchManager.Domain.Types;
using System;
using System.Collections.Generic;
using static ASChurchManager.Domain.Entities.Evento;

namespace ASChurchManager.Domain.Intefaces.Repository
{
    public interface IRelatoriosSecretariaRepository : IRepositoryDAO<Aniversariantes>
    {
        IEnumerable<Aniversariantes> Aniversariantes(DateTimeOffset dataInicio, DateTimeOffset dataFinal, long congregacaoId, long usuarioID, TipoMembro tipoMembro);
        IEnumerable<Transferencia> Transferencia(DateTimeOffset dataInicio, DateTimeOffset dataFinal, long congregacaoId, long usuarioID);
        IEnumerable<Batismo> CandidatosBatismo(long batismoId, DateTimeOffset dataBatismo, long congregacaoId, SituacaoCandidatoBatismo situacao, long usuarioID);
        IEnumerable<Nascimentos> Nascimento(DateTimeOffset dataInicio, DateTimeOffset dataFinal, long congregacaoId, long usuarioID);
        IEnumerable<Casamento> Casamento(DateTimeOffset dataInicio, DateTimeOffset dataFinal, long congregacaoId, long usuarioID);
        IEnumerable<Congregacoes> Congregacao(long congregacaoId, long usuarioID);
        IEnumerable<Obreiros> Obreiros(long congregacaoId, long usuarioID);
        IEnumerable<RelatorioMensal> RelatorioMensal(string mes, int ano, long congregacaoId, long usuarioID);
        IEnumerable<RelatorioMembros> RelatorioMembros(long congrecacaoId, Status status, TipoMembro tipoMembro, EstadoCivil estadoCivil, bool abedabe, bool filtrarConf, bool ativosConf, long usuarioId);
        IEnumerable<CursoMembro> CursosMembro(long congregacaoId, int cursoId, long usuarioID);
        EventosFeriados RelatorioEventos(long congregacaoId, int mes, int ano, TipoEvento tipoEvento);
        List<PresencaLista> RelatorioPresencaLista(int idPresenca, int idData, long usuarioId = 0);
        List<PresencaLista> RelatorioInscricoes(int idPresenca, int idCongregacao, int tipo, long usuarioID = 0);
        RelatorioFrequencia RelatorioFrequencia(int congregacaoId, DateTime dataInicial, DateTime dataFinal, string cargos, string tipoEvento);
        IEnumerable<Membro> MembrosGrid(bool gridBatismo, List<string> congregacoes, List<string> cargos, int idBatismo, bool imprimirObrVinc);
    }
}
