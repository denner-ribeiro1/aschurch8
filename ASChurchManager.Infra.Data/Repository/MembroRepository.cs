using ASBaseLib.Data.Helpers.Microsoft.ApplicationBlocks;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Types;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ASChurchManager.Infra.Data.Repository.EnterpriseLibrary
{
    public class MembroRepository : RepositoryDAO<Membro>, IMembroRepository
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString { get; set; }


        private readonly StorageCredentials storageCredentials;

        private readonly CloudStorageAccount storageAccount;

        public MembroRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration["ConnectionStrings:ConnectionDB"];

            var accountName = _configuration["AzureStorage:AccountName"];
            var accountKey = _configuration["AzureStorage:AccountKey"];

            storageCredentials = new StorageCredentials(accountName, accountKey);
            storageAccount = new CloudStorageAccount(storageCredentials, true);
        }

        public MembroRepository(string connection)
        {
            ConnectionString = connection;
        }

        #region Constantes
        private const string SalvarMembro = "dbo.SalvarMembro";
        private const string ListarMembros = "dbo.ListarMembros";
        private const string ConsultarMembro = "dbo.ConsultarMembro";
        private const string DeletarMembro = "dbo.DeletarMembro";
        private const string ExisteCodigoRegistroMembro = "dbo.ExisteCodigoRegistroMembro";
        private const string DeletarSituacaoMembro = "dbo.DeletarSituacaoMembro";
        private const string SalvarSituacaoMembro = "dbo.SalvarSituacaoMembro";
        private const string ListarSituacoes = "dbo.ListarSituacaoMembro";
        private const string DeletarCargoMembro = "dbo.DeletarCargoMembro";
        private const string SalvarCargoMembro = "dbo.SalvarCargoMembro";
        private const string ListarCargos = "dbo.ListarCargosMembro";
        private const string ExisteCPFDuplicadoMembro = "dbo.ExisteCPFDuplicadoMembro";
        private const string ConsultarPorCPF = "dbo.ConsultarPorCPF";
        private const string ConsultaMembroConfirmado = "ConsultaMembroConfirmado";
        private const string ListarObservacao = "dbo.ListarObservacaoMembro";
        private const string SalvarObservacao = "dbo.SalvarObservacaoMembro";
        private const string DeletarObservacaoMembro = "dbo.DeletarObservacaoMembro";
        private const string DeletarBatismoCandidato = "DeletarBatismoCandidato";
        private const string AprovaReprovaMembro = "dbo.AprovaReprovaMembro";
        private const string ListarHistorico = "ListarHistoricoTrans";
        private const string MembrosPendencias = "ListarMembrosPendencias";
        private const string CarteirinhaMembro = "CarteirinhaMembros";
        private const string AtualValidadeCarteirinha = "AtualizarValidadeCarteirinha";
        private const string AtualizarStatusPorSituacao = "AtualizarStatusPorSituacao";
        private const string RelFichaMembro = "ConsultarFichaMembro";
        private const string InclCandidatoBatismo = "IncluirCandidatoBatismo";
        private const string BuscarMembro = "BuscarMembro";
        private const string MembroPaginada = "ListarMembroPaginada";
        private const string MembroObreiroPaginada = "ListarMembroObreiroPaginada";
        private const string UpdMembroExterno = "AtualizarMembroExterno";
        private const string RestaurarMembroConf = "RestaurarMembroConfirmado";
        private const string DeleteObreiroNaCongregacao = "DeleteObreiroNaCongregacao";
        private const string AtualMembroFotoUrl = "AtualizarMembroFotoUrl";
        private const string ListaCarteirinhaMembros = "ListaCarteirinhaMembros";
        private const string AtualSenha = "AtualizarSenha";
        private const string AtualEmail = "AtualizarEmail";
        private const string AtualMembroAtualizado = "AtualizarMembroAtualizado";
        #endregion

        private SqlTransaction transaction = null;
        private SqlConnection connection = null;

        public override long Add(Membro entity,
                                 long usuarioID = 0)
        {
            SqlConnection con = connection;
            var trans = false;
            if (con == null)
            {
                con = new SqlConnection(ConnectionString);
                con.Open();
            }

            if (transaction == null)
            {
                trans = true;
                transaction = con.BeginTransaction();
            }

            try
            {
                /*Delete tabelas auxiliares*/
                var paramsId = new List<SqlParameter>()      {
                    new("@MembroId", entity.Id)
                };
                MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, DeletarCargoMembro, paramsId.ToArray());
                MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, DeletarSituacaoMembro, paramsId.ToArray());
                MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, DeletarObservacaoMembro, paramsId.ToArray());

                var dataNascimento = entity.DataNascimento == DateTimeOffset.MinValue || entity.DataNascimento == null ? (object)DBNull.Value : entity.DataNascimento;
                var dataRecepcao = entity.DataRecepcao == DateTimeOffset.MinValue || entity.DataRecepcao == null ? (object)DBNull.Value : entity.DataRecepcao;
                var dataBatismo = entity.DataBatismoAguas == DateTimeOffset.MinValue || entity.DataBatismoAguas == null ? (object)DBNull.Value : entity.DataBatismoAguas;

                var lstParameters = new List<SqlParameter>
                    {
                        new("@Id", entity.Id),
                        new("@CongregacaoId", entity.Congregacao.Id),
                        new("@Nome", entity.Nome),
                        new("@Cpf", entity.Cpf),
                        new("@RG", entity.RG),
                        new("@OrgaoEmissor", entity.OrgaoEmissor),
                        new("@DataNascimento", dataNascimento),
                        new("@NomePai", entity.NomePai),
                        new("@NomeMae", entity.NomeMae),
                        new("@EstadoCivil", entity.EstadoCivil),
                        new("@Sexo", entity.Sexo),
                        new("@Escolaridade", entity.Escolaridade),
                        new("@Nacionalidade", entity.Nacionalidade),
                        new("@NaturalEstado", entity.NaturalidadeEstado),
                        new("@NaturalCidade", entity.NaturalidadeCidade),
                        new("@Profissao", entity.Profissao),
                        new("@TituloEleitorNumero", entity.TituloEleitorNumero),
                        new("@TituloEleitorZona", entity.TituloEleitorZona),
                        new("@TituloEleitorSecao", entity.TituloEleitorSecao),
                        new("@TelefoneResidencial", entity.TelefoneResidencial),
                        new("@TelefoneCelular", entity.TelefoneCelular),
                        new("@TelefoneComercial", entity.TelefoneComercial),
                        new("@Email", entity.Email),
                        new("@FotoPath", DBNull.Value),
                        new("@Logradouro", entity.Endereco.Logradouro),
                        new("@Numero", entity.Endereco.Numero),
                        new("@Complemento", entity.Endereco.Complemento),
                        new("@Bairro", entity.Endereco.Bairro),
                        new("@Cidade", entity.Endereco.Cidade),
                        new("@Estado", entity.Endereco.Estado.ToString()),
                        new("@Pais", entity.Endereco.Pais),
                        new("@Cep", entity.Endereco.Cep),
                        new("@RecebidoPor", entity.RecebidoPor),
                        new("@DataRecepcao",dataRecepcao),
                        new("@DataBatismoAguas", dataBatismo),
                        new("@BatimoEspiritoSanto", entity.BatimoEspiritoSanto),
                        new("@ABEDABE", entity.ABEDABE),
                        new("@Status", entity.Status),
                        new("@TipoMembro", entity.TipoMembro),
                        new("@IdConjuge", entity.Conjuge.Id),
                        new("@BatismoId", entity.BatismoId),
                        new("@CriadoPorId", entity.CriadoPorId),
                        new("@AprovadoPorId", entity.AprovadoPorId),
                        new("@TamanhoCapa", entity.TamanhoCapa),
                        new("@IdPai", entity.Pai.Id),
                        new("@IdMae", entity.Mae.Id),
                        new("@NomeConjuge", entity.Conjuge.Nome)

                    };
                var idMembro = Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, SalvarMembro, lstParameters.ToArray()));

                var fotoUrl = string.Empty;
                if (!string.IsNullOrWhiteSpace(entity.FotoPath))
                {
                    var tipo = "image/jpeg";
                    var extensao = "jpg";
                    if (entity.FotoPath.IndexOf("image/png") > 0)
                    {
                        extensao = "png";
                        tipo = "image/png";
                    }
                    fotoUrl = UploadBase64ImageAsync(idMembro, entity.FotoPath, tipo, $"foto_{idMembro}.{extensao}").Result;

                    var lstParamFoto = new List<SqlParameter>
                    {
                        new("@Id", idMembro),
                        new("@FotoUrl", fotoUrl)
                    };
                    MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, AtualMembroFotoUrl, lstParamFoto.ToArray());
                }
                else
                {
                    DeleteImage($"foto_{idMembro}.jpg");
                    DeleteImage($"foto_{idMembro}.png");

                    var lstParamFoto = new List<SqlParameter>
                    {
                        new("@Id", idMembro),
                        new("@FotoUrl", DBNull.Value)
                    };
                    MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, AtualMembroFotoUrl, lstParamFoto.ToArray());
                }

                /*SITUACAO*/
                if (entity.Situacoes.Any())
                {
                    var sitAnt = MembroSituacao.NaoDefinido;

                    foreach (var sit in entity.Situacoes.OrderBy(p => p.Data))
                    {
                        var obsSit = !string.IsNullOrWhiteSpace(sit.Observacao) ? sit.Observacao.Replace("\"", "") : (object)DBNull.Value;
                        var paramSit = new List<SqlParameter>
                            {
                                new("@MembroId", idMembro),
                                new("@Situacao", sit.Situacao),
                                new("@Data", sit.Data),
                                new("@Observacao", obsSit),
                                new("@SituacaoAnterior", sitAnt)
                            };
                        MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, SalvarSituacaoMembro, paramSit.ToArray());
                        /*Situação Anterior*/
                        sitAnt = sit.Situacao;
                    }
                }

                /*CARGO*/
                if (entity.Cargos.Any())
                {
                    foreach (var cargo in entity.Cargos.OrderBy(c => c.DataCargo))
                    {
                        var locCons = !string.IsNullOrWhiteSpace(cargo.LocalConsagracao) ? cargo.LocalConsagracao.Replace("\"", "") : (object)DBNull.Value;
                        var confradesp = !string.IsNullOrWhiteSpace(cargo.Confradesp) ? cargo.Confradesp.Replace("\"", "") : (object)DBNull.Value;
                        var cgadb = !string.IsNullOrWhiteSpace(cargo.CGADB) ? cargo.CGADB.Replace("\"", "") : (object)DBNull.Value;

                        var paramCargo = new List<SqlParameter>
                            {
                                new("@CargoId", cargo.CargoId),
                                new("@MembroId", idMembro),
                                new("@DataCargo", cargo.DataCargo),
                                new("@LocalConsagracao", locCons),
                                new("@Confradesp", confradesp),
                                new("@CGADB", cgadb)
                            };
                        MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, SalvarCargoMembro, paramCargo.ToArray());
                    }
                }

                /*OBSERVACAO*/
                if (entity.Observacoes.Any())
                {
                    foreach (var _obs in entity.Observacoes.OrderBy(o => o.DataCadastro))
                    {
                        var obs = !string.IsNullOrWhiteSpace(_obs.Observacao) ? _obs.Observacao.Replace("\"", "") : (object)DBNull.Value;

                        var paramObs = new List<SqlParameter>
                            {
                                new("@MembroId", idMembro),
                                new("@Observacao", obs),
                                new("@UsuarioId", _obs.Usuario.Id),
                                new("@DataCadastro", _obs.DataCadastro)
                            };
                        MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, SalvarObservacao, paramObs.ToArray());
                    }
                }

                /*Atualizo somente o status quando nao for Pendentes de Aprovação*/
                if (entity.TipoMembro == TipoMembro.Membro && entity.Status != Status.PendenteAprovacao)
                {
                    /*Atualizando o status do Membro*/
                    var paramAtuSit = new List<SqlParameter>{
                            new("@MembroId", idMembro)
                        };
                    MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, AtualizarStatusPorSituacao, paramAtuSit.ToArray());

                    /* Excluindo o membro da lista de obreiros quando for Mudança ou falecimento*/

                    var membro = new Membro();

                    var lstParam1 = new List<SqlParameter>
                    {
                        new("@Id", idMembro),
                        new("@UsuarioID", usuarioID)
                    };

                    using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure,
                               ConsultarMembro, lstParam1.ToArray()))
                    {
                        while (dr.Read())
                        {
                            membro = DataMapper.ExecuteMapping<Membro>(dr);
                        }
                    }
                    if (membro.Status == Status.Falecido || membro.Status == Status.Mudou)
                    {
                        var paramDelObr = new List<SqlParameter>{
                            new("@MembroId", idMembro)
                        };
                        MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, DeleteObreiroNaCongregacao, paramDelObr.ToArray());
                    }
                }

                /*Incluindo registro na tabela de CandidatoBatismo*/
                if (entity.TipoMembro == TipoMembro.Batismo && entity.Status != Status.PendenteAprovacao)
                {
                    var paramDelCandBat = new List<SqlParameter>{
                            new("@MembroId", idMembro)
                        };

                    MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, DeletarBatismoCandidato, paramDelCandBat.ToArray());

                    var paramCandBat = new List<SqlParameter>{
                            new("@BatismoId", entity.BatismoId),
                            new("@MembroId", idMembro)
                        };
                    MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, InclCandidatoBatismo, paramCandBat.ToArray());
                }

                if (trans)
                    transaction.Commit();
                return idMembro;
            }
            catch
            {
                if (trans)
                    transaction.Rollback();
                throw;
            }
        }

        public override long Update(Membro entity,
                                    long usuarioID = 0)
        {
            try
            {
                // A lógica para update está na procedure SalvarMembro, passando Id > 0;
                return Add(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override int Delete(Membro entity, long usuarioID = 0)
        {

            var lstParameters = new List<SqlParameter>
                {
                    new("@Id", entity.Id),
                    new("@Trans", transaction != null ? "N" : "S")
                };

            if (transaction != null)
                return Convert.ToInt32(MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, DeletarMembro, lstParameters.ToArray()));

            return Convert.ToInt32(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, DeletarMembro, lstParameters.ToArray())); ;

        }

        public override int Delete(long id, long usuarioID = 0)
        {
            return Delete(new Membro() { Id = id });
        }

        public override Membro GetById(long id, long usuarioID)
        {
            var membro = new Membro();

            var lstParameters = new List<SqlParameter>
            {
                new("@Id", id),
                new("@UsuarioID", usuarioID)
            };


            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure,
                       ConsultarMembro, lstParameters.ToArray()))
            {
                while (dr.Read())
                {
                    membro = DataMapper.ExecuteMapping<Membro>(dr);
                    membro.Endereco = DataMapper.ExecuteMapping<Endereco>(dr);

                    membro.Congregacao.Id = Convert.ToInt64(dr["CongregacaoId"].ToString());
                    membro.Congregacao.Nome = dr["CongregacaoNome"].TryConvertTo<string>();

                    membro.Conjuge = new Membro();
                    if (int.TryParse(dr["IdConjuge"].ToString(), out int IdConj) && IdConj > 0)
                    {
                        membro.Conjuge.Id = Convert.ToInt64(dr["IdConjuge"].ToString());
                        membro.Conjuge.Nome = dr["NomeConjuge"].TryConvertTo<string>();
                    }

                    membro.Pai = new Membro();
                    if (int.TryParse(dr["IdPai"].ToString(), out int IdPai) && IdPai > 0)
                    {
                        membro.Pai.Id = Convert.ToInt64(dr["IdPai"].ToString());
                        membro.Pai.Nome = dr["NomePai"].TryConvertTo<string>();
                    }

                    membro.Mae = new Membro();
                    if (int.TryParse(dr["IdMae"].ToString(), out int IdMae) && IdMae > 0)
                    {
                        membro.Mae.Id = Convert.ToInt64(dr["IdMae"].ToString());
                        membro.Mae.Nome = dr["NomeMae"].TryConvertTo<string>();
                    }

                    membro.DataNascimento = membro.DataNascimento == DateTimeOffset.MinValue ? null : membro.DataNascimento;
                    membro.DataBatismoAguas = membro.DataBatismoAguas == DateTimeOffset.MinValue ? null : membro.DataBatismoAguas;
                    membro.DataRecepcao = membro.DataRecepcao == DateTimeOffset.MinValue ? null : membro.DataRecepcao;
                }
            }

            var situacoes = ListarSituacoesMembro(membro.Id);
            var cargos = ListarCargosMembro(membro.Id);
            var observacoes = ListarObservacaoMembro(membro.Id);

            var HistoricoCartas = ListarHistoricoCartas(membro.Id);
            membro.Situacoes = situacoes.ToList();
            membro.Cargos = cargos.ToList();
            membro.Observacoes = observacoes.ToList();
            membro.Historico = HistoricoCartas;


            return membro;
        }

        public override IEnumerable<Membro> GetAll(long usuarioID)
        {
            List<Membro> lstMembros = new();
            var lstParameters = new List<SqlParameter>
                {
                    new("@UsuarioID", usuarioID)
                };

            try
            {
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ListarMembros, lstParameters.ToArray()))
                {
                    while (dr.Read())
                    {
                        var membro = DataMapper.ExecuteMapping<Membro>(dr);
                        membro.Congregacao.Id = Convert.ToInt64(dr["CongregacaoId"].ToString());
                        membro.Congregacao.Nome = dr["CongregacaoNome"].TryConvertTo<string>();

                        lstMembros.Add(membro);
                    }
                }
            }
            catch (System.Exception)
            {

                throw;
            }

            return lstMembros;
        }

        public bool ExisteCodigoRegistro(long codigoRegistro)
        {
            bool existeMembro = false;

            try
            {
                SqlParameter param = new("@Id", codigoRegistro);

                existeMembro = Convert.ToBoolean(MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure,
                                                                                    ExisteCodigoRegistroMembro, param));
            }
            catch (System.Exception)
            {
                throw;
            }

            return existeMembro;
        }

        public void AdicionarSituacao(SituacaoMembro situacao)
        {
            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new("@MembroId", situacao.MembroId),
                    new("@Situacao", situacao.Situacao),
                    new("@Data", situacao.Data),
                    new("@Observacao", situacao.Observacao)
                };

                MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure,
                                                    SalvarSituacaoMembro, lstParameters.ToArray());

            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<SituacaoMembro> ListarSituacoesMembro(long membroId)
        {

            var lstSituacoes = new List<SituacaoMembro>();

            try
            {
                var param = new SqlParameter("@MembroId", membroId);

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ListarSituacoes, param))
                {
                    while (dr.Read())
                    {
                        var situacao = DataMapper.ExecuteMapping<SituacaoMembro>(dr);
                        lstSituacoes.Add(situacao);
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }

            return lstSituacoes;
        }

        public void ExcluirSituacao(long membroId, long situacaoId)
        {
            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new("@SituacaoId", situacaoId),
                    new("@MembroId", membroId)
                };

                MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, DeletarSituacaoMembro, lstParameters.ToArray());

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AdicionarCargo(long membroId, long cargoId, string LocalConsagracao, DateTimeOffset dataCargo)
        {
            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new("@CargoId", cargoId),
                    new("@MembroId", membroId),
                    new("@DataCargo", dataCargo),
                    new("@LocalConsagracao", LocalConsagracao)

                };

                MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, SalvarCargoMembro, lstParameters.ToArray());

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ExcluirCargo(long membroId, long cargoId)
        {
            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new("@CargoId", cargoId),
                    new("@MembroId", membroId)
                };

                MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, DeletarCargoMembro, lstParameters.ToArray());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<CargoMembro> ListarCargosMembro(long membroId)
        {
            var cargos = new List<CargoMembro>();
            try
            {
                var param = new SqlParameter("@MembroId", membroId);

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ListarCargos, param))
                {
                    while (dr.Read())
                    {
                        var situacao = DataMapper.ExecuteMapping<CargoMembro>(dr);
                        cargos.Add(situacao);
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }

            return cargos;
        }

        public IEnumerable<HistoricoCartas> ListarHistoricoCartas(long membroId)
        {
            var listHist = new List<HistoricoCartas>();
            try
            {
                var param = new SqlParameter("@MembroId", membroId);

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ListarHistorico, param))
                {
                    while (dr.Read())
                    {
                        var hist = new HistoricoCartas();
                        hist.CongregacaoOrigem = dr["CongregacaoOrigem"].TryConvertTo<string>();
                        hist.CongregacaoDestino = dr["CongregacaoDestino"].TryConvertTo<string>();
                        hist.DataDaTransferencia = (DateTimeOffset)dr["DataDaTransferencia"];
                        listHist.Add(hist);
                    }
                }
            }

            catch (System.Exception)
            {
                throw;
            }
            return listHist;
        }

        public Membro ExisteCPFDuplicado(long membroId, string cpf)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@MembroId", membroId),
                    new("@Cpf", cpf)
                };

            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure,
                       ExisteCPFDuplicadoMembro, lstParameters.ToArray()))
            {
                var membro = new Membro();
                while (dr.Read())
                {
                    membro = DataMapper.ExecuteMapping<Membro>(dr);
                    membro.Endereco = DataMapper.ExecuteMapping<Endereco>(dr);
                    membro.Congregacao.Id = Convert.ToInt64(dr["CongregacaoId"].ToString());
                    membro.Congregacao.Nome = dr["CongregacaoNome"].TryConvertTo<string>();

                }
                return membro;
            }

        }

        public IEnumerable<ObservacaoMembro> ListarObservacaoMembro(long membroId)
        {
            var lstObs = new List<ObservacaoMembro>();
            try
            {
                var param = new SqlParameter("@MembroId", membroId);
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ListarObservacao, param))
                {
                    while (dr.Read())
                    {
                        var obs = DataMapper.ExecuteMapping<ObservacaoMembro>(dr);
                        obs.Usuario.Id = Convert.ToInt64(dr["UsuarioId"].ToString());
                        obs.Usuario.Nome = dr["Nome"].TryConvertTo<string>();

                        lstObs.Add(obs);
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            return lstObs;
        }

        public void AdicionarObservacao(ObservacaoMembro obsMembro)
        {
            AdicionarObservacao(obsMembro, null);
        }

        public void AdicionarObservacao(ObservacaoMembro obsMembro, SqlTransaction trans = null)
        {
            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new("@MembroId", obsMembro.MembroId),
                    new("@Observacao", obsMembro.Observacao),
                    new("@UsuarioId", obsMembro.Usuario.Id),
                    new("@DataCadastro", obsMembro.DataCadastro)
                };

                if (trans != null)
                    MicrosoftSqlHelper.ExecuteScalar(trans, CommandType.StoredProcedure, SalvarObservacao, lstParameters.ToArray());
                else
                    MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, SalvarObservacao, lstParameters.ToArray());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ExcluirObservacao(long id)
        {
            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new("@Id", id)
                };
                MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, DeletarObservacaoMembro, lstParameters.ToArray());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AprovarReprovaMembro(long membroId, long usuarioId, Status status, string motivoReprovacao)
        {
            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new("@MembroId", membroId),
                    new("@UsuarioId", usuarioId),
                    new("@Status", status),
                    new("@MotivoReprovacao", motivoReprovacao)
                };

                MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, AprovaReprovaMembro, lstParameters.ToArray());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<Membro> ListarMembrosPendencias(int pageSize, int rowStart, out int rowCount, string sorting, long usuarioID)
        {
            rowCount = 0;
            List<Membro> lstMembros = new();

            var lstParameters = new List<SqlParameter>
                {
                    new("@PAGESIZE", pageSize),
                    new("@ROWSTART", rowStart),
                    new("@SORTING", sorting),
                    new("@USUARIOID", usuarioID)
                };
            try
            {
                using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, MembrosPendencias, lstParameters.ToArray()))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var membro = new Membro()
                            {
                                Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                                Nome = ds.Tables[0].Rows[i]["Nome"].ToString(),
                                NomeMae = ds.Tables[0].Rows[i]["NomeMae"].ToString(),
                                Status = ds.Tables[0].Rows[i]["Status"].ToString().ToEnum<Status>(),
                                Cpf = ds.Tables[0].Rows[i]["Cpf"].ToString()
                            };
                            membro.Congregacao.Nome = ds.Tables[0].Rows[i]["CongregacaoNome"].ToString();

                            lstMembros.Add(membro);
                        }
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        rowCount = Convert.ToInt16(ds.Tables[1].Rows[0][0]);
                    }
                }

            }
            catch
            {
                throw;
            }

            return lstMembros;
        }

        public IEnumerable<Carteirinha> CarteirinhaMembros(long membroId)
        {
            List<Carteirinha> lCarteirinha = new();
            var carteirinha = new Carteirinha();

            var lstParameters = new List<SqlParameter>
                {
                    new("@ID", membroId)
                };

            try
            {
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, CarteirinhaMembro, lstParameters.ToArray()))
                {
                    while (dr.Read())
                    {
                        carteirinha.Id = Convert.ToInt64(dr["Id"].ToString());
                        carteirinha.Nome = dr["Nome"].TryConvertTo<string>();
                        carteirinha.NomePai = dr["NomePai"].TryConvertTo<string>();
                        carteirinha.NomeMae = dr["NomeMae"].TryConvertTo<string>();
                        carteirinha.Cidade = dr["NaturalidadeCidade"].TryConvertTo<string>();
                        carteirinha.Estado = dr["NaturalidadeEstado"].TryConvertTo<string>();
                        carteirinha.Cpf = dr["Cpf"].TryConvertTo<string>();
                        carteirinha.EstadoCivil = dr["EstadoCivil"].TryConvertTo<string>();
                        carteirinha.Confradesp = dr["Confradesp"].TryConvertTo<string>();
                        carteirinha.Cgadb = dr["Cgadb"].TryConvertTo<string>();




                        if (dr["DataRecepcao"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue)
                            carteirinha.DataRecepcao = dr["DataRecepcao"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy");

                        if (dr["DataNascimento"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue)
                            carteirinha.DataNascimento = dr["DataNascimento"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy");

                        carteirinha.EstadoCivil = dr["EstadoCivil"].TryConvertTo<int>() > 0 ? ((EstadoCivil)dr["EstadoCivil"].TryConvertTo<int>()).GetDisplayAttributeValue() : "";
                        carteirinha.RG = dr["RG"].TryConvertTo<string>();

                        if (dr["DataBatismoAguas"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue)
                            carteirinha.DataBatismoAguas = dr["DataBatismoAguas"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy");

                        carteirinha.FotoUrl = dr["FotoUrl"].TryConvertTo<string>();
                        carteirinha.TipoCarteirinha = ((TipoCarteirinha)dr["TipoCarteirinha"].TryConvertTo<int>());
                        carteirinha.LocalConsagracao = dr["LocalConsagracao"].ToString();

                        if (dr["DataConsagracao"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue)
                            carteirinha.DataConsagracao = dr["DataConsagracao"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy");

                        if (dr["DataValidadeCarteirinha"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue)
                            carteirinha.DataValidadeCarteirinha = dr["DataValidadeCarteirinha"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy");
                        lCarteirinha.Add(carteirinha);
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }

            return lCarteirinha;
        }

        public void AtualizarValidadeCarteirinha(long membroId)
        {
            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new("@idMembro", membroId)
                };
                MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, AtualValidadeCarteirinha, lstParameters.ToArray());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public RelatorioFichaMembro FichaMembro(long membroId)
        {
            var ficha = new RelatorioFichaMembro();

            var lstParameters = new List<SqlParameter>
                {
                    new("@Id", membroId)
                };


            using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, RelFichaMembro, lstParameters.ToArray()))
            {
                /*DADOS DO MEMBRO*/
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var membro = new FichaMembro()
                        {
                            Id = Convert.ToInt64(row["Id"].ToString()),
                            Congregacao = row["Congregacao"].ToString(),
                            Nome = row["Nome"].ToString(),
                            Cpf = row["Cpf"].ToString(),
                            RG = row["RG"].ToString(),
                            OrgaoEmissor = row["OrgaoEmissor"].ToString(),
                            DataNascimento = row["DataNascimento"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue ?
                                                    row["DataNascimento"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy") : "",
                            IdPai = row["IdPai"].ToString(),
                            NomePai = row["NomePai"].ToString(),
                            IdMae = row["IdMae"].ToString(),
                            NomeMae = row["NomeMae"].ToString(),
                            EstadoCivil = row["EstadoCivil"].TryConvertTo<int>() > 0 ?
                                            ((EstadoCivil)row["EstadoCivil"].TryConvertTo<int>()).GetDisplayAttributeValue() : "",
                            Nacionalidade = row["Nacionalidade"].ToString(),
                            NaturalidadeEstado = row["NaturalidadeEstado"].ToString(),
                            NaturalidadeCidade = row["NaturalidadeCidade"].ToString(),
                            Sexo = row["Sexo"].TryConvertTo<int>() > 0 ?
                                            ((Sexo)row["Sexo"].TryConvertTo<int>()).GetDisplayAttributeValue() : "",
                            Escolaridade = row["Escolaridade"].TryConvertTo<int>() > 0 ?
                                            ((Escolaridade)row["Escolaridade"].TryConvertTo<int>()).GetDisplayAttributeValue() : "",
                            Profissao = row["Profissao"].ToString(),
                            TituloEleitorNumero = row["TituloEleitorNumero"].ToString(),
                            TituloEleitorZona = row["TituloEleitorZona"].ToString(),
                            TituloEleitorSecao = row["TituloEleitorSecao"].ToString(),
                            TelefoneResidencial = row["TelefoneResidencial"].ToString(),
                            TelefoneCelular = row["TelefoneCelular"].ToString(),
                            TelefoneComercial = row["TelefoneComercial"].ToString(),
                            Email = row["Email"].ToString(),
                            Logradouro = row["Logradouro"].ToString(),
                            Numero = row["Numero"].ToString(),
                            Complemento = row["Complemento"].ToString(),
                            Bairro = row["Bairro"].ToString(),
                            Cidade = row["Cidade"].ToString(),
                            Estado = row["Estado"].ToString(),
                            Pais = row["Pais"].ToString(),
                            Cep = row["Cep"].ToString(),
                            RecebidoPor = row["RecebidoPor"].TryConvertTo<int>() > 0 ?
                                            ((MembroRecebidoPor)row["RecebidoPor"].TryConvertTo<int>()).GetDisplayAttributeValue() : "",
                            DataRecepcao = row["DataRecepcao"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue ?
                                                    row["DataRecepcao"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy") : "",
                            DataBatismoAguas = row["DataBatismoAguas"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue ?
                                                    row["DataBatismoAguas"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy") : "",
                            BatimoEspiritoSanto = row["BatimoEspiritoSanto"].ToString(),
                            Status = row["Status"].TryConvertTo<int>() > 0 ?
                                            ((Status)row["Status"].TryConvertTo<int>()).GetDisplayAttributeValue() : "",
                            TipoMembro = row["TipoMembro"].TryConvertTo<int>() > 0 ?
                                            ((TipoMembro)row["TipoMembro"].TryConvertTo<int>()).GetDisplayAttributeValue() : "",
                            IdConjuge = row["IdConjuge"].ToString(),
                            NomeConjuge = row["NomeConjuge"].ToString(),
                            MembroAbedabe = row["MembroAbedabe"].ToString(),
                            UsuarioCriacao = row["UsuarioCriacao"].ToString(),
                            UsuarioAprovacao = row["UsuarioAprovacao"].ToString(),
                            DataCriacao = row["DataCriacao"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue ?
                                                    row["DataCriacao"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy") : "",
                            DataAlteracao = row["DataAlteracao"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue ?
                                                    row["DataAlteracao"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy") : "",
                            FotoUrl = row["FotoUrl"].ToString()
                        };


                        //if (!string.IsNullOrEmpty(row["FotoPath"].TryConvertTo<string>()))
                        //{
                        //    try
                        //    {
                        //        membro.FotoPath = row["FotoPath"].TryConvertTo<string>().Replace("data:image/png;base64,", "").Replace("data:image/jpeg;base64,", "");
                        //        membro.FotoByte = Convert.FromBase64String(membro.FotoPath);
                        //    }
                        //    catch
                        //    {
                        //        membro.FotoPath = "";
                        //        membro.FotoByte = null;
                        //    }
                        //}
                        ficha.Membro.Add(membro);
                    }
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        var sit = new SituacaoFichaMembro()
                        {
                            Situacao = row["Situacao"].TryConvertTo<int>() > 0 ?
                                            ((MembroSituacao)row["Situacao"].TryConvertTo<int>()).GetDisplayAttributeValue() : "",
                            Data = row["Data"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue ?
                                                    row["Data"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy") : "",
                            Observacao = row["Observacao"].ToString().Replace("\"", "")
                        };
                        ficha.Situacao.Add(sit);
                    }
                }

                if (ds.Tables[2].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[2].Rows)
                    {
                        var carg = new CargoFichaMembro()
                        {
                            Cargo = row["Cargo"].ToString(),
                            LocalConsagracao = row["LocalConsagracao"].ToString().Replace("\"", ""),
                            DataCargo = row["DataCargo"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue ?
                                           row["DataCargo"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy") : "",
                            Confradesp = row["Confradesp"].ToString().Replace("\"", ""),
                            CGADB = row["CGADB"].ToString().Replace("\"", "")

                        };
                        ficha.Cargo.Add(carg);
                    }
                }

                if (ds.Tables[3].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[3].Rows)
                    {
                        var obs = new ObservacaoFichaMembro()
                        {
                            Observacao = row["Observacao"].ToString().Replace("\"", ""),
                            DataCadastro = row["DataCadastro"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue ?
                                                row["DataCadastro"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy") : "",
                            Nome = row["Nome"].ToString()
                        };
                        ficha.Observacao.Add(obs);
                    }
                }

                if (ds.Tables[4].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[4].Rows)
                    {
                        var hist = new HistoricoCartaFichaMembro()
                        {
                            CongregacaoOrigem = row["CongregacaoOrigem"].ToString(),
                            CongregacaoDestino = row["CongregacaoDestino"].ToString(),
                            DataDaTransferencia = row["DataDaTransferencia"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue ?
                                                    row["DataDaTransferencia"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy") : ""
                        };
                        ficha.Historico.Add(hist);
                    }
                }

                if (ds.Tables[5].Rows.Count > 0)
                {
                    var id = 0;
                    var pres = new PresencaFichaMembro();

                    foreach (DataRow row in ds.Tables[5].Rows)
                    {
                        if (id != Convert.ToInt64(row["Id"].ToString()))
                        {
                            if (pres != null && pres.Id > 0)
                            {
                                ficha.Presenca.Add(pres);
                            }
                            pres = new PresencaFichaMembro()
                            {
                                Id = Convert.ToInt64(row["Id"].ToString()),
                                Descricao = row["Descricao"].ToString(),

                            };
                            pres.Datas.Add(new PresencaFichaMembroDatas
                            {
                                DataHoraInicio = row["DataHoraInicio"].TryConvertTo<DateTime>() != DateTime.MinValue ?
                                                 row["DataHoraInicio"].TryConvertTo<DateTime>().ToString("dd/MM/yyyy") : "",
                                Justificativa = row["Justificativa"].ToString(),
                                Situacao = row["Situacao"].ToString()
                            });
                        }
                        else
                        {
                            pres.Datas.Add(new PresencaFichaMembroDatas
                            {
                                DataHoraInicio = row["DataHoraInicio"].TryConvertTo<DateTime>() != DateTime.MinValue ?
                                                 row["DataHoraInicio"].TryConvertTo<DateTime>().ToString("dd/MM/yyyy") : "",
                                Justificativa = row["Justificativa"].ToString(),
                                Situacao = row["Situacao"].ToString()
                            });

                        }

                        id = Convert.ToInt32(row["Id"].ToString());

                    }
                }
            }

            return ficha;
        }

        public void BeginTran()
        {
            connection = new SqlConnection(ConnectionString);
            connection.Open();
            transaction = connection.BeginTransaction();
        }

        public void Commit()
        {
            transaction.Commit();
            connection.Close();
            transaction = null;
            connection = null;
        }

        public void RollBack()
        {
            transaction.Rollback();
            connection.Close();
            transaction = null;
            connection = null;
        }

        public IEnumerable<Membro> BuscarMembros(int pageSize, int rowStart, out int rowCount, string campo, string valor)
        {
            rowCount = 0;
            List<Membro> lstMembros = new();
            var _campo = !string.IsNullOrWhiteSpace(campo) ? (object)campo : DBNull.Value;
            var _valor = !string.IsNullOrWhiteSpace(valor) ? (object)valor : DBNull.Value;

            var lstParameters = new List<SqlParameter>
                {
                    new("@PAGESIZE", pageSize),
                    new("@ROWSTART", rowStart),
                    new("@CAMPO", _campo),
                    new("@VALOR", _valor)
                };

            try
            {
                using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, BuscarMembro, lstParameters.ToArray()))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var membro = new Membro()
                            {
                                Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                                Nome = ds.Tables[0].Rows[i]["Nome"].ToString()
                            };
                            lstMembros.Add(membro);
                        }
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        rowCount = Convert.ToInt16(ds.Tables[1].Rows[0][0]);
                    }
                }

            }
            catch (System.Exception)
            {
                throw;
            }

            return lstMembros;
        }

        public IEnumerable<Membro> ListarMembroPaginado(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor,
            long usuarioID, TipoMembro tipoMembro = TipoMembro.NaoDefinido, Status status = Status.NaoDefinido)
        {
            rowCount = 0;
            List<Membro> lstMembros = [];
            var _campo = !string.IsNullOrWhiteSpace(campo) ? (object)campo : DBNull.Value;
            var _valor = !string.IsNullOrWhiteSpace(valor) ? (object)valor : DBNull.Value;

            var lstParameters = new List<SqlParameter>
                {
                    new("@PAGESIZE", pageSize),
                    new("@ROWSTART", rowStart),
                    new("@SORTING", sorting),
                    new("@USUARIOID", usuarioID),
                    new("@CAMPO", _campo),
                    new("@VALOR", _valor),
                    new("@TIPOMEMBRO", tipoMembro),
                    new("@STATUS", status)
                };
            try
            {
                using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, MembroPaginada, lstParameters.ToArray()))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var membro = new Membro()
                            {
                                Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                                Nome = ds.Tables[0].Rows[i]["Nome"].ToString(),
                                NomeMae = ds.Tables[0].Rows[i]["NomeMae"].ToString(),
                                Status = ds.Tables[0].Rows[i]["Status"].ToString().ToEnum<Status>(),
                                Cpf = ds.Tables[0].Rows[i]["Cpf"].ToString()
                            };
                            membro.Congregacao.Nome = ds.Tables[0].Rows[i]["CongregacaoNome"].ToString();

                            lstMembros.Add(membro);
                        }
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        rowCount = Convert.ToInt16(ds.Tables[1].Rows[0][0]);
                    }
                }

            }
            catch
            {
                throw;
            }

            return lstMembros;
        }

        public Membro GetByCPF(string cpf, bool completo = false)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@Cpf", cpf)
                };

            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure,
                       ConsultarPorCPF, lstParameters.ToArray()))
            {
                var membro = new Membro();
                while (dr.Read())
                {
                    membro = DataMapper.ExecuteMapping<Membro>(dr);
                    membro.AtualizarSenha = dr["AtualizarSenha"].TryConvertTo<bool>();


                    membro.Endereco = DataMapper.ExecuteMapping<Endereco>(dr);
                    membro.Congregacao.Id = Convert.ToInt64(dr["CongregacaoId"].ToString());
                    membro.Congregacao.Nome = dr["CongregacaoNome"].TryConvertTo<string>();
                    membro.Congregacao.CongregacaoResponsavelId = Convert.ToInt64(dr["CongregacaoResponsavelId"].ToString());

                    if (completo)
                    {
                        membro.Conjuge = new Membro();
                        if (int.TryParse(dr["IdConjuge"].ToString(), out int IdConj) && IdConj > 0)
                        {
                            membro.Conjuge.Id = Convert.ToInt64(dr["IdConjuge"].ToString());
                            membro.Conjuge.Nome = dr["NomeConjuge"].TryConvertTo<string>();
                        }

                        membro.Pai = new Membro();
                        if (int.TryParse(dr["IdPai"].ToString(), out int IdPai) && IdPai > 0)
                        {
                            membro.Pai.Id = Convert.ToInt64(dr["IdPai"].ToString());
                            membro.Pai.Nome = dr["NomePai"].TryConvertTo<string>();
                        }

                        membro.Mae = new Membro();
                        if (int.TryParse(dr["IdMae"].ToString(), out int IdMae) && IdMae > 0)
                        {
                            membro.Mae.Id = Convert.ToInt64(dr["IdMae"].ToString());
                            membro.Mae.Nome = dr["NomeMae"].TryConvertTo<string>();
                        }

                        membro.DataNascimento = membro.DataNascimento == DateTimeOffset.MinValue ? null : membro.DataNascimento;
                        membro.DataBatismoAguas = membro.DataBatismoAguas == DateTimeOffset.MinValue ? null : membro.DataBatismoAguas;
                        membro.DataRecepcao = membro.DataRecepcao == DateTimeOffset.MinValue ? null : membro.DataRecepcao;

                        var situacoes = ListarSituacoesMembro(membro.Id);
                        var cargos = ListarCargosMembro(membro.Id);
                        var observacoes = ListarObservacaoMembro(membro.Id);

                        var HistoricoCartas = ListarHistoricoCartas(membro.Id);
                        membro.Situacoes = situacoes.ToList();
                        membro.Cargos = cargos.ToList();
                        membro.Observacoes = observacoes.ToList();
                        membro.Historico = HistoricoCartas;
                    }
                }

                return membro;
            }

        }

        public long AtualizarMembroExterno(Membro entity, string ip)
        {
            try
            {
                //var dataNascimento = entity.DataNascimento == DateTimeOffset.MinValue || entity.DataNascimento == null ? (object)DBNull.Value : entity.DataNascimento;
                //var dataRecepcao = entity.DataRecepcao == DateTimeOffset.MinValue || entity.DataRecepcao == null ? (object)DBNull.Value : entity.DataRecepcao;
                //var dataBatismo = entity.DataBatismoAguas == DateTimeOffset.MinValue || entity.DataBatismoAguas == null ? (object)DBNull.Value : entity.DataBatismoAguas;

                var lstParameters = new List<SqlParameter>
                    {
                        new("@Id", entity.Id),
                        new("@TelefoneResidencial", entity.TelefoneResidencial),
                        new("@TelefoneCelular", entity.TelefoneCelular),
                        new("@TelefoneComercial", entity.TelefoneComercial),
                        new("@Email", entity.Email),
                        new("@Logradouro", entity.Endereco.Logradouro),
                        new("@Numero", entity.Endereco.Numero),
                        new("@Complemento", entity.Endereco.Complemento),
                        new("@Bairro", entity.Endereco.Bairro),
                        new("@Cidade", entity.Endereco.Cidade),
                        new("@Estado", entity.Endereco.Estado.ToString()),
                        new("@Pais", entity.Endereco.Pais),
                        new("@Cep", entity.Endereco.Cep),
                        new("@Ip", ip),
                    };
                var idMembro = Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, UpdMembroExterno, lstParameters.ToArray()));
                return idMembro;
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, Membro> GetMembroConfirmado(long membroId)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@membroId", membroId)
                };

            var ret = new Dictionary<string, Membro>();
            using (DataSet dr = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsultaMembroConfirmado, lstParameters.ToArray()))
            {
                if (dr.Tables[0].Rows.Count > 0)
                {
                    var membro = new Membro
                    {
                        Id = dr.Tables[0].Rows[0]["Id"].TryConvertTo<int>(),
                        Cpf = dr.Tables[0].Rows[0]["Cpf"].ToString(),
                        Email = dr.Tables[0].Rows[0]["Email"].ToString(),
                        Nacionalidade = dr.Tables[0].Rows[0]["Nacionalidade"].ToString(),
                        NaturalidadeCidade = dr.Tables[0].Rows[0]["NaturalidadeCidade"].ToString(),
                        NaturalidadeEstado = dr.Tables[0].Rows[0]["NaturalidadeEstado"].ToString(),
                        Nome = dr.Tables[0].Rows[0]["Nome"].ToString(),
                        NomeMae = dr.Tables[0].Rows[0]["NomeMae"].ToString(),
                        NomePai = dr.Tables[0].Rows[0]["NomePai"].ToString(),
                        OrgaoEmissor = dr.Tables[0].Rows[0]["OrgaoEmissor"].ToString(),
                        Profissao = dr.Tables[0].Rows[0]["Profissao"].ToString(),
                        RG = dr.Tables[0].Rows[0]["RG"].ToString(),
                        TelefoneCelular = dr.Tables[0].Rows[0]["TelefoneCelular"].ToString(),
                        TelefoneComercial = dr.Tables[0].Rows[0]["TelefoneComercial"].ToString(),
                        TelefoneResidencial = dr.Tables[0].Rows[0]["TelefoneResidencial"].ToString()
                    };
                    if (Enum.TryParse(dr.Tables[0].Rows[0]["Sexo"].ToString(), out Sexo sexo))
                        membro.Sexo = sexo;

                    if (!string.IsNullOrWhiteSpace(dr.Tables[0].Rows[0]["DataAlteracao"].ToString()))
                        membro.DataAlteracao = DateTimeOffset.Parse(dr.Tables[0].Rows[0]["DataAlteracao"].ToString());

                    if (!string.IsNullOrWhiteSpace(dr.Tables[0].Rows[0]["DataNascimento"].ToString()))
                        membro.DataNascimento = DateTimeOffset.Parse(dr.Tables[0].Rows[0]["DataNascimento"].ToString());

                    if (Enum.TryParse(dr.Tables[0].Rows[0]["Escolaridade"].ToString(), out Escolaridade escolaridade))
                        membro.Escolaridade = escolaridade;

                    if (Enum.TryParse(dr.Tables[0].Rows[0]["EstadoCivil"].ToString(), out EstadoCivil estadoCivil))
                        membro.EstadoCivil = estadoCivil;

                    membro.IpConfirmado = dr.Tables[0].Rows[0]["IP"].ToString();

                    membro.Endereco = new Endereco
                    {
                        Logradouro = dr.Tables[0].Rows[0]["Logradouro"].ToString(),
                        Numero = dr.Tables[0].Rows[0]["Numero"].ToString(),
                        Bairro = dr.Tables[0].Rows[0]["Bairro"].ToString(),
                        Cep = dr.Tables[0].Rows[0]["Cep"].ToString(),
                        Cidade = dr.Tables[0].Rows[0]["Cidade"].ToString(),
                        Complemento = dr.Tables[0].Rows[0]["Complemento"].ToString(),
                        Estado = dr.Tables[0].Rows[0]["Estado"].ToString(),
                        Pais = dr.Tables[0].Rows[0]["Pais"].ToString()
                    };

                    ret.Add("H", membro);
                }
                if (dr.Tables[1].Rows.Count > 0)
                {
                    var membro = new Membro
                    {
                        Id = dr.Tables[1].Rows[0]["Id"].TryConvertTo<int>(),
                        Cpf = dr.Tables[1].Rows[0]["Cpf"].ToString(),
                        Email = dr.Tables[1].Rows[0]["Email"].ToString(),
                        Nacionalidade = dr.Tables[1].Rows[0]["Nacionalidade"].ToString(),
                        NaturalidadeCidade = dr.Tables[1].Rows[0]["NaturalidadeCidade"].ToString(),
                        NaturalidadeEstado = dr.Tables[1].Rows[0]["NaturalidadeEstado"].ToString(),
                        Nome = dr.Tables[1].Rows[0]["Nome"].ToString(),
                        NomeMae = dr.Tables[1].Rows[0]["NomeMae"].ToString(),
                        NomePai = dr.Tables[1].Rows[0]["NomePai"].ToString(),
                        OrgaoEmissor = dr.Tables[1].Rows[0]["OrgaoEmissor"].ToString(),
                        Profissao = dr.Tables[1].Rows[0]["Profissao"].ToString(),
                        RG = dr.Tables[1].Rows[0]["RG"].ToString(),
                        TelefoneCelular = dr.Tables[1].Rows[0]["TelefoneCelular"].ToString(),
                        TelefoneComercial = dr.Tables[1].Rows[0]["TelefoneComercial"].ToString(),
                        TelefoneResidencial = dr.Tables[1].Rows[0]["TelefoneResidencial"].ToString()
                    };
                    if (Enum.TryParse(dr.Tables[1].Rows[0]["Sexo"].ToString(), out Sexo sexo))
                        membro.Sexo = sexo;

                    if (!string.IsNullOrWhiteSpace(dr.Tables[1].Rows[0]["DataAlteracao"].ToString()))
                        membro.DataAlteracao = DateTimeOffset.Parse(dr.Tables[1].Rows[0]["DataAlteracao"].ToString());

                    if (!string.IsNullOrWhiteSpace(dr.Tables[1].Rows[0]["DataNascimento"].ToString()))
                        membro.DataNascimento = DateTimeOffset.Parse(dr.Tables[1].Rows[0]["DataNascimento"].ToString());

                    if (Enum.TryParse(dr.Tables[1].Rows[0]["Escolaridade"].ToString(), out Escolaridade escolaridade))
                        membro.Escolaridade = escolaridade;

                    if (Enum.TryParse(dr.Tables[1].Rows[0]["EstadoCivil"].ToString(), out EstadoCivil estadoCivil))
                        membro.EstadoCivil = estadoCivil;

                    membro.Endereco = new Endereco
                    {
                        Logradouro = dr.Tables[1].Rows[0]["Logradouro"].ToString(),
                        Numero = dr.Tables[1].Rows[0]["Numero"].ToString(),
                        Bairro = dr.Tables[1].Rows[0]["Bairro"].ToString(),
                        Cep = dr.Tables[1].Rows[0]["Cep"].ToString(),
                        Cidade = dr.Tables[1].Rows[0]["Cidade"].ToString(),
                        Complemento = dr.Tables[1].Rows[0]["Complemento"].ToString(),
                        Estado = dr.Tables[1].Rows[0]["Estado"].ToString(),
                        Pais = dr.Tables[1].Rows[0]["Pais"].ToString()
                    };

                    ret.Add("A", membro);
                }
                return ret;
            }

        }

        public IEnumerable<Membro> ListarMembroObreiroPaginado(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor,
            int congregacaoId, long usuarioID)
        {
            rowCount = 0;
            List<Membro> lstMembros = new();
            var _campo = !string.IsNullOrWhiteSpace(campo) ? (object)campo : DBNull.Value;
            var _valor = !string.IsNullOrWhiteSpace(valor) ? (object)valor : DBNull.Value;

            var lstParameters = new List<SqlParameter>
                {
                    new("@PAGESIZE", pageSize),
                    new("@ROWSTART", rowStart),
                    new("@SORTING", sorting),
                    new("@USUARIOID", usuarioID),
                    new("@CAMPO", _campo),
                    new("@VALOR", _valor),
                    new("@CONGREGACAOID", congregacaoId)
                };
            try
            {
                using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, MembroObreiroPaginada, lstParameters.ToArray()))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var membro = new Membro()
                            {
                                Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                                Nome = ds.Tables[0].Rows[i]["Nome"].ToString(),
                                NomeMae = ds.Tables[0].Rows[i]["NomeMae"].ToString(),
                                Status = ds.Tables[0].Rows[i]["Status"].ToString().ToEnum<Status>(),
                                Cpf = ds.Tables[0].Rows[i]["Cpf"].ToString()
                            };
                            membro.Congregacao.Nome = ds.Tables[0].Rows[i]["CongregacaoNome"].ToString();

                            lstMembros.Add(membro);
                        }
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        rowCount = Convert.ToInt16(ds.Tables[1].Rows[0][0]);
                    }
                }

            }
            catch
            {
                throw;
            }

            return lstMembros;
        }

        public long RestaurarMembroConfirmado(long membroId, string campos, long usuarioId)
        {
            try
            {
                var lstParameters = new List<SqlParameter>
                    {
                        new("@MembroId", membroId),
                        new("@Campos", campos),
                        new("@UsuarioId", usuarioId)
                    };
                var idMembro = Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, RestaurarMembroConf, lstParameters.ToArray()));
                return idMembro;
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<Membro, string> ListarFotosMembros()
        {
            Dictionary<Membro, string> lstMembros = [];
            try
            {
                using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, "ConsultarMembroFotos"))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var membro = new Membro()
                            {
                                Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                                Nome = ds.Tables[0].Rows[i]["Nome"].ToString(),
                                FotoPath = ds.Tables[0].Rows[i]["FotoPath"].TryConvertTo<string>(),
                            };
                            var tipo = "jpg";
                            if (membro.FotoPath.IndexOf("image/png") > 0)
                                tipo = "png";
                            lstMembros.Add(membro, tipo);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return lstMembros;
        }

        public void AtualizarMembroFotoUrl(long id, string fotoUrl)
        {
            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new("@Id", id),
                    new("@FotoUrl", fotoUrl)
                };
                MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, AtualMembroFotoUrl, lstParameters.ToArray());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> UploadBase64ImageAsync(long membroId, string base64Image, string formato, string fileName)
        {
            // Limpa o hash enviado
            var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(base64Image, "");

            // Gera um array de Bytes
            byte[] imageBytes = Convert.FromBase64String(data);

            //// Define o BLOB no qual a imagem será armazenada
            DeleteImage($"foto_{membroId}.jpg");
            DeleteImage($"foto_{membroId}.png");

            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("fotos");
            await cloudBlobContainer.CreateIfNotExistsAsync();
            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            var fileStream = new MemoryStream(imageBytes);
            if (fileName != null && fileStream != null)
            {
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                cloudBlockBlob.Properties.ContentType = formato;
                await cloudBlockBlob.UploadFromStreamAsync(fileStream);
                return cloudBlockBlob.Uri.AbsoluteUri;
            }
            return "";
        }



        public async void DeleteImage(string fileName)
        {
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("fotos");
            var blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.DeleteIfExistsAsync();
        }

        public IEnumerable<Carteirinha> ListarCarteirinhaMembros(int[] membroId)
        {
            List<Carteirinha> lCarteirinha = [];

            var lstParameters = new List<SqlParameter>
                {
                    new("@ID", string.Join(',', membroId))
                };

            try
            {
                using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ListaCarteirinhaMembros, lstParameters.ToArray()))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var carteirinha = new Carteirinha
                            {
                                Id = Convert.ToInt64(ds.Tables[0].Rows[i]["Id"].ToString()),
                                Nome = ds.Tables[0].Rows[i]["Nome"].TryConvertTo<string>(),
                                NomePai = ds.Tables[0].Rows[i]["NomePai"].TryConvertTo<string>(),
                                NomeMae = ds.Tables[0].Rows[i]["NomeMae"].TryConvertTo<string>(),
                                Cidade = ds.Tables[0].Rows[i]["NaturalidadeCidade"].TryConvertTo<string>(),
                                Estado = ds.Tables[0].Rows[i]["NaturalidadeEstado"].TryConvertTo<string>()
                            };

                            if (ds.Tables[0].Rows[i]["DataNascimento"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue)
                                carteirinha.DataNascimento = ds.Tables[0].Rows[i]["DataNascimento"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy");

                            carteirinha.EstadoCivil = ds.Tables[0].Rows[i]["EstadoCivil"].TryConvertTo<int>() > 0 ? ((EstadoCivil)ds.Tables[0].Rows[i]["EstadoCivil"].TryConvertTo<int>()).GetDisplayAttributeValue() : "";
                            carteirinha.RG = ds.Tables[0].Rows[i]["RG"].TryConvertTo<string>();

                            if (ds.Tables[0].Rows[i]["DataBatismoAguas"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue)
                                carteirinha.DataBatismoAguas = ds.Tables[0].Rows[i]["DataBatismoAguas"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy");

                            carteirinha.FotoUrl = ds.Tables[0].Rows[i]["FotoUrl"].TryConvertTo<string>();
                            carteirinha.TipoCarteirinha = ((TipoCarteirinha)ds.Tables[0].Rows[i]["TipoCarteirinha"].TryConvertTo<int>());
                            carteirinha.LocalConsagracao = ds.Tables[0].Rows[i]["LocalConsagracao"].ToString();

                            if (ds.Tables[0].Rows[i]["DataConsagracao"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue)
                                carteirinha.DataConsagracao = ds.Tables[0].Rows[i]["DataConsagracao"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy");

                            if (ds.Tables[0].Rows[i]["DataValidadeCarteirinha"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue)
                                carteirinha.DataValidadeCarteirinha = ds.Tables[0].Rows[i]["DataValidadeCarteirinha"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy");
                            lCarteirinha.Add(carteirinha);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return lCarteirinha;
        }

        public void AtualizarSenha(long Id, string SenhaAtual, string NovaSenha, bool atualizarSenha, bool atualizarDataInscricao = false)
        {
            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new("@Id", Id),
                    new("@NovaSenha", NovaSenha),
                    new("@atualizarSenha", atualizarSenha),
                    new("@atualizarDtInsc", atualizarDataInscricao)

                };
                MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, AtualSenha, lstParameters.ToArray());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AtualizarEmail(long id, string email)
        {

            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new("@id", id),
                    new("@email", email),

                };
                MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, AtualEmail, lstParameters.ToArray());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool AtualizarMembroAtualizado(long id, bool atualizado)
        {
            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new("@id", id),
                    new("@membroAtualizado", atualizado),

                };
                MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, AtualMembroAtualizado, lstParameters.ToArray());
                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}