using ASChurchManager.Domain.Types;
using ASChurchManager.Web.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Relatorio
{
    public class MembroVM : RelatorioViewModel
    {
        [Display(Name = "Status")]
        public Status Status { get; set; }

        [Display(Name = "Estado Civil")]
        public EstadoCivil EstadoCivil { get; set; }

        [Display(Name = "Somente associado ABEDABE")]
        public bool ABEDABE { get; set; }

        public List<SelectListItem> SelectStatus { get;  set; }

        public List<SelectListItem> SelectEstadoCivil { get; set; }

        [Display(Name = "Filtrar Membros confirmados pelo site?")]
        public bool Confirmados { get; set; }

        public bool AtivosConfirmados { get; set; }
    }
}