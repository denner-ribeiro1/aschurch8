using ASChurchManager.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.Domain.Entities.Relatorios.API.In
{
    public class InAniversario
    {
        public int CongregacaoId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFinal { get; set; }
        public string TipoSaida { get; set; }
        public TipoMembro TipoMembro { get; set; }
    }
}

