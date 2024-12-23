using ASChurchManager.Web.Lib;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Presenca
{
    public class IndexFrequenciaVM
    {
        public IndexFrequenciaVM()
        {
            ListaPresencaAberto = new List<SelectListItem>();
            ListaData = new List<SelectListItem>();

            Filtros = new SelectList(new List<SelectListItem>() {
                new SelectListItem() { Text = "Inscr. Nr.", Value = "Id" },
                new SelectListItem() { Text = "RM", Value = "MembroId" },
                new SelectListItem() { Text = "Nome", Value = "Nome" },
                new SelectListItem() { Text = "CPF", Value = "CPF" },
                new SelectListItem() { Text = "Congregação", Value = "Igreja" },
            }, "Value", "Text");
        }

        [Display(Name = "Cursos/Eventos")]
        [Required(ErrorMessage = "Cursos/Eventos é de preenchimento obrigatório")]
        public long PresencaId { get; set; }

        public string Descricao { get; set; }
        public List<SelectListItem> ListaPresencaAberto { get; set; }

        public List<Domain.Entities.Presenca> ListaPresencaAndamento { get; set; }

        [Display(Name = "Data/Hora Inicio")]
        [Required(ErrorMessage = "Data/Hora é de preenchimento obrigatório")]
        public int DataId { get; set; }

        public List<SelectListItem> ListaData { get; set; }

        [Display(Name = "Finalizar a Data?")]
        public bool Finalizar { get; set; } = true;

        public string Status { get; set; }

        public SelectList Filtros { get; set; }

        [Display(Name = "Filtrar por:")]
        public string Filtro { get; set; }

        [Display(Name = "Conteúdo:")]
        public string Conteudo { get; set; }

        [Cpf(ErrorMessage = "CPF Inválido")]
        public string CpfFiltro { get; set; }

        public List<SelectListItem> ListaCongregacoes { get; set; }

        public int CongregacaoSelecionado { get; set; }
    }
}
