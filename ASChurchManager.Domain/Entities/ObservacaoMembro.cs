using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.Domain.Entities
{
    public class ObservacaoMembro
    {
        public ObservacaoMembro()
        {
            Usuario = new Usuario();
        }

        public long Id { get; set; }
        public long MembroId { get; set; }
        public string Observacao { get; set; }
        public Usuario Usuario { get; set; }
        public DateTimeOffset DataCadastro { get; set; }
    }
}
