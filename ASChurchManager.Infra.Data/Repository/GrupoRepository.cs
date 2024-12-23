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
    public class GrupoRepository : RepositoryDAO<Grupo>, IGrupoRepository
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        public GrupoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Privates
        private const string SalvarGrupo = "dbo.SalvarGrupo";
        private const string ListarGrupo = "dbo.ListarGrupo";
        private const string ConsultarGrupo = "dbo.ConsultarGrupo";
        private const string DeletarGrupo = "dbo.DeletarGrupo";
        private const string ListarGrpCongr = "dbo.ListarGruposCongregacao";
        #endregion

        #region publicos

        public override long Add(Grupo entity, long usuarioID = 0)
        {
            try
            {
                var lstParans = new List<SqlParameter>
                {
                    new SqlParameter("@Id", entity.Id),
                    new SqlParameter("@Descricao", entity.Descricao)
                };

                return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, SalvarGrupo, lstParans.ToArray()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override long Update(Grupo entity, long usuarioID = 0)
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

        public override int Delete(Grupo entity, long usuarioID = 0)
        {
            try
            {
                SqlParameter sqlParameter = new SqlParameter("@Id", entity.Id);
                return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, DeletarGrupo, sqlParameter));
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
                return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, DeletarGrupo, sqlParameter));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override Grupo GetById(long id, long usuarioID)
        {
            Grupo grupo = new Grupo();
            try
            {
                SqlParameter sqlParameter = new SqlParameter("@Id", id);

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ConsultarGrupo, sqlParameter))
                {
                    while (dr.Read())
                    {
                        grupo = DataMapper.ExecuteMapping<Grupo>(dr);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return grupo;
        }

        public override IEnumerable<Grupo> GetAll(long usuarioID)
        {
            List<Grupo> lstGrupos = new List<Grupo>();

            try
            {
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarGrupo))
                {
                    while (dr.Read())
                    {
                        var grupo = DataMapper.ExecuteMapping<Grupo>(dr);
                        lstGrupos.Add(grupo);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lstGrupos;
        }

        public IEnumerable<CongregacaoGrupo> ListarGrupoCongregacao(int congregacaoId)
        {
            var lstGrupos = new List<CongregacaoGrupo>();
            try
            {
                SqlParameter sqlParameter = new SqlParameter("@CongregacaoId", congregacaoId);

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarGrpCongr, sqlParameter))
                {
                    while (dr.Read())
                    {
                        var grupo = new CongregacaoGrupo()
                        {
                            Id = dr["Id"].TryConvertTo<int>(),
                            Grupo = dr["GrupoDescricao"].TryConvertTo<string>(),
                            ResponsavelId = dr["ConvidadoId"].TryConvertTo<long>(),
                            ResponsavelNome = dr["ConvidadoNome"].TryConvertTo<string>(),
                            CongregacaoId = dr["CongregacaoId"].TryConvertTo<long>()
                        };
                        lstGrupos.Add(grupo);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lstGrupos;
        }
        #endregion
    }
}
