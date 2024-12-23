using ASChurchManager.Web.ViewModels.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro
{
    public class GridHistoricoCartasViewModel : EntityViewModelBase
    {
        [Display(Name = "Congregação Origem")]
        public string CongregacaoOrigem { get; set; }

        [Display(Name = "Congregação Destino")]
        public string CongregacaoDestino { get; set; }

        [Display(Name = "Data da Transferência"),
         DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset DataDaTransferencia { get; set; }
    }
}