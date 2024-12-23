using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Carta
{
    public class IndexCartaVM
    {
        public IndexCartaVM()
        {
            Filtros = new SelectList(new List<SelectListItem>() {
                new SelectListItem() { Text = "Código", Value = "Id" },
                new SelectListItem() { Text = "Nome", Value = "Nome" },
                new SelectListItem() { Text = "Congregação Origem", Value = "CongregacaoOrigem" },
                new SelectListItem() { Text = "Congregação Destino", Value = "CongregacaoDestino" },
                new SelectListItem() { Text = "Tipo/Status", Value = "TipoCarta_Status" }
            }, "Value", "Text");
            ListaCongregacoes = new List<SelectListItem>();
        }

        public SelectList Filtros { get; set; }

        [Display(Name = "Filtrar por:")]
        public string Filtro { get; set; }

        [Display(Name = "Conteúdo:")]
        public string Conteudo { get; set; }
        public int StatusCartaSelecionado { get; set; }
        public int TipoCartaSelecionado { get; set; }
        public List<SelectListItem> ListaCongregacoes { get; set; }
        public int CongregacaoSelecionado { get; set; }
    }
}