using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Casamento
{
    public class IndexCasamentoVM
    {
        public IndexCasamentoVM()
        {
            Filtros = new SelectList(new List<SelectListItem>() {
                new SelectListItem() { Text = "Código", Value = "Id" },
                new SelectListItem() { Text = "Congregação", Value = "CongregacaoNome" },
                new SelectListItem() { Text = "Noivo", Value = "NoivoNome" },
                new SelectListItem() { Text = "Noiva", Value = "NoivaNome" },
            }, "Value", "Text");
            ListaCongregacoes = new List<SelectListItem>();
        }

        public SelectList Filtros { get; set; }

        [Display(Name = "Filtrar por:")]
        public string Filtro { get; set; }

        [Display(Name = "Conteúdo:")]
        public string Conteudo { get; set; }

        public List<SelectListItem> ListaCongregacoes { get; set; }

        public int CongregacaoSelecionado { get; set; }

        public bool EnviarBatismo { get; set; }
    }
}