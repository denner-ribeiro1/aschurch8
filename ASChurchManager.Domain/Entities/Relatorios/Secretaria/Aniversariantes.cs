using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.Domain.Entities.Relatorios.Secretaria
{
    public class Aniversariantes
    {
        public string Congregacao { get; set; }

        public string Nome { get; set; }

        public DateTimeOffset DataNascimento { get; set; }
    }
}
