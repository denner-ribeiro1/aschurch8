using ASChurchManager.Domain.Types;
using System;

namespace ASChurchManager.Domain.Entities.Relatorios.Secretaria
{
    public class PresencaLista
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public int MembroId { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public int CongregacaoId { get; set; }
        public string Igreja { get; set; }
        public string Cargo { get; set; }
        public SituacaoPresenca Situacao { get; set; }
        public bool Pago { get; set; }
        public float Valor { get; set; }
        public string Justificativa { get; set; }
    }
}
