using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Congregacao
{
    public class GrupoVM
    {
        public bool IsReadOnly { get; set; }

        [Display(Name = "Grupo")]
        public long? GrupoId { get; set; }

        public string Grupo { get; set; }

        [Display(Name = "Nome do Grupo")]
        public string NomeGrupo { get; set; }

        [Display(Name = "Grupo")]
        public IEnumerable<SelectListItem> SelectGrupos { get; set; }
        
        [Display(Name = "Cód. Resp.")]
        public long? ResponsavelId { get; set; }

        [Display(Name = "Responsável")]
        public string ResponsavelNome { get; set; }
    }
}