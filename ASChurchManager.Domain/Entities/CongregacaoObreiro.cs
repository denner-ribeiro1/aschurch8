namespace ASChurchManager.Domain.Entities
{
    public class CongregacaoObreiro
    {
        public long CongregacaoId { get; set; }
        public string CongregacaoNome { get; set; }
        public long ObreiroId { get; set; }
        public string ObreiroNome { get; set; }
        public string ObreiroCargo { get; set; }
    }
}