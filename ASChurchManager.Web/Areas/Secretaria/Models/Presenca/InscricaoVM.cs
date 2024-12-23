using ASChurchManager.Web.Lib;
using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Presenca
{
    public class InscricaoVM : ViewModelBase
    {
        public InscricaoVM(bool naoMembro = true)
        {
            ListaPresenca = new List<SelectListItem>();
            var lista = new List<SelectListItem>();
            if (naoMembro)
            {
                lista.Add(new SelectListItem() { Text = "Código", Value = "Id" });
            }
            lista.Add(new SelectListItem() { Text = "Nome", Value = "Nome" });
            lista.Add(new SelectListItem() { Text = "CPF", Value = "Cpf" });
            if (naoMembro)
            {
                lista.Add(new SelectListItem() { Text = "Congregação", Value = "CongregacaoId" });
            }
            Filtros = new SelectList(lista, "Value", "Text");

            ListaCongregacoes = new List<SelectListItem>();
        }

        [Display(Name = "Cursos/Eventos")]
        [Required(ErrorMessage = "Cursos/Eventos é de preenchimento obrigatório")]
        public long Presenca { get; set; }

        public List<SelectListItem> ListaPresenca { get; set; }

        [Display(Name = "Cursos/Eventos")]
        public string Descricao { get; set; }

        [Display(Name = "Data Máxima")]
        public string DataMaxima { get; set; }

        [Display(Name = "Data/Hora Inicio")]
        public string DataHoraInicio { get; set; }

        public bool Membro { get; set; } = true;

        [Display(Name = "Cod. Registro")]
        [Range(1, int.MaxValue, ErrorMessage = "Registro do Membro deve ser maior que zero")]
        public int? MembroId { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Nome é de preenchimento obrigatório")]
        public string Nome { get; set; }

        [Display(Name = "CPF"), Cpf(ErrorMessage = "CPF Inválido")]
        [Required(ErrorMessage = "CPF é de preenchimento obrigatório")]
        public string Cpf { get; set; }

        [Display(Name = "Congregação/Igreja")]
        public string Congregacao { get; set; }

        public int CongregacaoEventoId { get; set; }

        public string CongregacaoEvento { get; set; }

        [Display(Name = "Igreja")]
        public string Igreja { get; set; }

        [Display(Name = "Cargo")]
        public string Cargo { get; set; }

        [Display(Name = "Pago")]
        public bool Pago { get; set; }

        public double Valor { get; set; }

        public SelectList Filtros { get; set; }

        [Display(Name = "Filtrar por:")]
        public string Filtro { get; set; }

        [Display(Name = "Conteúdo:")]
        public string Conteudo { get; set; }

        [Cpf(ErrorMessage = "CPF Inválido")]
        public string CpfFiltro { get; set; }

        public List<SelectListItem> ListaCongregacoes { get; set; }

        public int CongregacaoSelecionado { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Arquivo em Excel")]
        public string Arquivo { get; set; }

        public bool PermiteInscricoes { get; set; }

        public bool NaoMembro { get; set; }
    }
}
