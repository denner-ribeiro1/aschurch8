using ASChurchManager.Domain.Types;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Entities
{
    public class Congregacao : BaseEntity
    {
        public Congregacao()
        {
            Endereco = new Endereco();
            Grupos = new List<CongregacaoGrupo>();
            Obreiros = new List<CongregacaoObreiro>();
            Observacoes = new List<ObservacaoCongregacao>();
        }

        public string Nome { get; set; }
        public bool Sede { get; set; }
        public Endereco Endereco { get; set; }
        public long CongregacaoResponsavelId { get; set; }
        public string CongregacaoResponsavelNome { get; set; }
        public long PastorResponsavelId { get; set; }
        public string PastorResponsavelNome { get; set; }
        public string CNPJ { get; set; }
        public List<CongregacaoGrupo> Grupos { get; set; }
        public List<CongregacaoObreiro> Obreiros { get; set; }
        public List<ObservacaoCongregacao> Observacoes { get; set; }

    }

    public class QuantidadePorCongregacao
    {
        public string Tipo { get; set; }
        public string TipoDescr
        {
            get
            {
                return Tipo switch
                {
                    "M" => "Membros",
                    "E" => "Eventos",
                    "C" => "Casamentos",
                    "N" => "Nascimentos",
                    "O" => "Obreiros",
                    "P" => "Cursos/Eventos",
                    "U" => "Usuários",
                    _ => "",
                };
            }
        }

        public int Quantidade { get; set; }

        public TipoMembro TipoMembro { get; set; }

        public Status Status { get; set; }
        public string TipoMembroDescr { get; set; }

        public string StatusDescr { get; set; }
    }
}