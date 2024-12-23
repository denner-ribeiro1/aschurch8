using ASBaseLib.Data.DataMapping;
using ASBaseLib.Data.Helpers.Microsoft.ApplicationBlocks;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Infra.Data.Repository.EnterpriseLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ASChurchManager.Infra.Data.Repository
{
    public class CasamentoRepository : RepositoryDAO<Casamento>, ICasamentoRepository
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        public CasamentoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        #region Privates
        private const string SalvarCasamento = "dbo.SalvarCasamento";
        private const string ListarCasamento = "dbo.ListarCasamento";
        private const string ConsultarCasamento = "dbo.ConsultarCasamento";
        private const string DeletarCasamento = "dbo.DeletarCasamento";
        private const string ListarCasamentoAgendadoCongregacao = "dbo.ListarCasamentoAgendadoCongregacao";
        private const string CasamentoPaginada = "dbo.ListarCasamentoPaginada";
        #endregion

        #region Publicos
        public override long Add(Casamento entity, long usuarioID = 0)
        {
            try
            {
                var lstParans = new List<SqlParameter>
                {
                    new SqlParameter("@Id", entity.Id),
                    new SqlParameter("@CongregacaoId", entity.CongregacaoId),
                    new SqlParameter("@PastorId", entity.PastorId),
                    new SqlParameter("@PastorNome", entity.PastorNome),
                    new SqlParameter("@DataHoraInicio", entity.DataHoraInicio),
                    new SqlParameter("@DataHoraFinal", entity.DataHoraFinal),
                    new SqlParameter("@NoivoId", entity.NoivoId),
                    new SqlParameter("@NoivoNome", entity.NoivoNome),
                    new SqlParameter("@PaiNoivoId", entity.PaiNoivoId),
                    new SqlParameter("@PaiNoivoNome", entity.PaiNoivoNome),
                    new SqlParameter("@MaeNoivoId", entity.MaeNoivoId),
                    new SqlParameter("@MaeNoivoNome", entity.MaeNoivoNome),
                    new SqlParameter("@NoivaId", entity.NoivaId),
                    new SqlParameter("@NoivaNome", entity.NoivaNome),
                    new SqlParameter("@PaiNoivaId", entity.PaiNoivaId),
                    new SqlParameter("@PaiNoivaNome", entity.PaiNoivaNome),
                    new SqlParameter("@MaeNoivaId", entity.MaeNoivaId),
                    new SqlParameter("@MaeNoivaNome", entity.MaeNoivaNome),
                };

                return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, SalvarCasamento,
                    lstParans.ToArray()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override long Update(Casamento entity, long usuarioID = 0)
        {
            try
            {
                return Add(entity);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public override int Delete(Casamento entity, long usuarioID = 0)
        {
            try
            {
                var sqlParameter = new SqlParameter("@Id", entity.Id);
                return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, DeletarCasamento, sqlParameter));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            try
            {
                var sqlParameter = new SqlParameter("@Id", id);

                return MicrosoftSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, DeletarCasamento, sqlParameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override Casamento GetById(long id, long usuarioID)
        {
            var casamento = new Casamento();
            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@UsuarioID", usuarioID)
                };

                using (var dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ConsultarCasamento, lstParameters.ToArray()))
                {
                    while (dr.Read())
                    {
                        casamento = DataMapper.ExecuteMapping<Casamento>(dr);
                        casamento.CongregacaoId = Convert.ToInt16(dr["CongregacaoId"].ToString());
                        casamento.Congregacao.Id = Convert.ToInt64(dr["CongregacaoId"].ToString());
                        casamento.Congregacao.Nome = dr["CongregacaoNome"].TryConvertTo<string>();
                        casamento.Congregacao.CongregacaoResponsavelId = Convert.ToInt64(dr["CongregacaoResponsavelId"].ToString());
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return casamento;
        }

        public override IEnumerable<Casamento> GetAll(long usuarioID)
        {
            var lstCasamento = new List<Casamento>();

            var lstParameters = new List<SqlParameter>
                {
                    new SqlParameter("@UsuarioID", usuarioID)
                };
            try
            {
                using (var dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarCasamento, lstParameters.ToArray()))
                {
                    while (dr.Read())
                    {
                        var casamento = DataMapper.ExecuteMapping<Casamento>(dr);
                        casamento.Congregacao.Id = Convert.ToInt64(dr["CongregacaoId"].ToString());
                        casamento.Congregacao.Nome = dr["CongregacaoNome"].TryConvertTo<string>();
                        lstCasamento.Add(casamento);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lstCasamento;
        }

        public Casamento VerificarCasamentoCongregacao(Casamento casamento)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", casamento.Id),
                    new SqlParameter("@CongregacaoId", casamento.CongregacaoId),
                    new SqlParameter("@DataHoraInicio", casamento.DataHoraInicio),
                    new SqlParameter("@DataHoraFinal", casamento.DataHoraFinal)
                };

            try
            {
                using (var dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarCasamentoAgendadoCongregacao, lstParameters.ToArray()))
                {
                    if (dr.Read())
                    {
                        var ret = DataMapper.ExecuteMapping<Casamento>(dr);
                        ret.Congregacao.Id = Convert.ToInt64(dr["CongregacaoId"].ToString());
                        ret.Congregacao.Nome = dr["CongregacaoNome"].TryConvertTo<string>();
                        return ret;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return new Casamento();
        }

        public IEnumerable<Casamento> ListarCasamentoPaginado(int pageSize, int rowStart, string sorting, string campo, string valor, long usuarioID, out int rowCount)
        {
            rowCount = 0;
            List<Casamento> lCasam = new List<Casamento>();
            var _campo = !string.IsNullOrWhiteSpace(campo) ? (object)campo : DBNull.Value;
            var _valor = !string.IsNullOrWhiteSpace(valor) ? (object)valor : DBNull.Value;

            var lstParameters = new List<SqlParameter>
                {
                    new SqlParameter("@PAGESIZE", pageSize),
                    new SqlParameter("@ROWSTART", rowStart),
                    new SqlParameter("@SORTING", sorting),
                    new SqlParameter("@USUARIOID", usuarioID),
                    new SqlParameter("@CAMPO", _campo),
                    new SqlParameter("@VALOR", _valor)
            };
            try
            {
                using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, CasamentoPaginada, lstParameters.ToArray()))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var casam = new Casamento()
                            {
                                Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                                DataHoraInicio = Convert.ToDateTime(ds.Tables[0].Rows[i]["DataHoraInicio"].ToString()),
                                NoivoNome = ds.Tables[0].Rows[i]["NoivoNome"].ToString(),
                                NoivaNome = ds.Tables[0].Rows[i]["NoivaNome"].ToString()
                            };
                            casam.Congregacao.Nome = ds.Tables[0].Rows[i]["CongregacaoNome"].ToString();

                            lCasam.Add(casam);
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

            return lCasam;
        }

        public Casamento GetById(long id)
        {
            return GetById(id, 0);
        }
        #endregion
    }
}
