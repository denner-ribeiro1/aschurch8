using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Batismo
{
    public class IndexBatismoVM
    {
        public IndexBatismoVM()
        {
            Filtros = new List<SelectListItem>() {
                new SelectListItem() { Text = "Código", Value = "Id" },
                new SelectListItem() { Text = "Nome", Value = "Nome" },
                new SelectListItem() { Text = "CPF", Value = "Cpf" },
                new SelectListItem() { Text = "Nome da Mãe", Value = "NomeMae" },
                new SelectListItem() { Text = "Congregação", Value = "CongregacaoId" }
            };

            ListaCongregacoes = new List<SelectListItem>();
            ListaDatasBatismo = new List<SelectListItem>();
        }

        public List<SelectListItem> Filtros { get; set; }

        [Display(Name = "Filtrar por:")]
        public string Filtro { get; set; }

        [Display(Name = "Conteúdo:")]
        public string Conteudo { get; set; }

        public string Cpf { get; set; }

        public List<SelectListItem> ListaCongregacoes { get; set; }

        public int CongregacaoSelecionado { get; set; }

        public List<SelectListItem> ListaDatasBatismo { get; set; }

        public DateTime? DataBatismoSelecionado { get; set; }
    }
}