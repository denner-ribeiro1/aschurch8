using ASChurchManager.Domain.Types;
using System;

namespace ASChurchManager.Domain.Entities.Relatorios.API.In
{
    public class InCandidatosBatismo
    {
        public long CongregacaoId { get; set; }
        public long BatismoId { get; set; }
        public DateTime DataBatismo { get; set; }
        public SituacaoCandidatoBatismo Situacao { get; set; }
        public string TipoSaida { get; set; }
    }
}
