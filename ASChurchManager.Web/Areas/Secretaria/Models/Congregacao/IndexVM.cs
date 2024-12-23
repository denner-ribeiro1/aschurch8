using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Congregacao
{
    public class IndexVM
    {
        public IndexVM()
        {
            Filtros = new SelectList(new List<SelectListItem>() {
                new SelectListItem() { Text = "Código", Value = "Id" },
                new SelectListItem() { Text = "Nome", Value = "Nome" },
                new SelectListItem() { Text = "Pastor Responsável", Value = "Pastor" },
            }, "Value", "Text");
        }

        public SelectList Filtros { get; set; }

        [Display(Name = "Filtrar por:")]
        public string Filtro { get; set; }

        [Display(Name = "Conteúdo:")]
        public string Conteudo { get; set; }
    }
}
