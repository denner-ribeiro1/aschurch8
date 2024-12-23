using ASChurchManager.Domain.Types;
using ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro;
using ASChurchManager.Web.ViewModels;
using ASChurchManager.Web.ViewModels.Shared;
using System;
using System.ComponentModel.DataAnnotations;
using ASChurchManager.Domain.Entities;


namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Nascimento
{
    public class NascimentoViewModel : EntityViewModelBase
    {
        public enum Tipo
        {
            [Display(Name = "Membro")]
            Membro,
            [Display(Name = "Não Membro")]
            NaoMembro
        }
        public enum SimNao
        {
            [Display(Name = "Não")]
            Nao,
            [Display(Name = "Sim")]
            Sim
        }

        public NascimentoViewModel()
        {
            ViewModelTitle = "Nascimento";
        }

        [Key]
        public override long Id { get; set; }

        public bool PaiMembro { get; set; }


        [Display(Name = "Cód. Pai")]
        public long? IdMembroPai { get; set; }

        public bool MaeMembro { get; set; }
        

        [Display(Name = "Cód. Mãe")]
        public long? IdMembroMae { get; set; }

        [Required(ErrorMessage = "O campo Código da Congregação é obrigatório"),
         Display(Name = "Cód. Congregação")]
        public long? CongregacaoId { get; set; }

        [Required(ErrorMessage = "O campo Nome da Congregação é obrigatório"),
         Display(Name = "Nome Congregação")]
        public string CongregacaoNome { get; set; }

        [Display(Name = "Data de Apresentação")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Data de Apresentação é de preenchimento obrigatório")]
        public DateTimeOffset? DataApresentacao { get; set; }

        [Display(Name = "Data de Nascimento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Data de Nascimento é de preenchimento obrigatório")]
        public DateTimeOffset? DataNascimento { get; set; }

        [Display(Name = "Nome do Pai")]
        [Required(ErrorMessage = "Nome do Pai é de preenchimento obrigatório")]
        public string NomePai { get; set; }

        [Display(Name = "Nome da Mãe")]
        [Required(ErrorMessage = "Nome da Mãe é de preenchimento obrigatório")]
        public string NomeMae { get; set; }

        [Display(Name = "Nome da Criança")]
        [Required(ErrorMessage = "Nome da Criança é de preenchimento obrigatório")]
        [StringLength(100)]
        public string Crianca { get; set; }

        [Display(Name = "Sexo")]
        public Sexo Sexo { get; set; }

        [Display(Name = "Nome do Pastor"),
        Required(ErrorMessage = "Nome do Pastor é obrigatório"),
        StringLength(100)]
        public string Pastor { get; set; }

        [Display(Name = "Cód. Pastor")]
        public long? PastorId { get; set; }

        public bool PastorMembro { get; set; }
    }
}