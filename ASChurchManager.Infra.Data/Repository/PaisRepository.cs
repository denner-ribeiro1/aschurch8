using ASBaseLib.Data.Helpers.Microsoft.ApplicationBlocks;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces.Repository;
using ASChurchManager.Infra.Data.Repository.EnterpriseLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ASChurchManager.Infra.Data.Repository
{
    public class PaisRepository : RepositoryDAO<Pais>, IPaisRepository
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];

        public const string ConsultarPaises = "ConsultarPaises";
        public PaisRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public override long Add(Pais entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public override int Delete(Pais entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Pais> GetAll(long usuarioID)
        {
            var paises = new List<Pais>();
            using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsultarPaises))
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        paises.Add(new Pais()
                        {
                            Id = Convert.ToInt32(ds.Tables[0].Rows[i]["Id"]),
                            Nome = ds.Tables[0].Rows[i]["Nome"].ToString(),
                            Abrev = ds.Tables[0].Rows[i]["Abrev"].ToString()
                        });
                    }
                }

            }
            return paises;
        }

        public override Pais GetById(long id, long usuarioID)
        {
            throw new NotImplementedException();
        }

        public override long Update(Pais entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }
    }
}
