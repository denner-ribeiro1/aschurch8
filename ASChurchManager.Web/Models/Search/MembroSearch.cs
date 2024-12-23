namespace ASChurchManager.Web.ViewModels.Search
{
    public class MembroSearch
    {
        [Pesquisa(Key = true, Display = "Código Registro", Mask = "", Tipo = TipoCampo.Inteiro, ExibeColuna = true, TamanhoColuna = 30)]
        public string Id { get; set; }

        [Pesquisa(Display = "Nome", Mask = "", Tipo = TipoCampo.Caracter, ExibeColuna = true, TamanhoColuna = 70)]
        public string Nome { get; set; }

        [Pesquisa(Display = "Cpf", Mask = "999.999.999-99", Tipo = TipoCampo.CPF, ExibeColuna = false, TamanhoColuna = 0)]
        public string CPF { get; set; }
    }
}