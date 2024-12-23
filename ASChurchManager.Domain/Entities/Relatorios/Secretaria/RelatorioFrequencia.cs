using System;
using System.Collections.Generic;
using System.Text;

namespace ASChurchManager.Domain.Entities.Relatorios.Secretaria
{
    public class RelatorioFrequencia
    {
        public RelatorioFrequencia()
        {
            Membros = new List<MembroSimplificado>();
            InscricoesCurso = new List<InscricaoCurso>();
            Datas = new List<PresencaData>();
            Presencas = new List<Presenca>();
        }

        public List<MembroSimplificado> Membros { get; set; }

        public List<InscricaoCurso> InscricoesCurso { get; set; }

        public List<PresencaData> Datas { get; set; }

        public List<Presenca> Presencas { get; set; }

    }

    public class MembroSimplificado
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Cargo { get; set; }

        public int CongregacaoId { get; set; }

        public string Congregacao { get; set; }

        public string CPF { get; set; }
    }

    public class InscricaoCurso
    {
        public int Id { get; set; }

        public int PresencaId { get; set; }

        public int MembroId { get; set; }

        public string CPF { get; set; }
    }

    public class PresencaData
    {
        public int Id { get; set; }

        public int PresencaId { get; set; }

        public DateTime Data { get; set; }
    }

    public class Presenca
    {
        public int InscricaoId { get; set; }
        public int DataId { get; set; }
        public int Situacao { get; set; }
    }
}
