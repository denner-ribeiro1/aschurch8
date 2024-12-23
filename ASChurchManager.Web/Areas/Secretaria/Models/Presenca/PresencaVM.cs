using ASChurchManager.Domain.Types;
using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Presenca
{
    public class PresencaVM : EntityViewModelBase
    {
        public PresencaVM()
        {
            NaoMembros = true;
            Exclusivo = false;
            GerarEventos = true;
            InscricaoAutomatica = false;

            Datas = new List<PresencaDataVM>();
            Arquivos = new List<ArquivoVM>();
        }

        [Display(Name = "Tipo de Evento")]
        [Required(ErrorMessage = "Tipo de Evento é obrigatório")]
        public long TipoEventoSelecionado { get; set; }

        public List<SelectListItem> SelectTiposEvento { get; set; }

        public string TipoEventoDescr { get; set; }
        [Display(Name = "Curso/Evento")]
        [Required(ErrorMessage = "O campo Curso/Evento é obrigatório")]
        public string Descricao { get; set; }

        [Display(Name = "Data máxima para o cadastro"),
         Required(ErrorMessage = "O campo Data Máxima para o cadastro é obrigatório"),
         DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataMaximaCadastro { get; set; }

        [Display(Name = "Valor da Inscrição")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public float? Valor { get; set; }

        [Display(Name = "Evento exclusivo da própria Congregação?")]
        public bool Exclusivo { get; set; }

        [Display(Name = "Permite inscrições de Não Membros?")]
        public bool NaoMembros { get; set; }

        [Display(Name = "Gerar Evento no Calendário?")]
        public bool GerarEventos { get; set; }

        [Display(Name = "Permite a Inscrição automática?")]
        public bool InscricaoAutomatica { get; set; }

        [Required(ErrorMessage = "O campo Código da Congregação é obrigatório"),
         Display(Name = "Cód. Congregação")]
        public long? CongregacaoId { get; set; }

        [Required(ErrorMessage = "O campo Nome da Congregação é obrigatório"),
         Display(Name = "Nome Congregação")]
        public string CongregacaoNome { get; set; }

        public List<PresencaDataVM> Datas { get; set; }

        public string DatasJSON
        {
            get
            {
                return JsonConvert.SerializeObject(Datas);
            }
            set
            {
                var format = "dd/MM/yyyy"; // your datetime format
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };
                Datas = JsonConvert.DeserializeObject<List<PresencaDataVM>>(value, dateTimeConverter);
            }
        }
        public StatusPresenca Status { get; set; }

        public Acao Acao { get; set; }

        [Display(Name = "Data"),
         DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? Data { get; set; }

        [DataType(DataType.Time), 
         Display(Name = "Hora Inicio"),
         DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? HoraInicio{ get; set; }

        [DataType(DataType.Time),
         Display(Name = "Hora Final"),
         DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? HoraFinal { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Arquivos permitidos: Word (*.doc e *.docx), Excel (*.xls e *.xlsx), Compactados - (*.zip) e PDF")]
        public string Arquivo { get; set; }

        public List<ArquivoVM> Arquivos { get; set; }
    }
}
