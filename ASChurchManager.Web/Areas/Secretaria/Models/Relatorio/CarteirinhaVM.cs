using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Relatorio
{
    public class CarteirinhaVM : EntityViewModelBase
    {
        public CarteirinhaVM()
        {
            ListaCargos = new List<SelectListItem>();
            ListaDatasBatismo = new List<SelectListItem>();
            ListaCongregacoesMembro = new List<SelectListItem>();
            ListaCongregacoesBatismo = new List<SelectListItem>();
        }

        [Display(Name = "Congregação"),
        Required(ErrorMessage = "Selecione uma Congregação")]
        public IEnumerable<SelectListItem> ListaCongregacoesMembro { get; set; }

        [Display(Name = "Congregação")]
        public int CongregacaoMembro { get; set; }

        [Display(Name = "Congregação"),
        Required(ErrorMessage = "Selecione uma Congregação")]
        public IEnumerable<SelectListItem> ListaCongregacoesBatismo { get; set; }

        [Display(Name = "Congregação")]
        public int CongregacaoBatismo { get; set; }

        [Display(Name = "Datas de Batismo")]
        public List<SelectListItem> ListaDatasBatismo { get; set; }

        [Display(Name = "Datas de Batismo")]
        public string DataBatismo { get; set; }

        [Display(Name = "Cargos")]
        public string Cargos { get; set; }

        public List<SelectListItem> ListaCargos { get; set; }

        [Display(Name = "Filtro")]
        public string Filtro { get; set; }


    }

    public class CarteirinhaLoteVM
    {
        public CarteirinhaLoteVM()
        {
            Membros = new List<int>();
        }
       
        public bool AtualizaDtValidade { get; set; }

        public List<int> Membros { get; set; }


    }
}
