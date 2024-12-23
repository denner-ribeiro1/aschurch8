using ASChurchManager.Domain.Types;
using System;
namespace ASChurchManager.Domain.Entities
{
    public class Cargo : BaseEntity
    {
        public string Descricao { get; set; }
        public bool Obreiro { get; set; }
        public bool Lider { get; set; }
        public DateTimeOffset? DataCargo { get; set; }
        public TipoCarteirinha TipoCarteirinha { get; set; }
        public string LocalConsagracao { get; set; }
        public bool Confradesp { get; set; }
        public bool CGADB { get; set; }
    }
}