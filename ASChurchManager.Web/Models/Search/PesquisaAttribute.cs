using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASChurchManager.Web.ViewModels.Search
{
    public enum TipoCampo
    {
        Caracter,
        Inteiro,
        CPF
    }
    public class PesquisaAttribute : Attribute
    {
        public bool Key { get; set; }
        public string Display { get; set; }
        public TipoCampo Tipo { get; set; }
        public string Mask { get; set; }
        public bool ExibeColuna { get; set; }
        public int TamanhoColuna { get; set; }
    }
}