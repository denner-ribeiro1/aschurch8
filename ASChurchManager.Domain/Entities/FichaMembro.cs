using System.Collections.Generic;

namespace ASChurchManager.Domain.Entities
{
    public class FichaMembro
    {
        public long Id { get; set; }
        public string Congregacao { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string RG { get; set; }
        public string OrgaoEmissor { get; set; }
        public string DataNascimento { get; set; }
        public string IdPai { get; set; }
        public string NomePai { get; set; }
        public string IdMae { get; set; }
        public string NomeMae { get; set; }
        public string EstadoCivil { get; set; }
        public string Nacionalidade { get; set; }
        public string NaturalidadeEstado { get; set; }
        public string NaturalidadeCidade { get; set; }
        public string Sexo { get; set; }
        public string Escolaridade { get; set; }
        public string Profissao { get; set; }
        public string TituloEleitorNumero { get; set; }
        public string TituloEleitorZona { get; set; }
        public string TituloEleitorSecao { get; set; }
        public string TelefoneResidencial { get; set; }
        public string TelefoneCelular { get; set; }
        public string TelefoneComercial { get; set; }
        public string Email { get; set; }
        public string FotoPath { get; set; }
        public string FotoUrl { get; set; }
        public byte[] FotoByte { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string Cep { get; set; }
        public string RecebidoPor { get; set; }
        public string DataRecepcao { get; set; }
        public string DataBatismoAguas { get; set; }
        public string BatimoEspiritoSanto { get; set; }
        public string Status { get; set; }
        public string TipoMembro { get; set; }
        public string IdConjuge { get; set; }
        public string NomeConjuge { get; set; }
        public string MembroAbedabe { get; set; }
        public string UsuarioCriacao { get; set; }
        public string UsuarioAprovacao { get; set; }
        public string DataCriacao { get; set; }
        public string DataAlteracao { get; set; }
    }

    public class SituacaoFichaMembro
    {
        public string Situacao { get; set; }
        public string Data { get; set; }
        public string Observacao { get; set; }
    }

    public class CargoFichaMembro
    {
        public string Cargo { get; set; }
        public string LocalConsagracao { get; set; }
        public string DataCargo { get; set; }
        public string Confradesp { get; set; }
        public string CGADB { get; set; }
    }

    public class ObservacaoFichaMembro
    {
        public string Observacao { get; set; }
        public string DataCadastro { get; set; }
        public string Nome { get; set; }
    }
    public class HistoricoCartaFichaMembro
    {
        public string CongregacaoOrigem { get; set; }
        public string CongregacaoDestino { get; set; }
        public string DataDaTransferencia { get; set; }
    }

    public class PresencaFichaMembroDatas
    {
        public string Situacao { get; set; }
        public string Justificativa { get; set; }
        public string DataHoraInicio { get; set; }
    }

    public class PresencaFichaMembro
    {
        public PresencaFichaMembro()
        {
            Datas = new List<PresencaFichaMembroDatas>();
        }
        public List<PresencaFichaMembroDatas> Datas { get; set; }
        public long Id { get; set; }
        public string Descricao { get; set; }
    }

    public class RelatorioFichaMembro
    {
        public List<FichaMembro> Membro { get; set; }
        public List<SituacaoFichaMembro> Situacao { get; set; }
        public List<CargoFichaMembro> Cargo { get; set; }
        public List<ObservacaoFichaMembro> Observacao { get; set; }
        public List<HistoricoCartaFichaMembro> Historico { get; set; }
        public List<PresencaFichaMembro> Presenca { get; set; }

        public RelatorioFichaMembro()
        {
            Membro = new List<FichaMembro>();
            Situacao = new List<SituacaoFichaMembro>();
            Cargo = new List<CargoFichaMembro>();
            Observacao = new List<ObservacaoFichaMembro>();
            Historico = new List<HistoricoCartaFichaMembro>();
            Presenca = new List<PresencaFichaMembro>();
        }
    }
}