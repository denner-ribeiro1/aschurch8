using System;

namespace ASChurchManager.Domain.Entities
{
    public class Casamento : BaseEntity
    {
        public Casamento()
        {
            Congregacao = new Congregacao();
        }

        public long CongregacaoId { get; set; }
        public Congregacao Congregacao { get; set; }
        public long PastorId { get; set; }
        public string PastorNome { get; set; }
        public DateTimeOffset DataHoraInicio { get; set; }
        public DateTimeOffset DataHoraFinal { get; set; }
        public long NoivoId { get; set; }
        public string NoivoNome { get; set; }
        public long PaiNoivoId { get; set; }
        public string PaiNoivoNome { get; set; }
        public long MaeNoivoId { get; set; }
        public string MaeNoivoNome { get; set; }
        public long NoivaId { get; set; }
        public string NoivaNome { get; set; }
        public long PaiNoivaId { get; set; }
        public string PaiNoivaNome { get; set; }
        public long MaeNoivaId { get; set; }
        public string MaeNoivaNome { get; set; }
    }
}
