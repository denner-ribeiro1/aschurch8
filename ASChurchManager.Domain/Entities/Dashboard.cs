using ASChurchManager.Domain.Types;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace ASChurchManager.Domain.Entities
{
    [Serializable]
    public class Dashboard
    {
        public List<QtdSitMembro> SituacaoMembro { get; set; }
        public int QuantidadeCongregados { get; set; }
        public int QuantidadeCartasPendentes { get; set; }
    }

    public class QtdSitMembro
    {
        public Status Status { get; set; }
        public int Quantidade { get; set; }
    }
}
