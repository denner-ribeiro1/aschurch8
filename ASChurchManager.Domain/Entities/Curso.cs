using System;

namespace ASChurchManager.Domain.Entities
{
    public class Curso : BaseEntity
    {
        public string Descricao { get; set; }
        public DateTimeOffset? DataInicio { get; set; }
        public DateTimeOffset? DataEncerramento { get; set; }
        public int CargaHoraria { get; set; }
    }
}
