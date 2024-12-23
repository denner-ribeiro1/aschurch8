using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Web.ViewModels.Shared
{
    public abstract class ViewModelBase
    {
        /// <summary>
        /// Titulo da ViewModel
        /// ScaffoldColumn: Não será gerado o elemento HTML correspondente via Scaffold do MVC
        /// GridHiddenColumn: Não será gerada a coluna no GridMvc
        /// </summary>
        [ScaffoldColumn(false)]
        public string ViewModelTitle { get; set; }

        /// <summary>
        /// Define se a view deve ser somente leitura
        /// ScaffoldColumn: Não será gerado o elemento HTML correspondente via Scaffold do MVC
        /// GridHiddenColumn: Não será gerada a coluna no GridMvc
        /// </summary>
        [ScaffoldColumn(false)]
        public bool IsReadOnly { get; set; }
    }
}