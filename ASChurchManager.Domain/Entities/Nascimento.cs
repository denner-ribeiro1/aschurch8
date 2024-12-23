using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASChurchManager.Domain.Types;

namespace ASChurchManager.Domain.Entities
{
    public class Nascimento : BaseEntity
    {
        public Nascimento()
        {
            congregacao = new Congregacao();
        }

        public Congregacao congregacao { get; set; }
        public DateTimeOffset DataApresentacao { get; set; }
        public DateTimeOffset DataNascimento { get; set; }
        public string NomePai { get; set; }
        public string NomeMae { get; set; }
        public string Crianca { get; set; }
        public long CongregacaoId { get; set; }
        public string CongregacaoNome { get; set; }
        public Sexo Sexo { get; set; }
        public long IdMembroPai { get; set; }
        public long IdMembroMae { get; set; }
        public string Pastor { get; set; }
        public long PastorId { get; set; }
    }
}
