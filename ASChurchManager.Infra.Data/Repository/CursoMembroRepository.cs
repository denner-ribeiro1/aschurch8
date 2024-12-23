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
    public class CursoMembroRepository : RepositoryDAO<CursoArquivoMembro>, ICursoArquivoMembroRepository
    {
        #region Variaveis Private
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        private SqlTransaction transaction = null;
        private SqlConnection connection = null;
        #endregion

        #region Construtor
        public CursoMembroRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Constantes
        private const string ConsultarCurso = "ConsultarCursoMembro";
        private const string DeletarCurso = "DeletarCursoMembro";
        private const string ListarCursos = "ListarCursoMembro";
        private const string SalvarCurso = "SalvarCursoMembro";
        #endregion

        #region Metodos Public
        public override long Add(CursoArquivoMembro entity, long usuarioID = 0)
        {
            var dtInicio = entity.DataInicioCurso == DateTimeOffset.MinValue || entity.DataInicioCurso == null ? (object)DBNull.Value : entity.DataInicioCurso;
            var dtEncer = entity.DataEncerramentoCurso == DateTimeOffset.MinValue || entity.DataEncerramentoCurso == null ? (object)DBNull.Value : entity.DataEncerramentoCurso;
            var carga = entity.CargaHoraria;

            var lstParans = new List<SqlParameter>
                      {
                          new("@Id", entity.Id),
                          new("@MembroId", entity.MembroId),
                          new("@TipoArquivo", entity.TipoArquivo),
                          new("@Descricao", entity.Descricao),
                          new("@CursoId", entity.CursoId),
                          new("@Local", entity.Local),
                          new("@NomeCurso", entity.NomeCurso),
                          new("@DataInicioCurso", dtInicio),
                          new("@DataEncerramentoCurso", dtEncer),
                          new("@CargaHoraria", carga),
                          new("@NomeOriginal", entity.NomeOriginal),
                          new("@Tamanho", entity.Tamanho),
                          new("@NomeArmazenado", entity.NomeArmazenado)
                      };
            lstParans[lstParans.Count - 1].Direction = ParameterDirection.Output;
            lstParans[lstParans.Count - 1].Size = 100;

            if (transaction != null)
                return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, SalvarCurso, lstParans.ToArray()));
            return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, SalvarCurso, lstParans.ToArray()));
        }

        public override int Delete(CursoArquivoMembro entity, long usuarioID = 0)
        {
            return Delete(entity.Id);
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            SqlParameter sqlParameter = new("@Id", id);
            if (transaction != null)
                return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, DeletarCurso, sqlParameter));
            return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, DeletarCurso, sqlParameter));
        }

        public IEnumerable<CursoMembro> GetAllCursoMembro(long usuarioID)
        {
            var lCursos = new List<CursoMembro>();
            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarCursos))
            {
                while (dr.Read())
                {
                    var cur = DataMapper.ExecuteMapping<CursoMembro>(dr);
                    lCursos.Add(cur);

                }
            }
            return lCursos;
        }

        public CursoMembro GetByIdCursoMembro(long id, long usuarioID)
        {
            var curso = new CursoMembro();
            SqlParameter sqlParameter = new("@Id", id);
            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ConsultarCurso, sqlParameter))
            {
                while (dr.Read())
                    curso = DataMapper.ExecuteMapping<CursoMembro>(dr);
            }
            return curso;
        }

        public long Update(CursoMembro entity)
        {
            return Add(entity);
        }

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

        public override long Update(CursoArquivoMembro entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CursoArquivoMembro> GetArquivoByMembro(long membroId)
        {
            throw new NotImplementedException();
        }

        public override CursoArquivoMembro GetById(long id, long usuarioID)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<CursoArquivoMembro> GetAll(long usuarioID)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
