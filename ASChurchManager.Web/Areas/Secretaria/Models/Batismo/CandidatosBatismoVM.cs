using System.ComponentModel.DataAnnotations;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.ViewModels.Shared;
using System;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Batismo
{
    public class CandidatosBatismoVM : EntityViewModelBase
    {
        /// <summary>
        /// Membro ID
        /// </summary>
        [Display(Name = "Candidato Id")]
        public int MembroId { get; set; }

        /// <summary>
        /// Nome do Membro
        /// </summary>
        [Display(Name = "Nome do Candidato")]
        public string Nome { get; set; }

        public string NomeMae { get; set; }

        public string Cpf { get; set; }
        /// <summary>
        /// Congregação
        /// </summary>
        [Display(Name = "Congregação Id")]
        public int CongregacaoId { get; set; }
        /// <summary>
        /// Congregação
        /// </summary>
        [Display(Name = "Congregação")]
        public string CongregacaoNome { get; set; }

        [Display(Name = "Data Nascimento"),
         DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? DataNascimento { get; set; }

        public bool Situacao { get; set; }

        public string DataBatismo { get; set; }
    }
}