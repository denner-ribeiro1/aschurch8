using System.Collections.Generic;

namespace ASChurchManager.Domain.Entities
{
    public class FichaCongregacao
    {
        public FichaCongregacao()
        {
            Congregacao = new List<Congregacao>();
            Grupos = new List<CongregacaoGrupo>();
            Obreiros = new List<CongregacaoObreiro>();
        }

        public List<Congregacao> Congregacao { get; set; }
        public List<CongregacaoGrupo> Grupos { get; set; }
        public List<CongregacaoObreiro> Obreiros { get; set; }
    }
}
