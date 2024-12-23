using ASChurchManager.Domain.Types;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Entities
{
    public class Template : BaseEntity
    {
        public string Nome { get; set; }

        public string Conteudo { get; set; }

        public TipoTemplate Tipo { get; set; }

        public List<TemplateTag> TagsDisponiveis { get; set; }

        public int MargemAbaixo { get; set; }

        public int MargemEsquerda { get; set; }

        public int MargemDireita { get; set; }

        public int MargemAcima { get; set; }

        public Template()
        {
            this.TagsDisponiveis = new List<TemplateTag>()
            {
                new TemplateTag("NomeCarta", "Nome da Carta", "[NomeCarta]"),
                new TemplateTag("DataAtual", "Data Atual", "[DataAtual]"),
                new TemplateTag("DataAtualExtenso", "Data Atual Extenso", "[DataAtualExtenso]"),
                new TemplateTag("Nome", "Nome", "[Nome]"),
                new TemplateTag("DataRecepcao", "Data da Recepção do Membro", "[DataRecepcao]"),
                new TemplateTag("DataValidade", "Data Validade da Carta", "[DataValidade]"),
                new TemplateTag("CongregacaoOrigem", "Congregação de Origem", "[CongregacaoOrigem]"),
                new TemplateTag("CongregacaoOrigemCidade", "Cidade da Congregação de Origem", "[CongregacaoOrigemCidade]"),
                new TemplateTag("CongregacaoDest", "Congregação de Destino", "[CongregacaoDest]"),
                new TemplateTag("CodigoRecebimento", "Código de Recebimento", "[CodigoRecebimento]"),
                new TemplateTag("Observacao", "Observações", "[Observacao]"),
                new TemplateTag("TipoCarta", "Tipo da Carta", "[TipoCarta]"),
                new TemplateTag("Cargo", "Último Cargo", "[Cargo]"),
                new TemplateTag("CGADB", "Nº CGADB", "[CGADB]"),
                new TemplateTag("CONFRADESP", "Nº CONFRADESP", "[CONFRADESP]")
            };
        }
    }

    public class TemplateTag
    {
        public string Id { get; private set; }
        public string Descricao { get; private set; }
        public string Tag { get; private set; }

        public TemplateTag(string id, string descricao, string tag)
        {
            this.Id = id;
            this.Descricao = descricao;
            this.Tag = tag;
        }
    }
}
