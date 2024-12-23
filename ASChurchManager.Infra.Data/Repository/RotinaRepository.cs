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
    public class RotinaRepository : RepositoryDAO<Rotina>, IRotinaRepository
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        public RotinaRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Privates
        private const string SalvarRotina = "SalvarRotina";
        private const string ConsRotinas = "ConsultarRotinas";
        private const string ConsRotinaPorId = "ConsultarRotinaPorID";
        private const string ConsRotinaPorAreaController = "ConsultarRotinaPorAreaController";
        private const string ConsRotinasPorUsuario = "ConsultarRotinasPorUsuario";
        #endregion

        #region Publicos
        public override long Add(Rotina entity, long usuarioID = 0)
        {
            try
            {
                var lstParans = new List<SqlParameter>
                {
                    new SqlParameter("@Id", entity.Id),
                    new SqlParameter("@Area", entity.Area),
                    new SqlParameter("@AreaDescricao", entity.AreaDescricao),
                    new SqlParameter("@AreaIcone", entity.AreaIcone),
                    new SqlParameter("@Controller", entity.Controller),
                    new SqlParameter("@Action", entity.Action),
                    new SqlParameter("@MenuDescricao", entity.MenuDescricao),
                    new SqlParameter("@MenuIcone", entity.MenuIcone),
                    new SqlParameter("@SubMenuDescricao", entity.SubMenuDescricao),
                    new SqlParameter("@SubMenuIcone", entity.SubMenuIcone),

                };
                return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure,
                                                                            SalvarRotina, lstParans.ToArray()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override long Update(Rotina entity, long usuarioID = 0)
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

        public override int Delete(Rotina entity, long usuarioID = 0)
        {
            throw new System.NotImplementedException();
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            throw new System.NotImplementedException();
        }

        public override Rotina GetById(long id, long usuarioID = 0)
        {
            Rotina rotina = new Rotina();
            try
            {
                SqlParameter sqlParameter = new SqlParameter("@Id", id);

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, 
                                                                            ConsRotinaPorId, sqlParameter))
                {
                    while (dr.Read())
                    {
                        rotina = DataMapper.ExecuteMapping<Rotina>(dr);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return rotina;
        }

        public override IEnumerable<Rotina> GetAll(long usuarioID = 0)
        {
            List<Rotina> lstRotinas = new List<Rotina>();

            try
            {
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ConsRotinas))
                {
                    while (dr.Read())
                    {
                        var rotina = DataMapper.ExecuteMapping<Rotina>(dr);
                        lstRotinas.Add(rotina);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lstRotinas;
        }

        public Rotina ConsultarRotinaPorAreaController(string area, string controller)
        {
            Rotina rotina = new Rotina();
            try
            {
                var lstParans = new List<SqlParameter>
                {
                    new SqlParameter("@Area", area),
                    new SqlParameter("@Controller", controller)
                };

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, 
                                                                                ConsRotinaPorAreaController, lstParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        rotina = DataMapper.ExecuteMapping<Rotina>(dr);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return rotina;
        }

        public IEnumerable<Rotina> ConsultarRotinasPorUsuario(long usuarioID)
        {
            List<Rotina> lstRotinas = new List<Rotina>();

            try
            {
                var param = new SqlParameter("@UsuarioId", usuarioID);

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, 
                                                                            ConsRotinasPorUsuario, param))
                {
                    while (dr.Read())
                    {
                        var rotina = DataMapper.ExecuteMapping<Rotina>(dr);
                        lstRotinas.Add(rotina);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lstRotinas;
        }

        #endregion
    }
}