using ASChurchManager.Domain.Types;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro
{
    public class SituacaoVM
    {
        public bool IsReadOnly { get; set; }

        [Display(Name = "Situação")]
        public MembroSituacao? IdSit { get; set; }

        public string DescricaoSit { get; set; }

        [Display(Name = "Data Situação")]
        public string DataSit { get; set; }

        [Display(Name = "Observação")]
        public string ObservacaoSit { get; set; }

        public IEnumerable<SelectListItem> SelectMembroSituacao { get; set; }
    }
}