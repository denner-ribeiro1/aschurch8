using System;
using ASChurchManager.Domain.Types;

namespace ASChurchManager.Domain.Entities
{
    public class Carta : BaseEntity
    {
        /// <summary>
        /// Id membro
        /// </summary>
        public long MembroId { get; set; }
        /// <summary>
        /// Membro
        /// </summary>
        public string Nome { get; set; }
        /// <summary>
        /// Tipo de Carta
        /// </summary>
        public TipoDeCarta TipoCarta { get; set; }
        /// <summary>
        /// Id da congregação de origem
        /// </summary>
        public long CongregacaoOrigemId { get; set; }
        /// <summary>
        /// Congregacao Origem
        /// </summary>
        public string CongregacaoOrigem { get; set; }
        /// <summary>
        /// Cidade Congregação Origem
        /// </summary>
        public string CongregacaoOrigemCidade { get; set; }
        /// <summary>
        /// Id congregacao destino
        /// </summary>
        public long CongregacaoDestId { get; set; }
        /// <summary>
        /// congregacao destino
        /// </summary>
        public string CongregacaoDest { get; set; }
        /// <summary>
        /// Observação
        /// </summary>
        public string Observacao { get; set; }
        /// <summary>
        /// status da carta
        /// </summary>
        public StatusCarta StatusCarta { get; set; }
        /// <summary>
        /// Data de Vencimento
        /// </summary>
        public DateTimeOffset DataValidade { get; set; }
        /// <summary>
        /// Data de Vencimento
        /// </summary>
        public DateTimeOffset DataRecepcao { get; set; }
        /// <summary>
        /// código de recebimento da carta
        /// </summary>
        public string CodigoRecebimento { get; set; }
        public long TemplateId { get; set; }
        /// <summary>
        /// Cargo
        /// </summary>
        public string Cargo { get; set; }
        /// <summary>
        /// CGADB
        /// </summary>
        public string CGADB { get; set; }
        /// <summary>
        /// Confradesp
        /// </summary>
        public string CONFRADESP { get; set; }

        /// <summary>
        /// Codigo do Membro responsavel pelo cadastro.
        /// </summary>
        public int IdCadastro { get; set; }
    }
}
