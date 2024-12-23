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
    public class TipoEventoRepository : RepositoryDAO<TipoEvento>, ITipoEventoRepository
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        public TipoEventoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Privates
        private const string SalvarTipoEvento = "dbo.SalvarTipoEvento";
        private const string ListarTipoEvento = "dbo.ListarTipoEvento";
        private const string ConsultarTipoEvento = "dbo.ConsultarTipoEvento";
        private const string DeletarTipoEvento = "dbo.DeletarTipoEvento"; 
        #endregion

        #region Publicos
        public override long Add(TipoEvento entity, long usuarioID = 0)
        {
            try
            {
                var lstParans = new List<SqlParameter>
                {
                    new SqlParameter("@Id", entity.Id),
                    new SqlParameter("@Descricao", entity.Descricao)
                };

                return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, SalvarTipoEvento,
                    lstParans.ToArray()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override long Update(TipoEvento entity, long usuarioID = 0)
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

        public override int Delete(TipoEvento entity, long usuarioID = 0)
        {
            try
            {
                SqlParameter sqlParameter = new SqlParameter("@Id", entity.Id);
                return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, 
                                                                        DeletarTipoEvento, sqlParameter));
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

                return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, 
                                                                        DeletarTipoEvento, sqlParameter));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override TipoEvento GetById(long id, long usuarioID = 0)
        {
            SqlParameter sqlParameter = new SqlParameter("@Id", id);
            TipoEvento TipoEvento = new TipoEvento();
            try
            {
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure,
                                                                            ConsultarTipoEvento, sqlParameter))
                {
                    while (dr.Read())
                    {
                        TipoEvento = DataMapper.ExecuteMapping<TipoEvento>(dr);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return TipoEvento;
        }

        public override IEnumerable<TipoEvento> GetAll(long usuarioID = 0)
        {
            List<TipoEvento> lstTipoEventos = new List<TipoEvento>();

            try
            {
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarTipoEvento))
                {
                    while (dr.Read())
                    {
                        var tipoEvento = DataMapper.ExecuteMapping<TipoEvento>(dr);
                        lstTipoEventos.Add(tipoEvento);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lstTipoEventos;
        } 
        #endregion
    }
}
