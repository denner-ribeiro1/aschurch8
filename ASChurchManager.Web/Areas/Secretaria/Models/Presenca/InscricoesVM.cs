using ASChurchManager.Web.ViewModels.Shared;

namespace ASChurchManager.Web.Areas.Secretaria.Models.Presenca
{
    public class InscricoesVM : EntityViewModelBase
    {
        public string MembroId { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }

        public string Igreja { get; set; }

        public bool Situacao { get; set; }

        public string Tipo { get; set; }

        public string Justificativa { get; set; }

        public int IdData { get; set; }

        public string StatusData { get; set; }

    }
}
