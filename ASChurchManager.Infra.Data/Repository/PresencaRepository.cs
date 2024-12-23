using ASBaseLib.Data.Helpers.Microsoft.ApplicationBlocks;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces.Repository;
using ASChurchManager.Domain.Types;
using ASChurchManager.Infra.Data.Repository.EnterpriseLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ASChurchManager.Infra.Data.Repository
{
    public class PresencaRepository : RepositoryDAO<Presenca>, IPresencaRepository
    {
        #region Variaveis Private
        private readonly IConfiguration _configuration;
        private string ConnectionString { get; set; }
        private SqlTransaction transaction;
        private SqlConnection connection;
        #endregion

        #region Constantes
        private const string DelPresenca = "DeletePresenca";
        private const string DelPresencaDatas = "DeletePresencaDatas";
        private const string SavePresenca = "SalvarPresenca";
        private const string SavePresencaDatas = "SalvarPresencaDatas";
        private const string ConsPresenca = "ConsultarPresenca";
        private const string ConsPresencaEmAberto = "ConsultarPresencaEmAberto";
        private const string ListarPresencaPaginada = "ListarPresencaPaginada";
        private const string SavePresencaInscricao = "SalvarPresencaInscricao";
        private const string SavePresencaInscricaoArquivo = "SalvarPresencaInscricaoArquivo";
        private const string DelPresencaInscricao = "DeletePresencaInscricao";
        private const string ConsPresencaMembro = "ConsultarPresencaInscricao";
        private const string ConsPresencaInscricaoPorPresencaId = "ConsultarPresencaInscricaoPorPresencaId";
        private const string ConsPresencaMembroIdCPF = "ConsultarPresencaInscricaoIdCPF";
        private const string UpdPagoPresencaInscricao = "AtualizarPagoPresencaInscricao";
        private const string ConsPresencaDatas = "ConsultarPresencaDatas";
        private const string UpdStatusDatas = "AtualizarStatusDatas";
        private const string ConsPresencaPorStatusData = "ConsultarPresencaPorStatusData";
        private const string SavePresencaInscricaoDatas = "SalvarPresencaInscricaoDatas";
        private const string ConsPresencaInscricaoDatasPorIdEData = "ConsultarPresencaInscricaoDatasPorIdEData";
        private const string ConsPresencaInscricoesDatas = "ConsultarPresencaInscricoesDatas";
        private const string ConsTodasPresenca = "ConsultarTodasPresenca";
        private const string ConsPresencaEtiquetas = "ConsultarPresencaEtiquetas";
        private const string ConsPresencaDatasEmAndamento = "ConsultarPresencaDatasEmAndamento";
        private const string ListarPresencaInscricoesDatas = "ListarPresencaInscricoesDatas";
        private const string ExisInscricaoDatas = "ExisteInscricaoDatas";
        private const string ConsPresencaPorIdData = "ConsultarPresencaPorIdData";
        #endregion

        #region Construtor
        public PresencaRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration["ConnectionStrings:ConnectionDB"];
        }

        public PresencaRepository(string conn)
        {
            ConnectionString = conn;
        }
        #endregion

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

        #region Presenca/PresencaDatas
        public override long Add(Presenca entity, long usuarioID = 0)
        {
            using SqlConnection con = new(ConnectionString);

            con.Open();
            if (transaction == null)
                transaction = con.BeginTransaction();
            try
            {
                var parameters = new List<SqlParameter>
                    {
                        new("@Id", entity.Id),
                        new("@Descricao", entity.Descricao),
                        new("@TipoEventoId", entity.TipoEventoId),
                        new("@DataMaxima", entity.DataMaxima),
                        new("@Valor", entity.Valor),
                        new("@ExclusivoCongregacao", entity.ExclusivoCongregacao),
                        new("@NaoMembros", entity.NaoMembros),
                        new("@GerarEventos", entity.GerarEventos),
                        new("@InscricaoAutomatica", entity.InscricaoAutomatica),
                        new("@CongregacaoId", entity.CongregacaoId),
                        new("@Status", entity.Status)
                    };
                var id = Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, SavePresenca, [.. parameters]));
                entity.Id = id;

                foreach (var item in entity.Datas.Where(d => d.Acao == Acao.Delete))
                {
                    /*Delete tabelas auxiliares*/
                    var paramsId = new List<SqlParameter>()      {
                        new("@DataId", item.Id)
                    };
                    MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, DelPresencaDatas, [.. paramsId]);
                }


                foreach (var item in entity.Datas.Where(d => d.Acao != Acao.Delete))
                {
                    var dataHoraInicio = item.DataHoraInicio == DateTime.MinValue ? (object)DBNull.Value : item.DataHoraInicio;
                    var dataHoraFim = item.DataHoraFim == DateTime.MinValue ? (object)DBNull.Value : item.DataHoraFim;

                    var paramDatas = new List<SqlParameter>
                    {
                        new("@Id", item.Id),
                        new("@PresencaId", id),
                        new("@DataHoraInicio", dataHoraInicio),
                        new("@DataHoraFim", dataHoraFim),
                        new("@CongregacaoId", entity.CongregacaoId),
                        new("@TipoEventoId", entity.TipoEventoId),
                        new("@Descricao", entity.Descricao),
                        new("@GerarEventos", entity.GerarEventos)
                    };

                    MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, SavePresencaDatas, [.. paramDatas]);
                }

                transaction.Commit();
                return id;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public override int Delete(Presenca entity, long usuarioID = 0)
        {
            return Delete(entity.Id, usuarioID);
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@Id", id)
                };

            return Convert.ToInt32(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, DelPresenca, [.. lstParameters])); ;
        }

        public override IEnumerable<Presenca> GetAll(long usuarioID)
        {
            List<Presenca> lPresenca = [];
            var lstParameters = new List<SqlParameter>
                {
                    new("@UsuarioID", usuarioID)
                };
            using DataSet dr = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsTodasPresenca, [.. lstParameters]);
            if (dr.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dr.Tables[0].Rows.Count; i++)
                {
                    var presenca = new Presenca
                    {
                        Id = dr.Tables[0].Rows[i]["Id"].TryConvertTo<int>(),
                        CongregacaoId = Convert.ToInt32(dr.Tables[0].Rows[i]["CongregacaoId"].ToString()),
                        Descricao = dr.Tables[0].Rows[i]["Descricao"].TryConvertTo<string>(),
                        TipoEventoId = dr.Tables[0].Rows[i]["TipoEventoId"].TryConvertTo<int>(),
                        DescrTipoEventoId = dr.Tables[0].Rows[i]["DescrTipoEventoId"].TryConvertTo<string>(),
                        Valor = dr.Tables[0].Rows[i]["Valor"].TryConvertTo<double>(),
                        ExclusivoCongregacao = dr.Tables[0].Rows[i]["ExclusivoCongregacao"].TryConvertTo<bool>(),
                        NaoMembros = dr.Tables[0].Rows[i]["NaoMembros"].TryConvertTo<bool>(),
                        GerarEventos = dr.Tables[0].Rows[i]["GerarEventos"].TryConvertTo<bool>(),
                        InscricaoAutomatica = dr.Tables[0].Rows[i]["InscricaoAutomatica"].TryConvertTo<bool>()
                    };

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<string>()))
                        presenca.DataHoraInicio = dr.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<DateTime>();

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataMaxima"].TryConvertTo<string>()))
                        presenca.DataMaxima = dr.Tables[0].Rows[i]["DataMaxima"].TryConvertTo<DateTime>();

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<string>()))
                        presenca.DataAlteracao = dr.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<DateTime>();
                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<string>()))
                        presenca.DataCriacao = dr.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<DateTime>();

                    if (Enum.TryParse(typeof(StatusPresenca), dr.Tables[0].Rows[i]["Status"].TryConvertTo<string>(), out object statusPres))
                        presenca.Status = (StatusPresenca)statusPres;

                    presenca.Congregacao = new Congregacao()
                    {
                        Id = Convert.ToInt64(dr.Tables[0].Rows[i]["CongregacaoId"].ToString()),
                        Nome = dr.Tables[0].Rows[i]["Congregacao"].TryConvertTo<string>()
                    };

                    if (dr.Tables[1].Rows.Count > 0)
                    {
                        var datas = from row in dr.Tables[1].AsEnumerable()
                                    where row.Field<int>("PresencaId") == presenca.Id
                                    select row;

                        foreach (var item in datas)
                        {
                            var presencadata = new PresencaDatas()
                            {
                                Id = item["Id"].TryConvertTo<int>(),
                                DataHoraInicio = item["DataHoraInicio"].TryConvertTo<DateTime>(),
                                DataHoraFim = item["DataHoraFim"].TryConvertTo<DateTime>(),
                                EventoId = item["EventoId"].TryConvertTo<int>()
                            };
                            if (Enum.TryParse(typeof(StatusPresenca), item["Status"].TryConvertTo<string>(), out object statusData))
                                presenca.Status = (StatusPresenca)statusData;
                            presenca.Datas.Add(presencadata);
                        }
                    }
                    lPresenca.Add(presenca);
                }
            }
            return lPresenca;
        }

        public override Presenca GetById(long id, long usuarioID)
        {
            Presenca presenca = new();
            SqlParameter sqlParameter = new("@Id", id);
            using DataSet dr = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsPresenca, sqlParameter);
            if (dr.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dr.Tables[0].Rows.Count; i++)
                {
                    presenca.Id = dr.Tables[0].Rows[i]["Id"].TryConvertTo<int>();
                    presenca.CongregacaoId = Convert.ToInt32(dr.Tables[0].Rows[i]["CongregacaoId"].ToString());
                    presenca.Descricao = dr.Tables[0].Rows[i]["Descricao"].TryConvertTo<string>();
                    presenca.TipoEventoId = dr.Tables[0].Rows[i]["TipoEventoId"].TryConvertTo<int>();
                    presenca.DescrTipoEventoId = dr.Tables[0].Rows[i]["DescrTipoEventoId"].TryConvertTo<string>();

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<string>()))
                        presenca.DataHoraInicio = dr.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<DateTime>();

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataMaxima"].TryConvertTo<string>()))
                        presenca.DataMaxima = dr.Tables[0].Rows[i]["DataMaxima"].TryConvertTo<DateTime>();

                    presenca.Valor = dr.Tables[0].Rows[i]["Valor"].TryConvertTo<double>();
                    presenca.ExclusivoCongregacao = dr.Tables[0].Rows[i]["ExclusivoCongregacao"].TryConvertTo<bool>();
                    presenca.NaoMembros = dr.Tables[0].Rows[i]["NaoMembros"].TryConvertTo<bool>();
                    presenca.GerarEventos = dr.Tables[0].Rows[i]["GerarEventos"].TryConvertTo<bool>();
                    presenca.InscricaoAutomatica = dr.Tables[0].Rows[i]["InscricaoAutomatica"].TryConvertTo<bool>();

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<string>()))
                        presenca.DataAlteracao = dr.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<DateTime>();
                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<string>()))
                        presenca.DataCriacao = dr.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<DateTime>();

                    if (Enum.TryParse(typeof(StatusPresenca), dr.Tables[0].Rows[i]["Status"].TryConvertTo<string>(), out object status))
                        presenca.Status = (StatusPresenca)status;

                    presenca.Congregacao = new Congregacao()
                    {
                        Id = Convert.ToInt64(dr.Tables[0].Rows[i]["CongregacaoId"].ToString()),
                        Nome = dr.Tables[0].Rows[i]["Congregacao"].TryConvertTo<string>()
                    };
                }
            }
            if (dr.Tables[1].Rows.Count > 0)
            {
                for (int i = 0; i < dr.Tables[1].Rows.Count; i++)
                {
                    var presencadata = new PresencaDatas()
                    {
                        Id = dr.Tables[1].Rows[i]["Id"].TryConvertTo<int>(),
                        DataHoraInicio = dr.Tables[1].Rows[i]["DataHoraInicio"].TryConvertTo<DateTime>(),
                        DataHoraFim = dr.Tables[1].Rows[i]["DataHoraFim"].TryConvertTo<DateTime>(),
                        EventoId = dr.Tables[1].Rows[i]["EventoId"].TryConvertTo<int>()
                    };
                    if (Enum.TryParse(typeof(StatusPresenca), dr.Tables[1].Rows[i]["Status"].TryConvertTo<string>(), out object status))
                        presencadata.Status = (StatusPresenca)status;
                    presenca.Datas.Add(presencadata);
                }
            }
            return presenca;
        }

        public override long Update(Presenca entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Presenca> ListarPresencaPaginado(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor, bool naoMembro, long usuarioID)
        {
            rowCount = 0;
            List<Presenca> lstPresenca = [];

            var lstParameters = new List<SqlParameter>
                {
                    new("@PAGESIZE", pageSize),
                    new("@ROWSTART", rowStart),
                    new("@SORTING", sorting),
                    new("@USUARIOID", usuarioID),
                    new("@CAMPO", campo),
                    new("@VALOR", valor),
                    new("@NAOMEMBRO", naoMembro)

                };

            using DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ListarPresencaPaginada, [.. lstParameters]);
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var presenca = new Presenca()
                    {
                        Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                        Descricao = ds.Tables[0].Rows[i]["Descricao"].ToString(),
                        DescrTipoEventoId = ds.Tables[0].Rows[i]["TipoEvento"].ToString(),
                        Status = ds.Tables[0].Rows[i]["Status"].ToString().ToEnum<StatusPresenca>(),
                    };
                    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["DataMaxima"].TryConvertTo<string>()))
                        presenca.DataMaxima = ds.Tables[0].Rows[i]["DataMaxima"].TryConvertTo<DateTime>();

                    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<string>()))
                        presenca.DataHoraInicio = ds.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<DateTime>();

                    presenca.Congregacao.Nome = ds.Tables[0].Rows[i]["Congregacao"].ToString();

                    lstPresenca.Add(presenca);
                }
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                rowCount = Convert.ToInt16(ds.Tables[1].Rows[0][0]);
            }

            return lstPresenca;
        }

        public IEnumerable<Presenca> ListarPresencaEmAberto(long usuarioID)
        {
            List<Presenca> ret = [];

            SqlParameter sqlParameter = new("@UsuarioID", usuarioID);
            using DataSet dr = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsPresencaEmAberto, sqlParameter);
            if (dr.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dr.Tables[0].Rows.Count; i++)
                {
                    Presenca presenca = new()
                    {
                        Id = dr.Tables[0].Rows[i]["Id"].TryConvertTo<int>(),
                        CongregacaoId = Convert.ToInt32(dr.Tables[0].Rows[i]["CongregacaoId"].ToString()),
                        Descricao = dr.Tables[0].Rows[i]["Descricao"].TryConvertTo<string>(),
                        TipoEventoId = dr.Tables[0].Rows[i]["TipoEventoId"].TryConvertTo<int>(),
                        DescrTipoEventoId = dr.Tables[0].Rows[i]["DescrTipoEventoId"].TryConvertTo<string>(),
                        Valor = dr.Tables[0].Rows[i]["Valor"].TryConvertTo<double>(),
                        ExclusivoCongregacao = dr.Tables[0].Rows[i]["ExclusivoCongregacao"].TryConvertTo<bool>(),
                        NaoMembros = dr.Tables[0].Rows[i]["NaoMembros"].TryConvertTo<bool>(),
                        GerarEventos = dr.Tables[0].Rows[i]["GerarEventos"].TryConvertTo<bool>(),
                        InscricaoAutomatica = dr.Tables[0].Rows[i]["InscricaoAutomatica"].TryConvertTo<bool>()
                    };

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataMaxima"].TryConvertTo<string>()))
                        presenca.DataMaxima = dr.Tables[0].Rows[i]["DataMaxima"].TryConvertTo<DateTime>();

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<string>()))
                        presenca.DataHoraInicio = dr.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<DateTime>();

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<string>()))
                        presenca.DataAlteracao = dr.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<DateTime>();

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<string>()))
                        presenca.DataCriacao = dr.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<DateTime>();

                    if (Enum.TryParse(typeof(StatusPresenca), dr.Tables[0].Rows[i]["Status"].TryConvertTo<string>(), out object status))
                        presenca.Status = (StatusPresenca)status;

                    presenca.Congregacao = new Congregacao()
                    {
                        Id = Convert.ToInt64(dr.Tables[0].Rows[i]["CongregacaoId"].ToString()),
                        Nome = dr.Tables[0].Rows[i]["Congregacao"].TryConvertTo<string>()
                    };
                    ret.Add(presenca);
                }
            }

            return ret;
        }

        public List<PresencaDatas> ListarPresencaDatas(long idPresenca)
        {
            List<PresencaDatas> presenca = [];

            SqlParameter sqlParameter = new("@PresencaId", idPresenca);
            using DataSet dr = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsPresencaDatas, sqlParameter);
            if (dr.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dr.Tables[0].Rows.Count; i++)
                {
                    var presencadata = new PresencaDatas()
                    {
                        Id = dr.Tables[0].Rows[i]["Id"].TryConvertTo<int>(),
                        DataHoraInicio = dr.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<DateTime>(),
                        DataHoraFim = dr.Tables[0].Rows[i]["DataHoraFim"].TryConvertTo<DateTime>(),
                        EventoId = dr.Tables[0].Rows[i]["EventoId"].TryConvertTo<int>(),
                    };
                    if (Enum.TryParse(typeof(StatusPresenca), dr.Tables[0].Rows[i]["Status"].TryConvertTo<string>(), out object status))
                        presencadata.Status = (StatusPresenca)status;

                    presenca.Add(presencadata);
                }
            }

            return presenca;

        }

        public int AtualizarStatusData(int idData, StatusPresenca status)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@DataId", idData),
                    new("@Status", status)
                };

            return Convert.ToInt32(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, UpdStatusDatas, [.. lstParameters])); ;
        }
        #endregion

        #region Inscrições
        public int SalvarInscricao(PresencaMembro entity)
        {
            var parameters = new List<SqlParameter>
                    {
                        new("@Id", entity.Id),
                        new("@PresencaId", entity.PresencaId),
                        new("@MembroId", entity.MembroId),
                        new("@Nome", entity.Nome),
                        new("@CPF", entity.CPF),
                        new("@Igreja", entity.Igreja),
                        new("@Cargo", entity.Cargo),
                        new("@Pago", entity.Pago),
                        new("@Usuario", entity.Usuario),
                        new("@ArquivoId", entity.ArquivoId),
                        new("@CongregacaoId", entity.CongregacaoId)
                    };
            var id = Convert.ToInt32(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, SavePresencaInscricao, [.. parameters]));
            entity.Id = id;
            return id;
        }

        public List<PresencaMembro> ConsultarPresencaInscricaoPorPresencaId(long idPresenca, long usuarioID)
        {
            List<PresencaMembro> lstPresenca = [];

            var lstParameters = new List<SqlParameter>
                {
                    new("@PresencaId", idPresenca),
                    new("@UsuarioId", usuarioID)
                };

            using DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsPresencaInscricaoPorPresencaId, [.. lstParameters]);
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var presenca = new PresencaMembro()
                    {
                        Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                        PresencaId = Convert.ToInt16(ds.Tables[0].Rows[i]["PresencaId"]),
                        MembroId = Convert.ToInt16(ds.Tables[0].Rows[i]["MembroId"]),
                        Nome = ds.Tables[0].Rows[i]["Nome"].ToString(),
                        CPF = ds.Tables[0].Rows[i]["CPF"].ToString(),
                        CongregacaoId = ds.Tables[0].Rows[i]["CongregacaoId"].TryConvertTo<int>(),
                        Igreja = ds.Tables[0].Rows[i]["Igreja"].ToString(),
                        Cargo = ds.Tables[0].Rows[i]["Cargo"].ToString(),
                        Pago = ds.Tables[0].Rows[i]["Pago"].TryConvertTo<bool>(),
                        Usuario = Convert.ToInt16(ds.Tables[0].Rows[i]["Usuario"])
                    };

                    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<string>()))
                        presenca.DataAlteracao = ds.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<DateTime>();
                    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<string>()))
                        presenca.DataCriacao = ds.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<DateTime>();

                    lstPresenca.Add(presenca);
                }
            }
            return lstPresenca;
        }

        public PresencaMembro ConsultarPresencaInscricao(long idInscricao)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@InscricaoId", idInscricao)
                };

            using DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsPresencaMembro, [.. lstParameters]);
            if (ds.Tables[0].Rows.Count > 0)
            {
                var presenca = new PresencaMembro()
                {
                    Id = Convert.ToInt16(ds.Tables[0].Rows[0]["Id"]),
                    PresencaId = Convert.ToInt16(ds.Tables[0].Rows[0]["PresencaId"]),
                    MembroId = Convert.ToInt16(ds.Tables[0].Rows[0]["MembroId"]),
                    Nome = ds.Tables[0].Rows[0]["Nome"].ToString(),
                    CPF = ds.Tables[0].Rows[0]["CPF"].ToString(),
                    CongregacaoId = ds.Tables[0].Rows[0]["CongregacaoId"].TryConvertTo<int>(),
                    Igreja = ds.Tables[0].Rows[0]["Igreja"].ToString(),
                    Cargo = ds.Tables[0].Rows[0]["Cargo"].ToString(),
                    Pago = ds.Tables[0].Rows[0]["Pago"].TryConvertTo<bool>(),
                    Usuario = Convert.ToInt16(ds.Tables[0].Rows[0]["Usuario"])
                };

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["DataAlteracao"].TryConvertTo<string>()))
                    presenca.DataAlteracao = ds.Tables[0].Rows[0]["DataAlteracao"].TryConvertTo<DateTime>();
                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["DataCriacao"].TryConvertTo<string>()))
                    presenca.DataCriacao = ds.Tables[0].Rows[0]["DataCriacao"].TryConvertTo<DateTime>();

                return presenca;
            }

            return null;
        }

        public PresencaMembro ConsultarPresencaInscricao(long idPresenca, long idMembro, string cpf, long usuarioID)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@PresencaId", idPresenca),
                    new("@MembroId", idMembro),
                    new("@CPF", cpf),
                    new("@UsuarioId", usuarioID)

                };

            using DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsPresencaMembroIdCPF, [.. lstParameters]);
            if (ds.Tables[0].Rows.Count > 0)
            {
                var presenca = new PresencaMembro()
                {
                    Id = Convert.ToInt16(ds.Tables[0].Rows[0]["Id"]),
                    PresencaId = Convert.ToInt16(ds.Tables[0].Rows[0]["PresencaId"]),
                    MembroId = Convert.ToInt16(ds.Tables[0].Rows[0]["MembroId"]),
                    Nome = ds.Tables[0].Rows[0]["Nome"].ToString(),
                    CPF = ds.Tables[0].Rows[0]["CPF"].ToString(),
                    CongregacaoId = ds.Tables[0].Rows[0]["CongregacaoId"].TryConvertTo<int>(),
                    Igreja = ds.Tables[0].Rows[0]["Igreja"].ToString(),
                    Cargo = ds.Tables[0].Rows[0]["Cargo"].ToString(),
                    Pago = ds.Tables[0].Rows[0]["Pago"].TryConvertTo<bool>(),
                    Usuario = Convert.ToInt16(ds.Tables[0].Rows[0]["Usuario"])
                };

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["DataAlteracao"].TryConvertTo<string>()))
                    presenca.DataAlteracao = ds.Tables[0].Rows[0]["DataAlteracao"].TryConvertTo<DateTime>();
                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["DataCriacao"].TryConvertTo<string>()))
                    presenca.DataCriacao = ds.Tables[0].Rows[0]["DataCriacao"].TryConvertTo<DateTime>();
                return presenca;
            }

            return null;
        }

        public PresencaMembro ConsultarPresencaInscricaoDatas(long idInscricao, int idData)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@InscricaoId", idInscricao),
                    new("@DataId", idData)
                };

            using DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsPresencaInscricaoDatasPorIdEData, [.. lstParameters]);
            if (ds.Tables[0].Rows.Count > 0)
            {
                var presenca = new PresencaMembro()
                {
                    Id = Convert.ToInt16(ds.Tables[0].Rows[0]["Id"]),
                    PresencaId = Convert.ToInt16(ds.Tables[0].Rows[0]["PresencaId"]),
                    MembroId = Convert.ToInt16(ds.Tables[0].Rows[0]["MembroId"]),
                    Nome = ds.Tables[0].Rows[0]["Nome"].ToString(),
                    CPF = ds.Tables[0].Rows[0]["CPF"].ToString(),
                    CongregacaoId = ds.Tables[0].Rows[0]["CongregacaoId"].TryConvertTo<int>(),
                    Igreja = ds.Tables[0].Rows[0]["Igreja"].ToString(),
                    Cargo = ds.Tables[0].Rows[0]["Cargo"].ToString(),
                    Pago = ds.Tables[0].Rows[0]["Pago"].TryConvertTo<bool>(),
                    Usuario = Convert.ToInt16(ds.Tables[0].Rows[0]["Usuario"])
                };

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["DataAlteracao"].TryConvertTo<string>()))
                    presenca.DataAlteracao = ds.Tables[0].Rows[0]["DataAlteracao"].TryConvertTo<DateTime>();
                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["DataCriacao"].TryConvertTo<string>()))
                    presenca.DataCriacao = ds.Tables[0].Rows[0]["DataCriacao"].TryConvertTo<DateTime>();
                if (Enum.TryParse(typeof(SituacaoPresenca), ds.Tables[0].Rows[0]["Situacao"].TryConvertTo<string>(), out object situacao))
                    presenca.Situacao = (SituacaoPresenca)situacao;
                if (Enum.TryParse(typeof(TipoRegistro), ds.Tables[0].Rows[0]["Tipo"].TryConvertTo<string>(), out object tipo))
                    presenca.Tipo = (TipoRegistro)tipo;

                return presenca;
            }

            return null;
        }

        public List<PresencaMembro> ConsultarPresencaInscricoesDatas(int idPresenca, int idData)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@PresencaId", idPresenca),
                    new("@DataId", idData)
                };

            var lpresenca = new List<PresencaMembro>();
            using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsPresencaInscricoesDatas, [.. lstParameters]))
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var presenca = new PresencaMembro()
                        {
                            Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                            PresencaId = Convert.ToInt16(ds.Tables[0].Rows[i]["PresencaId"]),
                            MembroId = Convert.ToInt16(ds.Tables[0].Rows[i]["MembroId"]),
                            Nome = ds.Tables[0].Rows[i]["Nome"].ToString(),
                            CPF = ds.Tables[0].Rows[i]["CPF"].ToString(),
                            CongregacaoId = ds.Tables[0].Rows[i]["CongregacaoId"].TryConvertTo<int>(),
                            Igreja = ds.Tables[0].Rows[i]["Igreja"].ToString(),
                            Cargo = ds.Tables[0].Rows[i]["Cargo"].ToString(),
                            Pago = ds.Tables[0].Rows[i]["Pago"].TryConvertTo<bool>(),
                            Usuario = Convert.ToInt16(ds.Tables[0].Rows[i]["Usuario"]),
                            Justificativa = ds.Tables[0].Rows[i]["Justificativa"].ToString()
                        };

                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<string>()))
                            presenca.DataAlteracao = ds.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<DateTime>();
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<string>()))
                            presenca.DataCriacao = ds.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<DateTime>();
                        if (Enum.TryParse(typeof(SituacaoPresenca), ds.Tables[0].Rows[i]["Situacao"].TryConvertTo<string>(), out object situacao))
                            presenca.Situacao = (SituacaoPresenca)situacao;
                        if (Enum.TryParse(typeof(TipoRegistro), ds.Tables[0].Rows[i]["Tipo"].TryConvertTo<string>(), out object tipo))
                            presenca.Tipo = (TipoRegistro)tipo;

                        lpresenca.Add(presenca);
                    }
                }
            }
            return lpresenca;
        }
        public int AtualizarPagoInscricao(int id, bool pago)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@Id", id),
                    new("@Pago", pago)
                };

            return Convert.ToInt32(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, UpdPagoPresencaInscricao, [.. lstParameters]));
        }

        public int SalvarInscricaoArquivo(string nomeArquivo)
        {
            int idArquivo = 0;
            if (!string.IsNullOrWhiteSpace(nomeArquivo))
            {
                var paramArq = new List<SqlParameter>
                    {
                        new("@NomeArquivo", nomeArquivo)
                    };
                idArquivo = Convert.ToInt32(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, SavePresencaInscricaoArquivo, [.. paramArq]));
            }
            return idArquivo;
        }

        public IEnumerable<Presenca> ConsultarPresencaPorStatusData(int id, StatusPresenca status)
        {
            List<Presenca> lPresenca = [];

            var lstParameters = new List<SqlParameter>
                {
                    new("@Id", id),
                    new("@Status", status)
                };
            using DataSet dr = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsPresencaPorStatusData, [.. lstParameters]);
            if (dr.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dr.Tables[0].Rows.Count; i++)
                {
                    var presenca = new Presenca
                    {
                        Id = dr.Tables[0].Rows[i]["Id"].TryConvertTo<int>(),
                        CongregacaoId = Convert.ToInt32(dr.Tables[0].Rows[i]["CongregacaoId"].ToString()),
                        Descricao = dr.Tables[0].Rows[i]["Descricao"].TryConvertTo<string>(),
                        TipoEventoId = dr.Tables[0].Rows[i]["TipoEventoId"].TryConvertTo<int>(),
                        DescrTipoEventoId = dr.Tables[0].Rows[i]["DescrTipoEventoId"].TryConvertTo<string>(),
                        Valor = dr.Tables[0].Rows[i]["Valor"].TryConvertTo<double>(),
                        ExclusivoCongregacao = dr.Tables[0].Rows[i]["ExclusivoCongregacao"].TryConvertTo<bool>(),
                        NaoMembros = dr.Tables[0].Rows[i]["NaoMembros"].TryConvertTo<bool>(),
                        GerarEventos = dr.Tables[0].Rows[i]["GerarEventos"].TryConvertTo<bool>(),
                        InscricaoAutomatica = dr.Tables[0].Rows[i]["InscricaoAutomatica"].TryConvertTo<bool>()
                    };

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<string>()))
                        presenca.DataHoraInicio = dr.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<DateTime>();

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataMaxima"].TryConvertTo<string>()))
                        presenca.DataMaxima = dr.Tables[0].Rows[i]["DataMaxima"].TryConvertTo<DateTime>();

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<string>()))
                        presenca.DataAlteracao = dr.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<DateTime>();
                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<string>()))
                        presenca.DataCriacao = dr.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<DateTime>();

                    if (Enum.TryParse(typeof(StatusPresenca), dr.Tables[0].Rows[i]["Status"].TryConvertTo<string>(), out object statusPres))
                        presenca.Status = (StatusPresenca)statusPres;

                    presenca.Congregacao = new Congregacao()
                    {
                        Id = Convert.ToInt64(dr.Tables[0].Rows[i]["CongregacaoId"].ToString()),
                        Nome = dr.Tables[0].Rows[i]["Congregacao"].TryConvertTo<string>()
                    };

                    if (dr.Tables[1].Rows.Count > 0)
                    {
                        var datas = from row in dr.Tables[1].AsEnumerable()
                                    where row.Field<int>("PresencaId") == presenca.Id
                                    select row;

                        foreach (var item in datas)
                        {
                            var presencadata = new PresencaDatas()
                            {
                                Id = item["Id"].TryConvertTo<int>(),
                                DataHoraInicio = item["DataHoraInicio"].TryConvertTo<DateTime>(),
                                DataHoraFim = item["DataHoraFim"].TryConvertTo<DateTime>(),
                                EventoId = item["EventoId"].TryConvertTo<int>()
                            };

                            if (Enum.TryParse(typeof(StatusPresenca), item["Status"].TryConvertTo<string>(), out object statusData))
                                presencadata.Status = (StatusPresenca)statusData;
                            presenca.Datas.Add(presencadata);
                        }
                    }
                    lPresenca.Add(presenca);
                }
            }
            return lPresenca;
        }

        public int SalvarPresencaInscricaoDatas(int idInscricao, int idData, SituacaoPresenca situacao, TipoRegistro tipo, string justificativa = "")
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@InscricaoId", idInscricao),
                    new("@DataId", idData),
                    new("@Situacao", situacao),
                    new("@Tipo", tipo),
                    new("@Justificativa", justificativa)
                };

            return Convert.ToInt32(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, SavePresencaInscricaoDatas, [.. lstParameters]));
        }

        public Task<int> DeleteInscricaoAsync(int id)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@Id", id)
                };

            var ret = Convert.ToInt32(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, DelPresencaInscricao, [.. lstParameters]));
            return Task.FromResult(ret);
        }

        public List<PresencaMembro> ConsultarPresencaEtiquetas(int idInscricao, int idCongregacao, int tipo, int membroId, string cpf, long usuarioId)
        {
            List<PresencaMembro> membro = [];

            var lstParameters = new List<SqlParameter>
                {
                    new("@PresencaId", idInscricao),
                    new("@CongregacaoId", idCongregacao),
                    new("@Tipo", tipo),
                    new("@MembroId", membroId),
                    new("@CPF", cpf),
                    new("@UsuarioId", usuarioId)
                };

            using DataSet dr = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsPresencaEtiquetas, [.. lstParameters]);
            if (dr.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dr.Tables[0].Rows.Count; i++)
                {
                    var presenca = new PresencaMembro
                    {
                        Id = dr.Tables[0].Rows[i]["Id"].TryConvertTo<int>(),
                        MembroId = dr.Tables[0].Rows[i]["MembroId"].TryConvertTo<int>(),
                        Nome = dr.Tables[0].Rows[i]["Nome"].TryConvertTo<string>(),
                        Igreja = dr.Tables[0].Rows[i]["Igreja"].TryConvertTo<string>(),
                        Cargo = dr.Tables[0].Rows[i]["Cargo"].TryConvertTo<string>(),
                    };

                    membro.Add(presenca);
                }
            }

            return membro;
        }

        public List<PresencaDatas> ListarPresencaDatasEmAndamento()
        {
            var lista = new List<PresencaDatas>();


            using (DataSet dr = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsPresencaDatasEmAndamento))
            {
                if (dr.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dr.Tables[0].Rows.Count; i++)
                    {
                        var presencadata = new PresencaDatas()
                        {
                            Id = dr.Tables[0].Rows[i]["Id"].TryConvertTo<int>(),
                            DataHoraInicio = dr.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<DateTime>(),
                            DataHoraFim = dr.Tables[0].Rows[i]["DataHoraFim"].TryConvertTo<DateTime>(),
                            EventoId = dr.Tables[0].Rows[i]["EventoId"].TryConvertTo<int>()
                        };

                        if (Enum.TryParse(typeof(StatusPresenca), dr.Tables[0].Rows[i]["Status"].TryConvertTo<string>(), out object statusData))
                            presencadata.Status = (StatusPresenca)statusData;
                        lista.Add(presencadata);
                    }
                }
            }
            return lista;

        }

        public IEnumerable<PresencaMembro> ListarPresencaDatasPaginado(int pageSize, int rowStart, out int rowCount, string sorting,
            int idPresenca, int idData, string campo, string valor, long usuarioID)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@PresencaId", idPresenca),
                    new("@DataId", idData),
                    new("@PAGESIZE", pageSize),
                    new("@ROWSTART", rowStart),
                    new("@SORTING", sorting),
                    new("@USUARIOID", usuarioID),
                    new("@CAMPO", campo),
                    new("@VALOR", valor)
                };

            var lpresenca = new List<PresencaMembro>();
            rowCount = 0;

            using DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ListarPresencaInscricoesDatas,
                [.. lstParameters]);
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var presenca = new PresencaMembro()
                    {
                        Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                        PresencaId = Convert.ToInt16(ds.Tables[0].Rows[i]["PresencaId"]),
                        MembroId = Convert.ToInt16(ds.Tables[0].Rows[i]["MembroId"]),
                        Nome = ds.Tables[0].Rows[i]["Nome"].ToString(),
                        CPF = ds.Tables[0].Rows[i]["CPF"].ToString(),
                        CongregacaoId = ds.Tables[0].Rows[i]["CongregacaoId"].TryConvertTo<int>(),
                        Igreja = ds.Tables[0].Rows[i]["Igreja"].ToString(),
                        Cargo = ds.Tables[0].Rows[i]["Cargo"].ToString(),
                        Pago = ds.Tables[0].Rows[i]["Pago"].TryConvertTo<bool>(),
                        Usuario = Convert.ToInt16(ds.Tables[0].Rows[i]["Usuario"]),
                        Justificativa = ds.Tables[0].Rows[i]["Justificativa"].ToString()
                    };

                    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<string>()))
                        presenca.DataAlteracao = ds.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<DateTime>();
                    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<string>()))
                        presenca.DataCriacao = ds.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<DateTime>();
                    if (Enum.TryParse(typeof(SituacaoPresenca), ds.Tables[0].Rows[i]["Situacao"].TryConvertTo<string>(), out object situacao))
                        presenca.Situacao = (SituacaoPresenca)situacao;
                    if (Enum.TryParse(typeof(TipoRegistro), ds.Tables[0].Rows[i]["Tipo"].TryConvertTo<string>(), out object tipo))
                        presenca.Tipo = (TipoRegistro)tipo;
                    if (Enum.TryParse(typeof(StatusPresenca), ds.Tables[0].Rows[i]["StatusData"].TryConvertTo<string>(), out object status))
                        presenca.StatusData = (StatusPresenca)status;

                    lpresenca.Add(presenca);
                }
            }

            if (ds.Tables[1].Rows.Count > 0)
            {
                rowCount = Convert.ToInt16(ds.Tables[1].Rows[0][0]);
            }
            return lpresenca;

        }

        public bool ExisteInscricaoDatas(int idData)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@Id", idData)
                };

            using DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ExisInscricaoDatas, [.. lstParameters]);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt16(ds.Tables[0].Rows[0]["EXISTEINSCRICAO"]) > 0;
            }

            return false;
        }

        public IEnumerable<Presenca> ConsultarPresencaIdData(int idData)
        {
            List<Presenca> lPresenca = [];

            var lstParameters = new List<SqlParameter>
                {
                    new("@IdData", idData)
                };
            using DataSet dr = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsPresencaPorIdData, [.. lstParameters]);
            if (dr.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dr.Tables[0].Rows.Count; i++)
                {
                    var presenca = new Presenca
                    {
                        Id = dr.Tables[0].Rows[i]["Id"].TryConvertTo<int>(),
                        CongregacaoId = Convert.ToInt32(dr.Tables[0].Rows[i]["CongregacaoId"].ToString()),
                        Descricao = dr.Tables[0].Rows[i]["Descricao"].TryConvertTo<string>(),
                        TipoEventoId = dr.Tables[0].Rows[i]["TipoEventoId"].TryConvertTo<int>(),
                        DescrTipoEventoId = dr.Tables[0].Rows[i]["DescrTipoEventoId"].TryConvertTo<string>(),
                        Valor = dr.Tables[0].Rows[i]["Valor"].TryConvertTo<double>(),
                        ExclusivoCongregacao = dr.Tables[0].Rows[i]["ExclusivoCongregacao"].TryConvertTo<bool>(),
                        NaoMembros = dr.Tables[0].Rows[i]["NaoMembros"].TryConvertTo<bool>(),
                        GerarEventos = dr.Tables[0].Rows[i]["GerarEventos"].TryConvertTo<bool>(),
                        InscricaoAutomatica = dr.Tables[0].Rows[i]["InscricaoAutomatica"].TryConvertTo<bool>()
                    };

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<string>()))
                        presenca.DataHoraInicio = dr.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<DateTime>();

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataMaxima"].TryConvertTo<string>()))
                        presenca.DataMaxima = dr.Tables[0].Rows[i]["DataMaxima"].TryConvertTo<DateTime>();

                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<string>()))
                        presenca.DataAlteracao = dr.Tables[0].Rows[i]["DataAlteracao"].TryConvertTo<DateTime>();
                    if (!string.IsNullOrEmpty(dr.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<string>()))
                        presenca.DataCriacao = dr.Tables[0].Rows[i]["DataCriacao"].TryConvertTo<DateTime>();

                    if (Enum.TryParse(typeof(StatusPresenca), dr.Tables[0].Rows[i]["Status"].TryConvertTo<string>(), out object statusPres))
                        presenca.Status = (StatusPresenca)statusPres;

                    presenca.Congregacao = new Congregacao()
                    {
                        Id = Convert.ToInt64(dr.Tables[0].Rows[i]["CongregacaoId"].ToString()),
                        Nome = dr.Tables[0].Rows[i]["Congregacao"].TryConvertTo<string>()
                    };

                    if (dr.Tables[1].Rows.Count > 0)
                    {
                        var datas = from row in dr.Tables[1].AsEnumerable()
                                    where row.Field<int>("PresencaId") == presenca.Id
                                    select row;

                        foreach (var item in datas)
                        {
                            var presencadata = new PresencaDatas()
                            {
                                Id = item["Id"].TryConvertTo<int>(),
                                DataHoraInicio = item["DataHoraInicio"].TryConvertTo<DateTime>(),
                                DataHoraFim = item["DataHoraFim"].TryConvertTo<DateTime>(),
                                EventoId = item["EventoId"].TryConvertTo<int>()
                            };

                            if (Enum.TryParse(typeof(StatusPresenca), item["Status"].TryConvertTo<string>(), out object statusData))
                                presencadata.Status = (StatusPresenca)statusData;
                            presenca.Datas.Add(presencadata);
                        }
                    }
                    lPresenca.Add(presenca);
                }
            }

            return lPresenca;
        }
        #endregion
    }
}
