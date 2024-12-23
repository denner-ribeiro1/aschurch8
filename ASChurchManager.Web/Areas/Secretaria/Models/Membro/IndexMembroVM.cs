using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro
{
    public class IndexMembroVM
    {
        public IndexMembroVM()
        {
            Filtros = new SelectList(new List<SelectListItem>() {
                new SelectListItem() { Text = "Código", Value = "Id" },
                new SelectListItem() { Text = "Nome", Value = "Nome" },
                new SelectListItem() { Text = "CPF", Value = "Cpf" },
                new SelectListItem() { Text = "Nome da Mãe", Value = "NomeMae" },
                new SelectListItem() { Text = "Congregação", Value = "CongregacaoId" },
                new SelectListItem() { Text = "Status", Value = "Status" },
            }, "Value", "Text");

        }

        public SelectList Filtros { get; set; }

        [Display(Name = "Filtrar por:")]
        public string Filtro { get; set; }

        [Display(Name = "Conteúdo:")]
        public string Conteudo { get; set; }

        public string Cpf { get; set; }

        public List<SelectListItem> ListaStatusMembro { get; set; }

        public int StatusMembroSelecionado { get; set; }

        public List<SelectListItem> ListaCongregacoes { get; set; }

        public int CongregacaoSelecionado { get; set; }
    }
}