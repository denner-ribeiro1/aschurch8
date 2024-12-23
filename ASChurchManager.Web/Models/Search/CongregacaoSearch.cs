namespace ASChurchManager.Web.ViewModels.Search
{
    public class CongregacaoSearch
    {
        [Pesquisa(Key = true, Display = "Código Registro", Mask = "", Tipo = TipoCampo.Inteiro, ExibeColuna = true, TamanhoColuna = 30)]
        public string Id { get; set; }

        [Pesquisa(Display = "Nome", Mask = "", Tipo = TipoCampo.Caracter, ExibeColuna = true, TamanhoColuna = 70)]
        public string Nome { get; set; } 
    }
}