using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Types;
using ASChurchManager.Web.ViewModels.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Admin.ViewModels.Perfil
{
    public class PerfilViewModel : EntityViewModelBase
    {
        public PerfilViewModel()
        {
            this.Rotinas = new Dictionary<Rotina, bool>();
        }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Nome { get; set; }

        [Display(Name = "Tipo Perfil")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public TipoPerfil? TipoPerfil { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public IEnumerable<KeyValuePair<Rotina, bool>> Rotinas { get; set; }

    }
}