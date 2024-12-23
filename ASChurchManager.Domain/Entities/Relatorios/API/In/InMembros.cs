using ASChurchManager.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.Domain.Entities.Relatorios.API.In
{
    public class InMembros
    {
        public int CongregacaoId { get; set; }
        public Types.Status Status { get; set; }
        public TipoMembro TipoMembro { get; set; }
        public EstadoCivil EstadoCivil { get; set; }
        public bool Completo { get; set; }
        public bool ABEDABE { get; set; }
        public string TipoSaida { get; set; }
    }
}
