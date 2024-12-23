﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Presenca
{
    public class IndexConfigVM
    {
        public IndexConfigVM()
        {
            Filtros = new SelectList(new List<SelectListItem>() {
                new SelectListItem() { Text = "Código", Value = "Id" },
                new SelectListItem() { Text = "Descrição", Value = "Descricao" },
                new SelectListItem() { Text = "Tipo Evento", Value = "TipoEventoId" },
                new SelectListItem() { Text = "Congregação", Value = "CongregacaoId" },
                new SelectListItem() { Text = "Status", Value = "Status" },
            }, "Value", "Text");
        }

        public SelectList Filtros { get; set; }

        [Display(Name = "Filtrar por:")]
        public string Filtro { get; set; }

        [Display(Name = "Conteúdo:")]
        public string Conteudo { get; set; }

        public List<SelectListItem> ListaTipoEvento { get; set; }

        public int TipoEventoSelecionado { get; set; }

        public List<SelectListItem> ListaStatusPresenca { get; set; }

        public int StatusPresencaSelecionado { get; set; }

        public List<SelectListItem> ListaCongregacoes { get; set; }

        public int CongregacaoSelecionado { get; set; }
    }
}
