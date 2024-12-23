using ASChurchManager.Domain.Types;
using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static ASChurchManager.Domain.Entities.Evento;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Eventos
{

    public class EventosViewModel : EntityViewModelBase
    {
        public EventosViewModel()
        {
            SelectFrequencia = (from item in Enum.GetValues(typeof(TipoFrequencia)).Cast<TipoFrequencia>()
                                select new SelectListItem
                                {
                                    Text = string.IsNullOrWhiteSpace(item.GetDisplayAttributeValue().Trim()) ? "" : item.GetDisplayAttributeValue(),
                                    Value = ((int)item).ToString()
                                }).ToList().Where(x => !string.IsNullOrWhiteSpace(x.Text)).ToList();
        }
        public Acao Acao { get; set; }
        public int IdEventoOriginal { get; set; }
        public string Titulo { get; set; }
        //-------------------------------------------- Congregação
        [Required(ErrorMessage = "O campo Código da Congregação é obrigatório"),
         Display(Name = "Cód. Congregação")]
        public long? CongregacaoId { get; set; }

        [Required(ErrorMessage = "O campo Nome da Congregação é obrigatório"),
         Display(Name = "Nome Congregação")]
        public string CongregacaoNome { get; set; }

        [Display(Name = "Tipo de Evento")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public long TipoEventoSelecionado { get; set; }

        public List<SelectListItem> SelectTiposEvento { get; set; }

        public string TipoEventoDescr { get; set; }

        [Display(Name = "Evento"),
         Required(ErrorMessage = "Descrição do Evento é de preenchimento obrigatório")]
        public string Descricao { get; set; }

        public string BgColor { get; set; }

        [Display(Name = "Data Inicial"),
         DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true),
         Required(ErrorMessage = "Data Inicial é de preenchimento obrigatório")]
        public DateTime? DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        [DataType(DataType.Time),
         Display(Name = "Hora Inicial"),
         DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true),
         Required(ErrorMessage = "Hora Inicial é de preenchimento obrigatório")]
        public TimeSpan HoraInicio { get; set; }
        public string HoraInicioStr { get { return HoraInicio.ToString(); } }

        [DataType(DataType.Time),
         Display(Name = "Hora Final"),
         DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true),
         Required(ErrorMessage = "Hora Final é de preenchimento obrigatório")]
        public TimeSpan HoraFinal { get; set; }
        public string HoraFinalStr { get { return HoraFinal.ToString(); } }
        [Display(Name = "Frequência")]
        public int TipoFrequenciaSelecionado { get; set; }

        public List<SelectListItem> SelectFrequencia { get; set; }

        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

        [Display(Name = "Observações")]
        public string Observacoes { get; set; }

        public TipoEvento Tipo { get; set; }

        public string TipoDescr { get; set; }

        [Display(Name = "Evento com presença Obrigatória para todas as Congregações")]
        public bool AlertarEventoMesmoDia { get; set; }

        public int CodigoCongregacaoSede { get; set; }

        public bool IsFeriado { get; set; }
    }
}