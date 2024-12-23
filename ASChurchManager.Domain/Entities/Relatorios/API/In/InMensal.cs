using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.Domain.Entities.Relatorios.API.In
{
    public class InMensal
    {
        public int CongregacaoId { get; set; }
        public string Mes { get; set; }
        public int Ano { get; set; }
        public string TipoSaida { get; set; }
    }
}
