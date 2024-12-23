using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Relatorio
{
    public class CursoMembroVM : RelatorioViewModel
    {
        [Display(Name = "Cursos")]
        public List<SelectListItem> SelectCursos { get; set; }

        [Display(Name = "Cursos")]
        public string Cursos { get; set; }
    }
}