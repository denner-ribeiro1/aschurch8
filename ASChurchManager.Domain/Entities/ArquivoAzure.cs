using System;

namespace ASChurchManager.Domain.Entities
{
    public class ArquivoAzure
    {
        public string Nome { get; set; }

        public double Tamanho { get; set; }

        public string Tipo { get; set; }

        public DateTime UltimaModificacao { get; set; }
    }
}
