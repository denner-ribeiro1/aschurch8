using ASChurchManager.Domain.Types;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ASChurchManager.Domain.Entities.Evento;

namespace ASChurchManager.Web.ViewModels.Shared
{
    public class RelatorioViewModel
    {
        [Display(Name = "Congregação"),
         Required(ErrorMessage = "Selecione uma Congregação")]
        public IEnumerable<SelectListItem> SelectCongregacoes { get; set; }

        [Display(Name = "Congregação")]
        public int Congregacao { get; set; }

        [Display(Name = "Data Inicio"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true),
        Required(ErrorMessage = "Data Inicio é de preenchimento obrigatório")]
        public DateTimeOffset? DataInicio { get; set; }

        [Display(Name = "Data Final"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true),
        Required(ErrorMessage = "Data Final é de preenchimento obrigatório")]
        public DateTimeOffset? DataFinal { get; set; }

        [Display(Name = "Mês")]
        public Meses Mes { get; set; }

        [Display(Name = "Ano"),
         Range(1950, 2050, ErrorMessage = "Ano deve ser maior que 1950 e menor de 2050"),
         Required(ErrorMessage = "Ano é de preenchimento obrigatório")]
        public int Ano { get; set; }

        public int IdMembro { get; set; }

        [Display(Name = "Tipo")]
        public TipoMembro TipoMembro { get; set; }

        public List<SelectListItem> SelectTipoMembro { get; set; }

        [Display(Name = "Tipo de Evento")]
        public TipoEvento TipoEvento { get; set; }

        public List<SelectListItem> SelectTipoEvento { get; set; }

        [Display(Name = "Imprimir a Versão Simplificada")]
        public bool Completo { get; set; }

        [Display(Name = "Formato")]
        public Saida TipoSaida { get; set; }

        public List<SelectListItem> SelectTipoSaida { get; set; }
    }

    public enum Saida
    {
        [Display(Name = "PDF")]
        PDF,
        [Display(Name = "Excel")]
        Excel
    }
}
