using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.Domain.Entities.Relatorios.API.Out
{
    public class OutRelatorio
    {
        public byte[] Relatorio { get; set; }
        public string MimeType { get; set; }
        public List<string> Erros { get; set; }

        public OutRelatorio()
        {
            Erros = new List<string>();
        }
    }
}
