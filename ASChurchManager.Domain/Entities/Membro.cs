using System.Collections.Generic;
using ASChurchManager.Domain.Types;
using System;

namespace ASChurchManager.Domain.Entities
{
    public class Membro : BaseEntity
    {
        public Membro()
        {
            Congregacao = new Congregacao();
            Endereco = new Endereco();
            Observacoes = new List<ObservacaoMembro>();
            Situacoes = new List<SituacaoMembro>();
            Cargos = new List<CargoMembro>();
        }

        public MembroRecebidoPor RecebidoPor { get; set; }
        public Types.Status Status { get; set; }
        public TipoMembro TipoMembro { get; set; }
        public Congregacao Congregacao { get; set; }
        public DateTimeOffset? DataRecepcao { get; set; }
        public DateTimeOffset? DataBatismoAguas { get; set; }
        public bool BatimoEspiritoSanto { get; set; }
        public bool ABEDABE { get; set; }
        public long CriadoPorId { get; set; }
        public long AprovadoPorId { get; set; }
        public bool Obreiro { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string RG { get; set; }
        public string OrgaoEmissor { get; set; }
        public DateTimeOffset? DataNascimento { get; set; }
        public Membro Pai { get; set; }
        public string NomePai { get; set; }
        public Membro Mae { get; set; }
        public string NomeMae { get; set; }
        public EstadoCivil EstadoCivil { get; set; }
        public Escolaridade Escolaridade { get; set; }
        public Sexo Sexo { get; set; }
        public string Nacionalidade { get; set; }
        public string NaturalidadeEstado { get; set; }
        public string NaturalidadeCidade { get; set; }
        public string Profissao { get; set; }
        public string TelefoneResidencial { get; set; }
        public string TelefoneCelular { get; set; }
        public string TelefoneComercial { get; set; }
        public string Email { get; set; }
        public string FotoPath { get; set; }
        public string FotoUrl { get; set; }
        public byte[] FotoByte { get; set; }
        public Endereco Endereco { get; set; }
        public string TituloEleitorNumero { get; set; }
        public string TituloEleitorZona { get; set; }
        public string TituloEleitorSecao { get; set; }
        public string CriadoPorNome { get; set; }
        public string AprovadoPorNome { get; set; }
        public List<SituacaoMembro> Situacoes { get; set; }
        public List<CargoMembro> Cargos { get; set; }
        public List<ObservacaoMembro> Observacoes { get; set; }
        public long BatismoId { get; set; }
        public DateTimeOffset? DataPrevistaBatismo { get; set; }
        public SituacaoCandidatoBatismo BatismoSituacao { get; set; }
        public Membro Conjuge { get; set; }
        public int PastorPresidente { get; set; }
        public string UsuarioReprovacao { get; set; }
        public string MotivoReprovacao { get; set; }
        public IEnumerable<HistoricoCartas> Historico { get; set; }
        public int MembroConfirmado { get; set; }
        public string IpConfirmado { get; set; }
        public Tamanho TamanhoCapa { get; set; }
    }
}