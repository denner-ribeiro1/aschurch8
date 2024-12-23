using ASChurchManager.Domain.Types;

namespace ASChurchManager.Domain.Entities
{
    public class PresencaMembro : BaseEntity
    {
        public int PresencaId { get; set; }

        public int MembroId { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }

        public int CongregacaoId{ get; set; }

        public string Igreja { get; set; }

        public string Cargo { get; set; }

        public bool Pago { get; set; }

        public bool OK { get; set; }

        public int Usuario { get; set; }

        public string NomeArquivo { get; set; }

        public int ArquivoId { get; set; }

        public SituacaoPresenca Situacao { get; set; }

        public TipoRegistro Tipo { get; set; }

        public string Justificativa { get; set; }

        public StatusPresenca StatusData { get; set; }
    }
}
