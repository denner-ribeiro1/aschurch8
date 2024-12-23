using ASChurchManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Presenca
{
    public class PresencaGridVM
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string TipoEvento { get; set; }
        public string Congregacao { get; set; }
        public string Status { get; set; }
        public string DataMaxima{ get; set; }
        public string DataHoraInicio { get; set; }

    }
}
