using System;

namespace ASChurchManager.Domain.Entities
{
    [Serializable]
    public class Feriado
    {
        public DateTime DataFeriado { get; set; }
        public string Descricao { get; set; }
    }
}
