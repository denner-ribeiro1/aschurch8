using ASChurchManager.Domain.Types;
using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Relatorio
{
    public class PresencaVM
    {
        public PresencaVM()
        {
            ListaPresenca = new List<SelectListItem>();
            var lista = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Todos", Value = "" },
                new SelectListItem() { Text = "Membros", Value = "Membro" },
                new SelectListItem() { Text = "Não Membros", Value = "NaoMembro" }
            };
            ListaTipoInscricao = new SelectList(lista, "Value", "Text");

            ListaCongregacoes = new List<SelectListItem>();

            IEnumerable<SituacaoPresenca> lStatus = Enum.GetValues(typeof(SituacaoPresenca))
                                                      .Cast<SituacaoPresenca>();
            var selectlst = (from item in lStatus
                             select new SelectListItem
                             {
                                 Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "Todos" : item.GetDisplayAttributeValue(),
                                 Value = ((int)item).ToString()
                             }).ToList();

            ListaSituacao = selectlst.ToList();

            ListaData = new List<SelectListItem>();
        }

        [Display(Name = "Cursos/Eventos")]
        public long PresencaId { get; set; }

        public List<SelectListItem> ListaPresenca { get; set; }

        [Display(Name = "Congregação")]
        public int CongregacaoId { get; set; }

        public List<SelectListItem> ListaCongregacoes { get; set; }

        [Display(Name = "Tipo de Inscrição")]
        public string TipoInscricao { get; set; }

        public SelectList ListaTipoInscricao { get; set; }

        [Display(Name = "Datas")]
        public int DataId { get; set; }

        public List<SelectListItem> ListaData { get; set; }

        [Display(Name = "Situação")]
        public int SituacaoSelecionado { get; set; }

        public List<SelectListItem> ListaSituacao { get; set; }

        [Display(Name = "Imprimir espaços para carimbos da Tesouraria/Secretaria?")]
        public bool Carimbo { get; set; }

        [Display(Name = "Formato")]
        public Saida TipoSaida { get; set; }

        public List<SelectListItem> SelectTipoSaida { get; set; }
    }
}
