using ASChurchManager.Domain.Types;
using System;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Entities
{
    public class Batismo : BaseEntity
    {
        public Batismo()
        {
            Celebrantes = new List<Membro>();
        }
        public DateTimeOffset DataMaximaCadastro { get; set; }
        public DateTimeOffset DataBatismo { get; set; }
        public StatusBatismo Status { get; set; }
        public List<Membro> Celebrantes { get; set; }
        public int IdadeMinima { get; set; }
    }


    public class BatismoMembro : BaseEntity
    {
        public long MembroId { get; set; }
        public SituacaoCandidatoBatismo SituacaoCandBatismo { get; set; }

    }
}
