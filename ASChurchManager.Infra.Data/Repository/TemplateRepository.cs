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
    public class TemplateRepository : RepositoryDAO<Template>, ITemplateRepository
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        public TemplateRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private const string ListarTemplates = "dbo.ListarTemplates";
        private const string ConsultarTemplatePorId = "dbo.ConsultarTemplatePorId";
        private const string SalvarTemplate = "dbo.SalvarTemplate";
        private const string DeletarTemplate = "dbo.DeletarTemplate";

        public override long Add(Template entity, long usuarioID = 0)
        {
            var lstParameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", entity.Id),
                new SqlParameter("@Nome", entity.Nome),
                new SqlParameter("@Conteudo", entity.Conteudo),
                new SqlParameter("@Tipo", entity.Tipo),
                new SqlParameter("@MargemAbaixo", entity.MargemAbaixo),
                new SqlParameter("@MargemAcima", entity.MargemAcima),
                new SqlParameter("@MargemDireita", entity.MargemDireita),
                new SqlParameter("@MargemEsquerda", entity.MargemEsquerda)
            };

            return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, 
                                                                        SalvarTemplate, lstParameters.ToArray()));
        }

        public override long Update(Template entity, long usuarioID = 0)
        {
            return this.Add(entity);
        }

        public override int Delete(Template entity, long usuarioID = 0)
        {
            return this.Delete(entity.Id);
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            var param = new SqlParameter("@Id", id);
            return Convert.ToInt32(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, DeletarTemplate, param));
        }

        public override Template GetById(long id, long usuarioID)
        {
            var template = new Template();

            try
            {
                var lstParameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@UsuarioID", usuarioID)
                };

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, 
                                                                            ConsultarTemplatePorId, lstParameters.ToArray()))
                {
                    while (dr.Read())
                    {
                        template = DataMapper.ExecuteMapping<Template>(dr);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return template;
        }

        public override IEnumerable<Template> GetAll(long usuarioID)
        {
            var lstTemplate = new List<Template>();

            try
            {
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarTemplates))
                {
                    while (dr.Read())
                    {
                        var Template = DataMapper.ExecuteMapping<Template>(dr);
                        lstTemplate.Add(Template);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return lstTemplate;
        }
    }
}
