using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Relatorio
{
    public class PresencaListaVM : EntityViewModelBase
    {
        public PresencaListaVM()
        {
            ViewModelTitle = "PresencaLista";
            ListaCongregacoes = new List<SelectListItem>();
            ListaTipoEventos = new List<SelectListItem>();
            ListaCargos = new List<SelectListItem>();
        }

        [Display(Name = "Congregação")]
        public int CongregacaoId { get; set; }

        public List<SelectListItem> ListaCongregacoes { get; set; }

        public List<SelectListItem> ListaTipoEventos { get; set; }

        public List<SelectListItem> ListaCargos { get; set; }


        [Display(Name = "Data Inicio"),
        Required(ErrorMessage = "Data Inicio é de preenchimento obrigatório"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataInicio{ get; set; }

        [Display(Name = "Data Final"),
        Required(ErrorMessage = "Data Final é de preenchimento obrigatório"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataFinal { get; set; }

    }
}
