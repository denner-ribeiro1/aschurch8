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
    public class PerfilRepository : RepositoryDAO<Perfil>, IPerfilRepository
    {
        #region Variaveis Private
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        #endregion

        #region Construtor
        public PerfilRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Constantes
        private const string ListarPerfis = "dbo.ListarPerfis";
        private const string ConsultarPerfilPorId = "dbo.ConsultarPerfilPorId";
        private const string SalvarPerfil = "dbo.SalvarPerfil";
        private const string SalvarPerfilRotina = "dbo.SalvarPerfilRotina";
        private const string DeletarPerfilRotina = "dbo.DeletarPerfilRotina";
        #endregion


        #region Metodos Public
        public override long Add(Perfil entity, long usuarioID = 0)
        {
            var lstParameters = new List<SqlParameter>
             {
                 new("@Id", entity.Id),
                 new("@Nome", entity.Nome),
                 new("@TipoPerfil",entity.TipoPerfil),
                 new("@Status", entity.Status)
             };

            return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure,
                                                                    SalvarPerfil, lstParameters.ToArray()));
        }

        public override long Update(Perfil entity, long usuarioID = 0)
        {
            return Add(entity);
        }

        public override int Delete(Perfil entity, long usuarioID = 0)
        {
            return Delete(entity.Id);
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            SqlParameter sqlParameter = new("@Id", id);
            return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, DeletarPerfilRotina, sqlParameter));
        }

        public override Perfil GetById(long id, long usuarioID)
        {
            var perfil = new Perfil();

            var lstParameters = new List<SqlParameter>
                 {
                     new("@Id", id),
                     new("@UsuarioID", usuarioID)
                 };

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure,
                                                                        ConsultarPerfilPorId, lstParameters.ToArray());
            do
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        if (dr.GetName(0) == "PerfilId")
                        {
                            perfil = DataMapper.ExecuteMapping<Perfil>(dr);
                            perfil.Id = Convert.ToInt64(dr["PerfilId"]);
                        }

                        if (dr.GetName(0) == "RotinaId")
                        {
                            var rotina = DataMapper.ExecuteMapping<Rotina>(dr);
                            rotina.Id = Convert.ToInt64(dr["RotinaId"]);

                            perfil.Rotinas.Add(rotina);
                        }

                    }

                }
            } while (dr.NextResult());

            return perfil;
        }

        public override IEnumerable<Perfil> GetAll(long usuarioID)
        {
            var lstPerfil = new List<Perfil>();

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarPerfis);
            while (dr.Read())
            {
                var perfil = DataMapper.ExecuteMapping<Perfil>(dr);
                lstPerfil.Add(perfil);
            }

            return lstPerfil;
        }

        public long AddRotinaPerfil(long perfilId, long rotinaId)
        {
            var lstParameters = new List<SqlParameter>
             {
                 new("@IdPerfil", perfilId),
                 new("@IdRotina", rotinaId)
             };

            return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure,
                                                                        SalvarPerfilRotina, lstParameters.ToArray()));
        }
        #endregion
    }
}
