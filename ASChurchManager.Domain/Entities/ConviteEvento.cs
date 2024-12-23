using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.Domain.Entities
{
    public class ConviteEvento : BaseEntity
    {
        public long EventoId;

        public long CongregacaoId;

        public long GrupoId;

        public long ConvidadoId;
    }
}
