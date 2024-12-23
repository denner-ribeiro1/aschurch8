using System;
using System.Collections.Generic;
using System.Text;

namespace ASChurchManager.Domain.Entities
{
    public class ObservacaoCongregacao
    {
        public ObservacaoCongregacao()
        {
            Usuario = new Usuario();
        }
        public long CongregacaoId { get; set; }
        public string Observacao { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
