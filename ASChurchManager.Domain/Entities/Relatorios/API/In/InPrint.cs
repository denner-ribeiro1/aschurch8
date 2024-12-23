using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.Domain.Entities.Relatorios.API.In
{
    public class InPrint
    {
        public long TemplateId { get; set; }
        public string TargetModel { get; set; }
        public long ModelId { get; set; }
    }
}
