using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASChurchManager.Web.ViewModels.Search
{
    public class SearchResult
    {
        [Display(Name = "Cód.")]
        public string Id { get; set; }

        [Display(Name = "Descrição")]
        public string Description { get; set; }

        public SearchResult(string id, string description)
        {
            Id = id;
            Description = description;
        }
    }

    public class CampoPropriedade
    {
        public string title { get; set; }
        public string width { get; set; }
        public bool columnResizable { get; set; }
    }
    public class ColunaPropriedade
    {
        public ColunaPropriedade(bool key, string display, string campo, int tamanhoColuna)
        {
            Key = key;
            Display = display;
            Campo = campo;
            TamanhoColuna = tamanhoColuna;
        }

        public bool Key { get; set; }
        public string Display { get; set; }
        public string Campo { get; set; }
        public int TamanhoColuna { get; set; }
    }

    public class SearchViewModel
    {
        public SearchViewModel(string searchModel, string targetElement, string titulo = "", string parametros = "")
        {
            ColunasGrid = new List<ColunaPropriedade>();
            Results = new List<SearchResult>();
            SearchableProperties = new List<SelectListItem>();
            SearchModel = searchModel;
            TargetElementId = targetElement;
            Parametros = parametros;

            if (!string.IsNullOrEmpty(searchModel))
            {
                switch (searchModel)
                {
                    case "Membro":
                    case "MembroObreiros":
                        this.SetSearchType<MembroSearch>();
                        Titulo = string.IsNullOrWhiteSpace(titulo) ? "Pesquisar Membro:" : titulo;
                        break;

                    case "Congregacao":
                        this.SetSearchType<CongregacaoSearch>();
                        Titulo = string.IsNullOrWhiteSpace(titulo) ? "Pesquisar Congregação:" : titulo;
                        break;
                }
            }
        }

        public List<SelectListItem> SearchableProperties { get; set; }

        [Display(Name = "Buscar Por"), Required(ErrorMessage = "Selecione o tipo de busca")]
        public string SearchProperty { get; set; }

        [Display(Name = "Valor"), Required(ErrorMessage = "Digite o que você deseja buscar")]
        public string SearchValue { get; set; }

        public List<SearchResult> Results { get; set; }

        public string SearchModel { get; set; }

        public string TargetElementId { get; set; }

        public string FiltrarUsuario { get; set; }

        public string Titulo { get; set; }

        public string Parametros { get; set; }

        public List<ColunaPropriedade> ColunasGrid { get; set; }

        private void SetSearchType<TEntity>()
        {
            var prop = typeof(TEntity).GetProperties()
                                            .Where(p => p.IsDefined(typeof(PesquisaAttribute), false))
                                            .Select(p => new
                                            {
                                                Value = $"Campo:{p.Name};" +
                                                        $"Mask:{p.GetCustomAttributes(typeof(PesquisaAttribute), false).Cast<PesquisaAttribute>().Single().Mask};" +
                                                        $"Tipo:{p.GetCustomAttributes(typeof(PesquisaAttribute), false).Cast<PesquisaAttribute>().Single().Tipo}",
                                                Text = p.GetCustomAttributes(typeof(PesquisaAttribute), false).Cast<PesquisaAttribute>().Single().Display
                                            });

            prop.ToList().ForEach(p => SearchableProperties
                                        .Add(new SelectListItem()
                                        {
                                            Value = p.Value,
                                            Text = p.Text
                                        }));

            var columns = typeof(TEntity).GetProperties()
                                            .Where(p => p.IsDefined(typeof(PesquisaAttribute), false))
                                            .Select(p => new
                                            {
                                                Campo = p.Name,
                                                p.GetCustomAttributes(typeof(PesquisaAttribute), false).Cast<PesquisaAttribute>().Single().ExibeColuna,
                                                p.GetCustomAttributes(typeof(PesquisaAttribute), false).Cast<PesquisaAttribute>().Single().Display,
                                                p.GetCustomAttributes(typeof(PesquisaAttribute), false).Cast<PesquisaAttribute>().Single().Key,
                                                p.GetCustomAttributes(typeof(PesquisaAttribute), false).Cast<PesquisaAttribute>().Single().TamanhoColuna
                                            });
            columns.Where(p => p.ExibeColuna).ToList().ForEach(p => ColunasGrid.Add(new ColunaPropriedade(p.Key, p.Display, p.Campo, p.TamanhoColuna)));
        }
    }
}