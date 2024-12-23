using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Eventos
{
    public class EventoVM : EntityViewModelBase
    {
        public EventoVM()
        {
            this.SelectTipoPesquisa = new List<SelectListItem>();
        }

        [Display(Name = "Congregação")]
        public int TipoPesquisaSelecionada { get; set; }
        public List<SelectListItem> SelectTipoPesquisa { get; set; }
    }
}