using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Congregacao
{
    public class ObservacaoVM
    {
        public bool IsReadOnly { get; set; }
        [Display(Name = "Observação"), MaxLength(300)]
        public string Observacao { get; set; }
        [Display(Name = "Data de Cadastro")]
        public string DataCadastroObs { get; set; }
        [Display(Name = "Responsável")]
        public string ResponsavelObs { get; set; }
        public int IdResponsavelObs { get; set; }
    }
}
