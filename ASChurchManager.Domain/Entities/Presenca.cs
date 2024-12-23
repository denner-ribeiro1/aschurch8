using ASChurchManager.Domain.Types;
using System;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Entities
{
    public class Presenca : BaseEntity
    {
        public Presenca()
        {
            Datas = new List<PresencaDatas>();
            Congregacao = new Congregacao();
        }

        public string Descricao { get; set; }
        public int TipoEventoId { get; set; }
        public string DescrTipoEventoId { get; set; }
        public DateTime DataMaxima { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public double Valor { get; set; }
        public bool ExclusivoCongregacao { get; set; }
        public bool NaoMembros { get; set; }
        public bool GerarEventos { get; set; }
        public bool InscricaoAutomatica { get; set; }
        public int CongregacaoId { get; set; }
        public Congregacao Congregacao { get; set; }
        public StatusPresenca Status { get; set; }
        public List<PresencaDatas> Datas { get; set; }
    }
}
