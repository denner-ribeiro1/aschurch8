using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASChurchManager.Domain.Entities
{
    [Serializable]
    public class Evento : BaseEntity
    {
        [Serializable]
        public enum TipoFrequencia
        {
            [Display(Name = "")]
            NaoDefinido = 0,

            [Display(Name = "Único")]
            Unico = 1,

            [Display(Name = "Dias")]
            Dias = 2,

            [Display(Name = "Semanas")]
            Semanas = 3,

            [Display(Name = "Meses")]
            Meses = 4,

            [Display(Name = "Anos")]
            Anos = 5
        }
        [Serializable]
        public enum TipoEvento
        {
            [Display(Name = "")]
            NaoDefinido = 0,

            [Display(Name = "Evento")]
            Evento = 1,

            [Display(Name = "Casamento")]
            Casamento = 2,

            [Display(Name = "Batismo")]
            Batismo = 3,

            [Display(Name = "Cursos/Eventos")]
            Curso = 4
        }

        public Evento()
        {
            Congregacao = new Congregacao();
        }

        public int CongregacaoId { get; set; }
        public Congregacao Congregacao { get; set; }
        public int TipoEventoId { get; set; }
        public string DescrTipoEventoId { get; set; }
        public string Descricao { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public string Observacoes { get; set; }
        public int IdEventoOriginal { get; set; }
        public TipoFrequencia Frequencia { get; set; }
        public int Quantidade { get; set; }
        public TipoEvento Tipo { get; set; }
        public bool AlertarEventoMesmoDia { get; set; }
    }
}
