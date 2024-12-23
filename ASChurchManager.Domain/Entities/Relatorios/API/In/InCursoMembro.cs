using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.Domain.Entities.Relatorios.API.In
{
    public class InCursoMembro
    {
        public int CongregacaoId { get; set; }
        public int CursoId { get; set; }
        public string TipoSaida { get; set; }
    }
}
