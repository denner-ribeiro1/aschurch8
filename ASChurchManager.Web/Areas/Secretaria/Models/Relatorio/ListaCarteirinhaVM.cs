using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Relatorio
{
    public class ListaCarteirinhaVM
    {
        public bool Imprimir {  get; set; }

        public int Id { get; set; }

        public string Nome { get; set; }

        public string Cpf { get; set; }

        public string Congregacao { get; set; }
    }
}
