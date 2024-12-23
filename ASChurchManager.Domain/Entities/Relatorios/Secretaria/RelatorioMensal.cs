using System;
using System.Collections.Generic;
namespace ASChurchManager.Domain.Entities.Relatorios.Secretaria
{
    public class RelatorioMensal
    {
        public RelatorioMensal()
        {
            Casamento = new List<Casamento>();
            Reconciliacao = new List<NomeData>();
            RecebidoCartaMudanca = new List<NomeData>();
            RecebidoAclamacao = new List<NomeData>();
            RecebidoTransferencia = new List<NomeData>();
            SaidaPor = new List<SaidaPor>();
            Funeral = new List<NomeData>();
            SaidaPorTranferencia = new List<SaidaPorTranferencia>();
            SaidaPorMudanca = new List<SaidaPorTranferencia>();
            CriancasApresentadas = new List<CriancasApresentadas>();
            Totais = new List<Totais>();
        }

        public List<Casamento> Casamento { get; set; }
        public List<NomeData> Reconciliacao { get; set; }
        public List<NomeData> RecebidoCartaMudanca { get; set; }
        public List<NomeData> RecebidoAclamacao { get; set; }
        public List<NomeData> RecebidoTransferencia { get; set; }
        public List<SaidaPor> SaidaPor { get; set; }
        public List<NomeData> Funeral { get; set; }
        public List<SaidaPorTranferencia> SaidaPorTranferencia { get; set; }
        public List<SaidaPorTranferencia> SaidaPorMudanca { get; set; }
        public List<CriancasApresentadas> CriancasApresentadas { get; set; }
        public List<Totais> Totais { get; set; }
    }

    public class NomeData
    {
        public string Nome { get; set; }
        public DateTimeOffset Data { get; set; }
    }
    public class SaidaPor : NomeData
    {
        public string Motivo { get; set; }
    }
    public class SaidaPorTranferencia : NomeData
    {
        public string NomeCongregacao { get; set; }
    }
    public class CriancasApresentadas
    {
        public string NomeCrianca { get; set; }
        public DateTimeOffset DataNascimento { get; set; }
        public string NomePai { get; set; }
        public string NomeMae { get; set; }
        public string PastorOficiante { get; set; }
        public DateTimeOffset DataApresentacaoCrianca { get; set; }
    }
    public class Totais
    {
        public int TotalMembros { get; set; }
        public int TotalCriancasApresentadas { get; set; }
        public int TotalCriancas { get; set; }
        public int TotalCongregados { get; set; }
        public int TotalNovosConvertidos { get; set; }
        public int TotalDemissoes { get; set; }
        public int TotalAdmissoes { get; set; }
        public int TotalRecAclamacao { get; set; }
        public int TotalRecMudanca { get; set; }
        public int TotalRecebidoBatismo { get; set; }
    }
}
