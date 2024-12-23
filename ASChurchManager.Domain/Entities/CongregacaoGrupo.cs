using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.Domain.Entities
{
    public class CongregacaoGrupo : BaseEntity
    {
        public int GrupoId { get; set; }

        public string Grupo { get; set; }

        public string NomeGrupo { get; set; }
                
        public long ResponsavelId { get; set; }

        public string ResponsavelNome { get; set; }

        public long CongregacaoId { get; set; }
    }
}
