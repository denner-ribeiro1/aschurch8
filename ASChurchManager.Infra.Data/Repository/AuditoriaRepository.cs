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
    public class AuditoriaRepository : RepositoryDAO<Auditoria>, IAuditoriaRepository
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        public AuditoriaRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Privates
        private const string IncluirAuditoria = "IncluirAuditoria";
        #endregion

        public override long Add(Auditoria entity, long usuarioID = 0)
        {
            try
            {
                var lstParans = new List<SqlParameter>
                {
                    new SqlParameter("@UsuarioId", entity.UsuarioId),
                    new SqlParameter("@Controle", entity.Controle),
                    new SqlParameter("@Acao", entity.Acao),
                    new SqlParameter("@Ip", entity.Ip),
                    new SqlParameter("@Url", entity.Url),
                    new SqlParameter("@Parametros", entity.Parametros),
                    new SqlParameter("@Navegador", entity.Navegador)
                    
                };
                return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure,
                                                                            IncluirAuditoria, lstParans.ToArray()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public override int Delete(Auditoria entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Auditoria> GetAll(long usuarioID)
        {
            throw new NotImplementedException();
        }

        public override Auditoria GetById(long id, long usuarioID)
        {
            throw new NotImplementedException();
        }

        public override long Update(Auditoria entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }
        
    }
}
