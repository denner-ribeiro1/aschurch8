using ASChurchManager.Web.ViewModels.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Casamento
{

    public class CasamentoViewModel : ViewModelBase
    {

        [Key]
        public long CasamentoId { get; set; }

        //-------------------------------------------- Congregação
        [Required(ErrorMessage = "O campo Código da Congregação é obrigatório"),
         Display(Name = "Cód. Congregação")]
        public long? CongregacaoId { get; set; }

        [Required(ErrorMessage = "O campo Nome da Congregação é obrigatório"),
         Display(Name = "Nome Congregação")]
        public string CongregacaoNome { get; set; }

        public bool PastorMembro { get; set; }

        [Display(Name = "Cód. Pastor")]
        public long? PastorId { get; set; }

        [Display(Name = "Nome do Pastor"),
         Required(ErrorMessage = "Nome do Pastor é obrigatório"),
         StringLength(100)]
        public string PastorNome { get; set; }

        [Display(Name = "Data do Casamento"),
         DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true),
         Required(ErrorMessage = "Data do Casamento é de preenchimento obrigatório")]
        public DateTimeOffset? DataCasamento { get; set; }

        [Display(Name = "Hora de Início"),
        DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true),
        Required(ErrorMessage = "Hora de Início do Casamento é de preenchimento obrigatório")]
        public TimeSpan? HoraInicio { get; set; }

        [Display(Name = "Hora do Término"),
        DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true),
        Required(ErrorMessage = "Hora de Término do Casamento é de preenchimento obrigatório")]
        public TimeSpan? HoraFim { get; set; }
        
        //-------------------------------------------- Noivo
        public bool NoivoMembro { get; set; }

        [Display(Name = "Cód.Reg")]
        public long? NoivoId { get; set; }

        [Display(Name = "Nome do Noivo"),
        Required(ErrorMessage = "Nome do Noivo é obrigatório"),
        StringLength(100)]
        public string NoivoNome { get; set; }

        public bool PaiNoivoMembro { get; set; }

        [Display(Name = "Cód.Reg.Pai")]
        public long? PaiNoivoId { get; set; }

        [Display(Name = "Nome do Pai"),
        StringLength(100)]
        public string PaiNoivoNome { get; set; }

        public bool MaeNoivoMembro { get; set; }

        [Display(Name = "Cód.Reg.Mãe")]
        public long? MaeNoivoId { get; set; }

        [Display(Name = "Nome da Mãe"),
        StringLength(100)]
        public string MaeNoivoNome { get; set; }

        //-------------------------------------------- Noiva
        public bool NoivaMembro { get; set; }

        [Display(Name = "Cód.Reg")]
        public long? NoivaId { get; set; }

        [Display(Name = "Nome da Noiva"),
         Required(ErrorMessage = "Nome da Noiva é obrigatório"),
         StringLength(100)]
        public string NoivaNome { get; set; }

        public bool PaiNoivaMembro { get; set; }

        [Display(Name = "Cód.Reg.Pai")]
        public long? PaiNoivaId { get; set; }

        [Display(Name = "Nome do Pai"),
        StringLength(100)]
        public string PaiNoivaNome { get; set; }

        public bool MaeNoivaMembro { get; set; }

        [Display(Name = "Cód.Reg.Mãe")]
        public long? MaeNoivaId { get; set; }

        [Display(Name = "Nome da Mãe"),
        StringLength(100)]
        public string MaeNoivaNome { get; set; }
    }
}