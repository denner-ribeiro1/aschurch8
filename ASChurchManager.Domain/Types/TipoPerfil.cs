using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Types
{
    public enum TipoPerfil : byte
    {
        [Display(Name = "Administrador")]
        Administrador = 1,
        [Display(Name = "Usuário")]
        Usuario = 2
    }
}
