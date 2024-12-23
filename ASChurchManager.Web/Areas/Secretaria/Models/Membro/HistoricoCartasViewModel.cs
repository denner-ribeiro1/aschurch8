using ASChurchManager.Web.ViewModels.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro
{
    public class HistoricoCartasViewModel : EntityViewModelBase
    {
        [Display(Name = "Congregação de Origem")]
        public string CongregacaoOrigem { get; set; }
        [Display(Name = "Congregação de Destino")]
        public string CongregacaoDestino { get; set; }

        [Display(Name = "Data de Transferência"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset DataDaTransferencia { get; set; }
    }
}