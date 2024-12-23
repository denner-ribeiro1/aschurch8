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
    public class CursoArquivoMembroRepository : RepositoryDAO<CursoArquivoMembro>, ICursoArquivoMembroRepository
    {
        #region Variaveis Private
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        private SqlTransaction transaction = null;
        private SqlConnection connection = null;
        #endregion

        #region Construtor
        public CursoArquivoMembroRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Constantes Private
        private const string ConsultarCursoArquivoMembro = "ConsultarCursoArquivoMembro";
        private const string DeletarCursoArquivoMembro = "DeletarCursoArquivoMembro";
        private const string ListarCursoArquivoMembro = "ListarCursoArquivoMembro";
        private const string SalvarCursoArquivoMembro = "SalvarCursoArquivoMembro";
        #endregion



        #region Metodos Public
        public override long Add(CursoArquivoMembro entity, long usuarioID = 0)
        {
            var dtInicio = entity.DataInicioCurso == DateTimeOffset.MinValue || entity.DataInicioCurso == null ? (object)DBNull.Value : entity.DataInicioCurso;
            var dtEncer = entity.DataEncerramentoCurso == DateTimeOffset.MinValue || entity.DataEncerramentoCurso == null ? (object)DBNull.Value : entity.DataEncerramentoCurso;

            var cursoId = entity.CursoId > 0 ? (object)entity.CursoId : DBNull.Value;

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
                        new("@CargaHoraria", entity.CargaHoraria),
                        new("@NomeOriginal", entity.NomeOriginal),
                        new("@Tamanho", entity.Tamanho),
                        new("@NomeArmazenado", entity.NomeArmazenado)
                    };
            lstParans[lstParans.Count - 1].Direction = ParameterDirection.Output;
            lstParans[lstParans.Count - 1].Size = 100;

            if (transaction != null)
                return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, SalvarCursoArquivoMembro, lstParans.ToArray()));
            return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, SalvarCursoArquivoMembro, lstParans.ToArray()));
        }
        public override long Update(CursoArquivoMembro entity, long usuarioID = 0)
        {
            return Add(entity);
        }

        public override int Delete(CursoArquivoMembro entity, long usuarioID = 0)
        {
            return Delete(entity.Id);
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            SqlParameter sqlParameter = new("@Id", id);

            if (transaction != null)
                return MicrosoftSqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, DeletarCursoArquivoMembro, sqlParameter);
            return MicrosoftSqlHelper.ExecuteNonQuery(ConnectionString, CommandType.StoredProcedure, DeletarCursoArquivoMembro, sqlParameter);
        }

        public override IEnumerable<CursoArquivoMembro> GetAll(long usuarioID)
        {
            throw new NotImplementedException();
        }

        public override CursoArquivoMembro GetById(long id, long usuarioID)
        {
            var arq = new CursoArquivoMembro();
            SqlParameter sqlParameter = new("@Id", id);

            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ConsultarCursoArquivoMembro, sqlParameter))
            {
                while (dr.Read())
                {
                    arq = DataMapper.ExecuteMapping<CursoArquivoMembro>(dr);
                }
            }
            return arq;
        }

        public IEnumerable<CursoArquivoMembro> GetArquivoByMembro(long membroId)
        {
            var lArq = new List<CursoArquivoMembro>();
            try
            {
                SqlParameter sqlParameter = new("@MembroId", membroId);
                using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarCursoArquivoMembro, sqlParameter);
                while (dr.Read())
                {
                    lArq.Add(DataMapper.ExecuteMapping<CursoArquivoMembro>(dr));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lArq;
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
        #endregion
    }
}
