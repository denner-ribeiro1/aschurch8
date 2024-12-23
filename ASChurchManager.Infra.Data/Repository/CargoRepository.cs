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
    public class CargoRepository : RepositoryDAO<Cargo>, ICargoRepository
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        public CargoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Privates
        private const string SalvarCargo = "dbo.SalvarCargo";
        private const string ListarCargo = "dbo.ListarCargos";
        private const string ConsultarCargo = "dbo.ConsultarCargo";
        private const string DeletarCargo = "dbo.DeletarCargo";
        #endregion

        #region Publicos
        public override long Add(Cargo entity, long usuarioID = 0)
        {
            try
            {
                var lstParans = new List<SqlParameter>
                {
                    new SqlParameter("@Id", entity.Id),
                    new SqlParameter("@Descricao", entity.Descricao),
                    new SqlParameter("@Obreiro", entity.Obreiro),
                    new SqlParameter("@Lider", entity.Lider),
                    new SqlParameter("@TipoCarteirinha", (byte)entity.TipoCarteirinha),
                    new SqlParameter("@Confradesp", entity.Confradesp),
                    new SqlParameter("@CGADB", entity.CGADB)
                };

                return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, SalvarCargo, lstParans.ToArray()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override long Update(Cargo entity, long usuarioID = 0)
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

        public override int Delete(Cargo entity, long usuarioID = 0)
        {
            try
            {
                SqlParameter sqlParameter = new SqlParameter("@Id", entity.Id);
                return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, DeletarCargo, sqlParameter));
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

                return MicrosoftSqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, DeletarCargo, sqlParameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override Cargo GetById(long id, long usuarioID = 0)
        {

            Cargo cargo = new Cargo();
            try
            {
                SqlParameter sqlParameter = new SqlParameter("@Id", id);

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ConsultarCargo, sqlParameter))
                {
                    while (dr.Read())
                    {
                        cargo = DataMapper.ExecuteMapping<Cargo>(dr);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return cargo;
        }

        public override IEnumerable<Cargo> GetAll(long usuarioID = 0)
        {
            List<Cargo> lstCargos = new List<Cargo>();

            try
            {
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarCargo))
                {
                    while (dr.Read())
                    {
                        var cargo = DataMapper.ExecuteMapping<Cargo>(dr);
                        lstCargos.Add(cargo);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lstCargos;
        }
        #endregion
    }
}
