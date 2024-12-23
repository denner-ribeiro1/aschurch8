using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.ViewModels.Search
{
    public class EventosSearch
    {
        [Pesquisa(Display = "Descrição", Mask = "", Tipo = TipoCampo.Inteiro)]
        public string Descricao { get; set; }
    }
}