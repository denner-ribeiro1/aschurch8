using System.ComponentModel;

namespace ASChurchManager.Domain.Types
{
    public enum TipoTelefone : byte
    {
        [Description("Residencial")]
        Residencial = 1,
        [Description("Celular")]
        Celular = 2,
        [Description("Comercial")]
        Comercial = 3
    }
}