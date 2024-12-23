using ASBaseLib.Data.DataMapping;
using ASBaseLib.Data.Helpers.Microsoft.ApplicationBlocks;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Types;
using ASChurchManager.Infra.Data.Repository.EnterpriseLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ASChurchManager.Infra.Data.Repository
{
    public class BatismoRepository : RepositoryDAO<Batismo>, IBatismoRepository
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        public BatismoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Privates
        private const string UpdMembroBatismo = "AtualizarMembroBatismo";
        private const string SalvarBatismo = "SalvarBatismo";
        private const string SalvarBatismoCelebrante = "SalvarBatismoCelebrante";
        private const string ListarDatasBatismo = "ListarDatasBatismo";
        private const string SelUltimaDataBatismo = "SelecionaUltimaDataBatismo";
        private const string SelMembrosParaBatismo = "SelecionaMembrosParaBatismo";
        private const string UpdStatusBatismo = "AtualizarStatusBatismo";
        private const string ListarPastoresCelebrante = "ListarPastorCelebrante";
        private const string ConsultarBatismo = "ConsultarBatismo";
        private const string DeletarBatismo = "DeletarBatismo";
        private const string DeletarBatismoCelebrante = "DeletarBatismoCelebrante";
        private const string CandidatosBatismoPaginada = "ListarCandidatosBatismoPaginada";
        private const string BatismoPaginada = "ListarBatismoPaginada";
        #endregion

        #region Publicos
        public override long Add(Batismo entity, long usuarioID = 0)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlTransaction sqlTran = con.BeginTransaction();
                try
                {
                    var lstParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@Id", entity.Id),
                        new SqlParameter("@DataMaximaCadastro", entity.DataMaximaCadastro),
                        new SqlParameter("@DataBatismo",entity.DataBatismo),
                        new SqlParameter("@Status", entity.Status),
                        new SqlParameter("@IdadeMinima", entity.IdadeMinima)
                    };
                    var idBatis = Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(sqlTran, CommandType.StoredProcedure, SalvarBatismo, lstParameters.ToArray()));

                    foreach (var cel in entity.Celebrantes)
                    {
                        lstParameters = new List<SqlParameter>
                        {
                            new SqlParameter("@BatismoId", idBatis),
                            new SqlParameter("@MembroId", cel.Id),
                        };
                        MicrosoftSqlHelper.ExecuteScalar(sqlTran, CommandType.StoredProcedure, SalvarBatismoCelebrante, lstParameters.ToArray());
                    }
                    sqlTran.Commit();
                    return idBatis;
                }
                catch
                {
                    sqlTran.Rollback();
                    throw;
                }
            }
        }

        public override long Update(Batismo entity, long usuarioID = 0)
        {
            return Add(entity);
        }

        public override int Delete(Batismo entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            try
            {
                SqlParameter sqlParameter = new SqlParameter("@Id", id);
                MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, DeletarBatismoCelebrante, sqlParameter);
                return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, DeletarBatismo, sqlParameter));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override Batismo GetById(long id, long usuarioID = 0)
        {
            Batismo batismo = new Batismo();
            try
            {
                SqlParameter sqlParameter = new SqlParameter("@Id", id);

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ConsultarBatismo, sqlParameter))
                {
                    while (dr.Read())
                    {
                        batismo = DataMapper.ExecuteMapping<Batismo>(dr);
                        //batismo.Status = (StatusBatismo)dr["Satus"].TryConvertTo<int>();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return batismo;
        }

        public override IEnumerable<Batismo> GetAll(long usuarioID)
        {
            var lst = new List<Batismo>();
            try
            {
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarDatasBatismo))
                {
                    while (dr.Read())
                    {
                        lst.Add(DataMapper.ExecuteMapping<Batismo>(dr));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lst;
        }

        public void AtualizarMembroBatismo(IEnumerable<BatismoMembro> candidatoBatismo, long batismoId)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlTransaction sqlTran = con.BeginTransaction();
                try
                {
                    foreach (var mem in candidatoBatismo)
                    {
                        var lstParans = new List<SqlParameter>
                        {
                            new SqlParameter("@Id", mem.MembroId),
                            new SqlParameter("@Presente", Convert.ToInt16(mem.SituacaoCandBatismo)),
                            new SqlParameter("@BatismoId", batismoId)
                        };
                        Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(sqlTran, CommandType.StoredProcedure, UpdMembroBatismo, lstParans.ToArray()));
                    }

                    var param = new List<SqlParameter>
                    {
                        new SqlParameter("@Id", batismoId),
                        new SqlParameter("@Status", Convert.ToInt16(StatusBatismo.Finalizado))
                    };
                    MicrosoftSqlHelper.ExecuteScalar(sqlTran, CommandType.StoredProcedure, UpdStatusBatismo, param.ToArray());
                    sqlTran.Commit();
                }
                catch (Exception)
                {
                    sqlTran.Rollback();
                    throw;
                }
            }
        }

        public Batismo SelecionaUltimoDataBatismo()
        {
            var batismo = new Batismo();
            try
            {
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, SelUltimaDataBatismo))
                {
                    while (dr.Read())
                    {
                        batismo = DataMapper.ExecuteMapping<Batismo>(dr);
                        batismo.Status = ((StatusBatismo)dr["Status"].TryConvertTo<int>());
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return batismo;
        }

        public IEnumerable<Membro> SelecionaMembrosParaBatismo(long batismoId, StatusBatismo status, long usuarioID)
        {
            var lstMembros = new List<Membro>();
            var lstParameters = new List<SqlParameter>
            {
                new SqlParameter("@BatismoId", batismoId),
                new SqlParameter("@Status", status),
                new SqlParameter("@UsuarioID", usuarioID)
            };

            try
            {
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, SelMembrosParaBatismo, lstParameters.ToArray()))
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

        public IEnumerable<Membro> ListarPastorCelebrante(long batismoId)
        {
            var lPastor = new List<Membro>();
            var lstParameters = new List<SqlParameter>
            {
                new SqlParameter("@BatismoId",batismoId)
            };
            try
            {
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ListarPastoresCelebrante, lstParameters.ToArray()))
                {
                    while (dr.Read())
                    {
                        var pastor = DataMapper.ExecuteMapping<Membro>(dr);
                        lPastor.Add(pastor);
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }

            return lPastor;
        }

        public void AtualizarStatusBatismo(long batismoId, SituacaoCandidatoBatismo situacao)
        {
            var lstParans = new List<SqlParameter>
            {
                new SqlParameter("@Id", batismoId),
                new SqlParameter("@Status", Convert.ToInt16(situacao))
            };
            MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, UpdMembroBatismo, lstParans.ToArray());
        }

        public IEnumerable<Membro> ListarCandidatosBatismoPaginada(int pageSize, int rowStart, string sorting, string campo, string valor, long usuarioID, out int rowCount)
        {
            rowCount = 0;
            var lstMembros = new List<Membro>();

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
                using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, CandidatosBatismoPaginada, lstParameters.ToArray()))
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
                                Cpf = ds.Tables[0].Rows[i]["Cpf"].ToString(),
                                DataNascimento = Convert.ToDateTime(ds.Tables[0].Rows[i]["DataNascimento"].ToString()),
                                BatismoId = Convert.ToInt16(ds.Tables[0].Rows[i]["BatismoId"])

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

        public IEnumerable<Batismo> ListarBatismoPaginada(int pageSize, int rowStart, string sorting, out int rowCount)
        {
            rowCount = 0;
            var lstBatismo = new List<Batismo>();

            var lstParameters = new List<SqlParameter>
                {
                    new SqlParameter("@PAGESIZE", pageSize),
                    new SqlParameter("@ROWSTART", rowStart),
                    new SqlParameter("@SORTING", sorting)
                };
            try
            {
                using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, BatismoPaginada, lstParameters.ToArray()))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var batismo = new Batismo()
                            {
                                Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                                DataBatismo = Convert.ToDateTime(ds.Tables[0].Rows[i]["DataBatismo"].ToString()),
                                DataMaximaCadastro = Convert.ToDateTime(ds.Tables[0].Rows[i]["DataMaximaCadastro"].ToString()),
                                Status = ds.Tables[0].Rows[i]["Status"].ToString().ToEnum<StatusBatismo>()
                            };
                            lstBatismo.Add(batismo);
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

            return lstBatismo;
        }
        #endregion
    }
}
