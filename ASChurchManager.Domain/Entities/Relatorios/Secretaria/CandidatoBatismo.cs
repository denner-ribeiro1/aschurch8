using ASChurchManager.Domain.Types;
using System;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Entities.Relatorios.Secretaria
{
    public class Batismo
    {
        public Batismo()
        {
            PastorCelebrante = new List<Pastor>();
            Candidatos = new List<CandidatoBatismo>();
            PastorPresidente = new Pastor();
        }

        public int Id { get; set; }
        public DateTimeOffset DataBatismo { get; set; }
        public StatusBatismo Status { get; set; }
        public List<CandidatoBatismo> Candidatos { get; set; }
        public List<Pastor> PastorCelebrante { get; set; }
        public Pastor PastorPresidente { get; set; }

    }

    public class CandidatoBatismo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTimeOffset DataNascimento { get; set; }
        public int CongregacaoId { get; set; }
        public string CongregacaoNome { get; set; }
        public string Situacao { get; set; }
        public Tamanho TamanhoCapa { get; set; }
        public List<ObservacaoMembro> Observacoes { get; set; }
    }

    public class Pastor
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }
}
