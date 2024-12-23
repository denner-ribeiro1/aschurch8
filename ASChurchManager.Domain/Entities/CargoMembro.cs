using ASChurchManager.Domain.Types;
using System;
namespace ASChurchManager.Domain.Entities
{
    public class CargoMembro : BaseEntity
    {
        public int CargoId { get; set; }
        public string Descricao { get; set; }
        public DateTimeOffset? DataCargo { get; set; }
        public TipoCarteirinha TipoCarteirinha { get; set; }
        public string LocalConsagracao { get; set; }
        public string Confradesp { get; set; }
        public string CGADB { get; set; }
    }
}