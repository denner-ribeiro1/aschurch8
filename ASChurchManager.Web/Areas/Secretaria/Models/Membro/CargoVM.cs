using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro
{
    public class CargoVM
    {
        public bool IsReadOnly { get; set; }

        public long? CargoId { get; set; }

        [Display(Name = "Descrição Cargo")]
        public string DescricaoCargo { get; set; }

        [Display(Name = "Data do Cargo")]
        public string DataCargo { get; set; }

        [Display(Name = "Local de Consagração")]
        public string LocalConsagracao { get; set; }

        [Display(Name = "CONFRADESP Nº")]
        public string Confradesp { get; set; }

        [Display(Name = "CGADB Nº")]
        public string CGADB { get; set; }

        [Display(Name = "Cargo")]
        public IEnumerable<SelectListItem> SelectCargos { get; set; }
    }
}