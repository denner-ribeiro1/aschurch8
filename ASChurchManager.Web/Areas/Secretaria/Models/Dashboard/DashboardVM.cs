using System.Collections.Generic;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Dashboard
{
    public class DashboardVM
    {
        public DashboardVM()
        {
            SituacaoMembro = new List<DashboardItemVM>();
        }
        public List<DashboardItemVM> SituacaoMembro { get; set; }
        public int QuantidadeCongregados { get; set; }
        public int QuantidadeCartasPendentes { get; set; }
        public int MembrosPendentes { get; set; }
        public int MembrosReprovados { get; set; }
    }

    public class DashboardItemVM
    {
        public int Quantidade { get; set; }
        public int Situacao { get; set; }

    }
}