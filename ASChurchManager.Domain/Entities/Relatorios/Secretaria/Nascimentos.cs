using System;

namespace ASChurchManager.Domain.Entities.Relatorios.Secretaria
{
    public class Nascimentos
    {
        public string CongregacaoNome { get; set; }

        public string Crianca { get; set; }

        public string NomeMae { get; set; }

        public string NomePai { get; set; }

        public string Pastor { get; set; }

        public DateTimeOffset DataApresentacao { get; set; }

        public DateTimeOffset DataNascimento { get; set; }

        public string Sexo { get; set; }
    }
}
