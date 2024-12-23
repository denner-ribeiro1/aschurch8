using ASChurchManager.Domain.Types;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Entities
{
    public class Perfil : BaseEntity
    {
        public Perfil()
        {
            this.Rotinas = new List<Rotina>();
        }

        public string Nome { get; set; }
        public TipoPerfil TipoPerfil { get; set; }
        public bool Status { get; set; }
        public List<Rotina> Rotinas { get; set; }
    }
}
