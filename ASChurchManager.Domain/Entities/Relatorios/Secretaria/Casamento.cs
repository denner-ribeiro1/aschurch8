using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.Domain.Entities.Relatorios.Secretaria
{
    public class Casamento
    {
        public string CongregacaoNome { get; set; }
        public string PastorNome { get; set; }
        public string NoivoNome { get; set; }
        public string PaiNoivoNome { get; set; }
        public string MaeNoivoNome { get; set; }
        public string NoivaNome { get; set; }
        public string PaiNoivaNome { get; set; }
        public string MaeNoivaNome { get; set; }
        public DateTimeOffset Data { get; set; }
        public string DataHora { get; set; }
    }
}
