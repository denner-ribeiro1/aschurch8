using ASChurchManager.Domain.Types;
using System;

namespace ASChurchManager.Application.Interfaces
{
    public interface IRelatorioAPISecretariaAppService
    {
        bool RelatorioAniversariantes(int congregacaoId, DateTime dataInicio, DateTime dataFinal, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType, TipoMembro tipoMembro);
        bool RelatorioCandidatosBatismo(int congregacaoId, long batismoId, DateTime dataBatismo, SituacaoCandidatoBatismo situacao, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType);
        bool RelatorioTransferencia(int congregacaoId, DateTime dataInicio, DateTime dataFinal, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType);
        bool RelatorioNascimentos(int congregacaoId, DateTime dataInicio, DateTime dataFinal, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType);
        bool RelatorioCongregacoes(int congregacaoId, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType);
        bool RelatorioObreiros(int congregacaoId, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType);
        bool RelatorioMensal(int congregacaoId, string mes, int ano, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType);
        bool RelatorioMembros(int congregacaoId, Status status, TipoMembro tipoMembro, EstadoCivil estadoCivil, bool completo, bool abedabe, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType);
        bool RelatorioCursosMembro(int congregacaoId, int cursoId, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType);
        bool RelatorioCasamentos(int congregacaoId, DateTime dataInicio, DateTime dataFinal, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType);
    }
}
