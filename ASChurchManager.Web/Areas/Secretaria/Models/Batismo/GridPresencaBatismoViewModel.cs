using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro
{
    public class GridPresencaBatismoViewModel
    {

        public GridPresencaBatismoViewModel()
        {

        }
        /// <summary>
        /// Id da Congregação
        /// Obs: Sobrescrito para que seja a primeira coluna no Grid.Mvc
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Nome do Membro
        /// </summary>
        [Display(Name = "Nome do Candidato")]
        public string Nome { get; set; }

        /// <summary>
        /// Congregação
        /// </summary>
        [Display(Name = "Congregação")]
        public string Congregacao { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
    }
}