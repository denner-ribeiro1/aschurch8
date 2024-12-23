using ASChurchManager.Web.ViewModels.Shared;

namespace ASChurchManager.Web.Areas.Secretaria.ViewModels.Membro
{
    public class GridMembroItem : EntityViewModelBase
    {
        public override long Id { get; set; }

        public string Congregacao { get; set; }

        public string Nome { get; set; }

        public string NomeMae { get; set; }

        public string Cpf { get; set; }

        public string Status { get; set; }

        public bool Sede { get; set; }

        public bool PermiteAprovarMembro { get; set; }

        public bool PermiteImprimirCarteirinha { get; set; }

    }
}