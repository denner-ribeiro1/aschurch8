using ASChurchManager.Web.Areas.Secretaria.ViewModels.PastorCelebrante;
using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Batismo
{
    public class ListaBatismoVM : EntityViewModelBase
    {
        public ListaBatismoVM()
        {
            Celebrantes = new List<GridPastorCelebranteViewModel>();
            Candidatos = new List<CandidatosBatismoVM>();
            ListaDatasBatismo = new List<SelectListItem>();
        }
        [Display(Name = "Selecione a Data para o Batismo:")]
        public DateTime DataBatismo { get; set; }
        public IEnumerable<GridPastorCelebranteViewModel> Celebrantes { get; set; }
        public IEnumerable<CandidatosBatismoVM> Candidatos { get; set; }
        public List<SelectListItem> ListaDatasBatismo { get; set; }
    }
}