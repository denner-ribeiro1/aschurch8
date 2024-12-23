using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ASChurchManager.Domain.Entities
{
    public enum TipoStatusRetorno
    {
        [Display(Name = "Sucesso")]
        OK,
        [Display(Name = "Erro")]
        ERRO,
        [Display(Name = "Validações")]
        VALIDACOES
    }

    public class ErroRetorno
    {
        public int Codigo { get; set; }

        public string Mensagem { get; set; }
    }

}
