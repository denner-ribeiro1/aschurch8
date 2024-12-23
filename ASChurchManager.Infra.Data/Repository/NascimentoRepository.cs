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
    public class NascimentoRepository : RepositoryDAO<Nascimento>, INascimentoRepository
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        public NascimentoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Privates
        private const string SalvarNascimento = "dbo.SalvarNascimento";
        private const string ListarNascimento = "dbo.ListarNascimentos";
        private const string ConsultarNascimento = "dbo.ConsultarNascimento";
        private const string DeletarNascimento = "dbo.DeletarNascimento";
        private const string NascimentosPaginada = "ListarNascimentosPaginada";
        #endregion

        #region Publicos
        public override long Add(Nascimento entity, long usuarioID = 0)
        {
            try
            {
                var lstParans = new List<SqlParameter>
                {
                    new SqlParameter("@Id", entity.Id),
                    new SqlParameter("@CongregacaoId", entity.CongregacaoId),
                    new SqlParameter("@IdMembroPai", entity.IdMembroPai),
                    new SqlParameter("@IdMembroMae", entity.IdMembroMae),
                    new SqlParameter("@Pai", entity.NomePai),
                    new SqlParameter("@Mae", entity.NomeMae),
                    new SqlParameter("@Crianca",entity.Crianca),
                    new SqlParameter("@Sexo", entity.Sexo),
                    new SqlParameter("@DataApresentacao", entity.DataApresentacao),
                    new SqlParameter("@DataNascimento", entity.DataNascimento),
                    new SqlParameter("@Pastor",entity.Pastor),
                    new SqlParameter("@PastorId", entity.PastorId)
                };

                return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, SalvarNascimento,
                    lstParans.ToArray()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override long Update(Nascimento entity, long usuarioID = 0)
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

        public override int Delete(Nascimento entity, long usuarioID = 0)
        {
            try
            {
                SqlParameter sqlParameter = new SqlParameter("@Id", entity.Id);
                return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, DeletarNascimento, sqlParameter));
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
                SqlParameter sqlParameter = new SqlParameter("@Id", id);

                return MicrosoftSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, DeletarNascimento, sqlParameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override Nascimento GetById(long id, long usuarioID)
        {

            Nascimento nascimento = new Nascimento();
            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@UsuarioID", usuarioID)
                };

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ConsultarNascimento, lstParameters.ToArray()))
                {
                    while (dr.Read())
                    {

                        nascimento = DataMapper.ExecuteMapping<Nascimento>(dr);
                        nascimento.CongregacaoId = Convert.ToInt16(dr["CongregacaoId"].ToString());
                        nascimento.congregacao.Id = Convert.ToInt64(dr["CongregacaoId"].ToString());
                        nascimento.congregacao.Nome = dr["CongregacaoNome"].TryConvertTo<string>();
                        nascimento.congregacao.CongregacaoResponsavelId = Convert.ToInt64(dr["CongregacaoResponsavelId"].ToString());
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return nascimento;
        }

        public override IEnumerable<Nascimento> GetAll(long usuarioID)
        {
            List<Nascimento> lNascimentos = new List<Nascimento>();

            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new SqlParameter("@UsuarioID", usuarioID)
                };
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarNascimento, lstParameters.ToArray()))
                {
                    while (dr.Read())
                    {
                        var nascimento = DataMapper.ExecuteMapping<Nascimento>(dr);
                        lNascimentos.Add(nascimento);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lNascimentos;
        }

        public IEnumerable<Nascimento> ListarNascimentoPaginado(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor, long usuarioID)
        {
            rowCount = 0;
            var lstNasc = new List<Nascimento>();
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
                using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, NascimentosPaginada, lstParameters.ToArray()))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var crianca = new Nascimento()
                            {
                                Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                                Crianca = ds.Tables[0].Rows[i]["Crianca"].ToString(),
                                NomePai = ds.Tables[0].Rows[i]["NomePai"].ToString(),
                                NomeMae = ds.Tables[0].Rows[i]["NomeMae"].ToString(),
                                DataNascimento = Convert.ToDateTime(ds.Tables[0].Rows[i]["DataNascimento"].ToString()),
                                Sexo = ds.Tables[0].Rows[i]["Sexo"].ToString() == "2" ? Domain.Types.Sexo.Feminino : Domain.Types.Sexo.Masculino,
                                DataApresentacao = Convert.ToDateTime(ds.Tables[0].Rows[i]["DataApresentacao"].ToString()),
                                DataAlteracao = Convert.ToDateTime(ds.Tables[0].Rows[i]["DataAlteracao"].ToString()),
                                DataCriacao = Convert.ToDateTime(ds.Tables[0].Rows[i]["DataCriacao"].ToString())
                            };
                            lstNasc.Add(crianca);
                        }
                    }
                    if (ds.Tables[1].Rows.Count > 0)
                        rowCount = Convert.ToInt16(ds.Tables[1].Rows[0][0]);
                }

            }
            catch
            {
                throw;
            }

            return lstNasc;
        }

        #endregion
    }
}
