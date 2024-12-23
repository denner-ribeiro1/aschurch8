using ASChurchManager.Web.Lib;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Presenca
{
    public class EmissaoEtiquetaVM
    {
        public EmissaoEtiquetaVM()
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
        }

        [Display(Name = "Cursos/Eventos")]
        public long PresencaId { get; set; }

        public List<SelectListItem> ListaPresenca { get; set; }

        [Display(Name = "Congregação")]
        public int CongregacaoId { get; set; }

        public List<SelectListItem> ListaCongregacoes { get; set; }

        [Display(Name = "Tipo de Inscrição")]
        public int TipoInscricao { get; set; }

        public SelectList ListaTipoInscricao{ get; set; }

        [Display(Name = "Cod. Registro")]
        [Range(1, int.MaxValue, ErrorMessage = "Registro do Membro deve ser maior que zero")]
        public int? MembroId { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Nome é de preenchimento obrigatório")]
        public string NomeMembro { get; set; }

        [Display(Name = "CPF"), Cpf(ErrorMessage = "CPF Inválido")]
        [Required(ErrorMessage = "CPF é de preenchimento obrigatório")]
        public string Cpf { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Nome é de preenchimento obrigatório")]
        public string NomeCpf { get; set; }

        [Display(Name = "Esquerda")]
        public int MargemEsquerda { get; set; } = 0;

        [Display(Name = "Top")]
        public int MargemTop { get; set; } = 0;
    }


}
