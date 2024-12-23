using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Nascimento
{
    public class IndexNascVM
    {
        public IndexNascVM()
        {
            Filtros = new SelectList(new List<SelectListItem>() {
                new SelectListItem() { Text = "Nome da Criança", Value = "Crianca" },
                new SelectListItem() { Text = "Nome do Pai", Value = "NomePai" },
                new SelectListItem() { Text = "Nome da Mãe", Value = "NomeMae" }
            }, "Value", "Text");

        }

        public SelectList Filtros { get; set; }

        [Display(Name = "Filtrar por:")]
        public string Filtro { get; set; }

        [Display(Name = "Conteúdo:")]
        public string Conteudo { get; set; }
    }
}