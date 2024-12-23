using ASChurchManager.Domain.Types;
using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Carta
{

    public class CartaViewModel : EntityViewModelBase
    {
        public CartaViewModel()
        {
            SelectTemplates = new List<SelectListItem>();
        }
        public Acao? acao { get; set; }

        [Display(Name = "Cod. Registro")]
        [Required(ErrorMessage = "Registro do Membro é de preenchimento obrigatório")]
        [Range(1, double.MaxValue, ErrorMessage = "Registro do Membro deve ser maior que zero")]
        public int MembroId { get; set; }

        /// <summary>
        /// Nome membro
        /// </summary> 
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Nome do Membro é de preenchimento obrigatório")]
        public string Nome { get; set; }

        /// <summary>
        /// Tipo de Carta
        /// </summary>
        [Display(Name = "Tipo de Carta"),
         Required(ErrorMessage = "Tipo de Carta é de preenchimento obrigatório")]
        public TipoDeCarta TipoCarta { get; set; }

        /// <summary>
        /// Id da congregação de origem
        /// </summary>
        [Display(Name = "Código")]
        [Range(1, double.MaxValue, ErrorMessage = "Código da congregação de origem deve ser maior que zero")]
        public long CongregacaoOrigemId { get; set; }

        /// <summary>
        /// Congregacao Origem
        /// </summary>
        [Display(Name = "Congregação")]
        [Required(ErrorMessage = "Nome da Congregação é de preenchimento obrigatório")]
        public string CongregacaoOrigem { get; set; }

        /// <summary>
        /// Id congregacao destino
        /// </summary>
        [Display(Name = "Cód. Congregação")]
        [Required(ErrorMessage = "Código Congregação Destino é de preenchimento obrigatório")]
        [Range(1, double.MaxValue, ErrorMessage = "Código da congregação de destino deve ser maior que zero")]
        public long CongregacaoDestId { get; set; }

        /// <summary>
        /// congregacao destino
        /// </summary>
        [Display(Name = "Congregação de Destino")]
        [Required(ErrorMessage = "Congregação de Destino é de preenchimento obrigatório")]
        public string CongregacaoDest { get; set; }

        /// <summary>
        /// Observação
        /// </summary>
        [Display(Name = "Observações")]
        [StringLength(500)]
        public string Observacao { get; set; }

        /// <summary>
        /// Status Carta
        /// </summary>
        public StatusCarta StatusCarta { get; set; }


        [Display(Name = "Status")]
        /// <summary>
        /// Status Carta
        /// </summary>
        public string StatusCartaDescricao => StatusCarta.GetDisplayAttributeValue();

        /// <summary>
        /// Data Validade
        /// </summary>
        [Display(Name = "Data de Validade da Carta"),
        DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataValidade { get; set; }

        /// <summary>
        /// Código de Emissão
        /// </summary>
        [Display(Name = "Código de Recebimento"),
         Required(ErrorMessage = "Código de Recebimento é de preenchimento obrigatório")]
        public string CodigoEmissao { get; set; }

        [Display(Name = "Template de Carta"),
         Required(ErrorMessage = "Template de Carta é obrigatório")]
        public long TemplateCartaSelecionado { get; set; }

        public List<SelectListItem> SelectTemplates { get; set; }
    }
}