using System;

namespace ASChurchManager.Domain.Entities.Relatorios.Secretaria
{
    public class Obreiros
    {
        public string Descricao { get; set; }
        public string NomeMembro { get; set; }
        public string CongregacaoNome { get; set; }
        public long CongregacaoId { get; set; }
        public DateTimeOffset DataNascimento { get; set; }
        public string PastorResponsavel { get; set; }
    }
}
