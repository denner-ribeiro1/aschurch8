using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Relatorio
{
    public class CandidatosBatismoViewModel : RelatorioViewModel
    {
        [Display(Name = "Datas de Batismo")]
        public IEnumerable<SelectListItem> SelectDatasBatismo { get; set; }

        [Display(Name = "Datas de Batismo")]
        public string DataBatismo { get; set; }

        [Display(Name = "Situação")]
        public IEnumerable<SelectListItem> SelectSituacao { get; set; }

        [Display(Name = "Situação")]
        public string Situacao { get; set; }
    }
}