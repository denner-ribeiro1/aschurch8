using System;

namespace ASChurchManager.Domain.Entities.Relatorios.API.In
{
    public class InCasamentos
    {
        public int CongregacaoId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFinal { get; set; }
        public string TipoSaida { get; set; }
    }
}

