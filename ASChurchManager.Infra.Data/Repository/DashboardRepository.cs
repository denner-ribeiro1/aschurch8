using ASBaseLib.Data.Helpers.Microsoft.ApplicationBlocks;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Types;
using ASChurchManager.Infra.Data.Repository.EnterpriseLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ASChurchManager.Infra.Data.Repository
{
    public class DashboardRepository : RepositoryDAO<Dashboard>, IDashboardRepository
    {
        #region Variaveis Private
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        #endregion

        #region Construtor
        public DashboardRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Constantes
        private const string ConsultarDashBoard = "ConsultarDashBoard";
        #endregion

        #region Metodos Public
        public override long Add(Dashboard entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public override int Delete(Dashboard entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Dashboard> GetAll(long usuarioID)
        {
            throw new NotImplementedException();
        }

        public override Dashboard GetById(long id, long usuarioID)
        {
            throw new NotImplementedException();
        }

        public Dashboard RetornaDadosDashboard(long usuarioId)
        {
            var lstParameters = new List<SqlParameter>
                    {
                        new("@UsuarioId", usuarioId)
                    };
            var dash = new Dashboard();
            using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ConsultarDashBoard, lstParameters.ToArray()))
            {
                dash.SituacaoMembro = [];
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Enum.TryParse(ds.Tables[0].Rows[i]["STATUS"].ToString(), out Status statusMem);
                        dash.SituacaoMembro.Add(new QtdSitMembro() { Status = statusMem, Quantidade = Convert.ToInt32(ds.Tables[0].Rows[i]["QTD"]) });
                    }
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    dash.QuantidadeCartasPendentes = Convert.ToInt32(ds.Tables[1].Rows[0]["QUANTIDADECARTASPENDENTES"]);
                }
                if (ds.Tables[2].Rows.Count > 0)
                {
                    dash.QuantidadeCongregados = Convert.ToInt32(ds.Tables[2].Rows[0]["QUANTIDADECONGREGADOS"]);
                }
            }
            return dash;
        }

        public override long Update(Dashboard entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        #endregion    
    }
}
