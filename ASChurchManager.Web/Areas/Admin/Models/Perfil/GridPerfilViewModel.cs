using ASChurchManager.Domain.Types;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.Areas.Admin.ViewModels.Perfil
{
    public class GridPerfilViewModel
    {
        [Key]
        public long Id { get; set; }

        public string Nome { get; set; }

        [Display(Name = "Tipo Perfil")]
        public TipoPerfil TipoPerfil { get; set; }
    }
}