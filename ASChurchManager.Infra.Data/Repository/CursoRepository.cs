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
    public class CursoRepository : RepositoryDAO<Curso>, ICursoRepository
    {
        #region Variaveis Private
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        #endregion

        #region Construtor
        public CursoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Constantes
        private const string ConsultarCurso = "ConsultarCurso";
        private const string DeletarCurso = "DeletarCurso";
        private const string ListarCursos = "ListarCursos";
        private const string SalvarCurso = "SalvarCurso";
        #endregion

        #region Metodos Public
        public override long Add(Curso entity, long usuarioID = 0)
        {
            var dtInicio = entity.DataInicio == DateTimeOffset.MinValue || entity.DataInicio == null ? (object)DBNull.Value : entity.DataInicio;
            var dtEncer = entity.DataEncerramento == DateTimeOffset.MinValue || entity.DataEncerramento == null ? (object)DBNull.Value : entity.DataEncerramento;

            var lstParans = new List<SqlParameter>
                    {
                        new("@Id", entity.Id),
                        new("@Descricao", entity.Descricao),
                        new("@DataInicio", dtInicio),
                        new("@DataEncerramento", dtEncer),
                        new("@CargaHoraria", entity.CargaHoraria)
                    };

            return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, SalvarCurso, lstParans.ToArray()));
        }

        public override long Update(Curso entity, long usuarioID = 0)
        {
            return Add(entity);
        }
        public override int Delete(Curso entity, long usuarioID = 0)
        {
            return Delete(entity.Id);
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            SqlParameter sqlParameter = new("@Id", id);
            return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, DeletarCurso, sqlParameter));
        }

        public override IEnumerable<Curso> GetAll(long usuarioID)
        {
            var lCursos = new List<Curso>();
            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarCursos))
            {
                while (dr.Read())
                {
                    var cur = DataMapper.ExecuteMapping<Curso>(dr);
                    lCursos.Add(cur);
                }
            }
            return lCursos;
        }

        public override Curso GetById(long id, long usuarioID)
        {
            var curso = new Curso();
            SqlParameter sqlParameter = new("@Id", id);
            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ConsultarCurso, sqlParameter))
            {
                while (dr.Read())
                    curso = DataMapper.ExecuteMapping<Curso>(dr);
            }
            return curso;
        }
        #endregion
    }
}
