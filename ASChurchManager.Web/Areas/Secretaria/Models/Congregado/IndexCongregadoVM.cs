using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Congregado
{
    public class IndexCongregadoVM
    {
        public IndexCongregadoVM()
        {
            Filtros = new SelectList(new List<SelectListItem>() {
                new SelectListItem() { Text = "Código", Value = "Id" },
                new SelectListItem() { Text = "Nome", Value = "Nome" },
                new SelectListItem() { Text = "Congregação", Value = "CongregacaoId" },
                new SelectListItem() { Text = "Status", Value = "Status" }
            }, "Value", "Text");

            Status = new SelectList(new List<SelectListItem>() {
                new SelectListItem() { Text = "Ativo", Value = "1" },
                new SelectListItem() { Text = "Inativo", Value = "2" }
            }, "Value", "Text");
        }

        public SelectList Filtros { get; set; }

        [Display(Name = "Filtrar por:")]
        public string Filtro { get; set; }

        [Display(Name = "Conteúdo:")]
        public string Conteudo { get; set; }

        public List<SelectListItem> ListaCongregacoes { get; set; }

        public int CongregacaoSelecionado { get; set; }

        public bool EnviarBatismo { get; set; }

        public int StatusSelecionado { get; set; }
        
        public SelectList Status { get; set; }
    }
}