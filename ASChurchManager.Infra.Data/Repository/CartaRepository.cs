using ASBaseLib.Data.Helpers.Microsoft.ApplicationBlocks;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using ASChurchManager.Infra.Data.Repository.EnterpriseLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ASChurchManager.Infra.Data.Repository
{
    public class CartaRepository : RepositoryDAO<Carta>, ICartaRepository
    {
        #region Variaveis Private
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        #endregion

        #region Contrutor
        public CartaRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Constantes
        private const string SalvarCarta = "dbo.SalvarCarta";
        private const string ListarCartas = "dbo.ListarCarta";
        private const string ConsultarCarta = "dbo.ConsultarCarta";
        private const string DeletarCartas = "dbo.DeletarCarta";
        private const string AprovaCarta = "dbo.AprovaCarta";
        private const string ConsultarCodRecebimento = "dbo.ConsultarCodigoRecebimento";
        private const string VerifCartaAguardReceb = "VerificaCartaAguardandoRecebimento";
        private const string ListarPorTipoEStatus = "ListarPorTipoEStatus";
        private const string AlterarCongregacaoMembro = "AlterarCongregacaoMembro";
        private const string CartaPaginada = "ListarCartaPaginada";
        private const string CancCarta = "CancelarCarta";
        private const string ListarCartasMembroId = "ListarCartasPorMembroId";
        #endregion

        #region Metodos Public
        public override long Add(Carta entity, long usuarioID = 0)
        {
            try
            {
                var ListParans = new List<SqlParameter>
                    {
                        new("@Id", entity.Id),
                        new("@MembroId", entity.MembroId),
                        new("@TipoCarta", Convert.ToInt16(entity.TipoCarta)),
                        new("@CongregacaoOrigemId", entity.CongregacaoOrigemId),
                        new("@CongregacaoDestID", entity.CongregacaoDestId),
                        new("@CongregacaoDest", entity.CongregacaoDest),
                        new("@Observacao", entity.Observacao = !string.IsNullOrWhiteSpace(entity.Observacao)? entity.Observacao.Replace("\r\n","<br>"): string.Empty),
                        new("@DataValidade", entity.DataValidade),
                        new("@StatusCarta", entity.StatusCarta),
                        new("@TemplateId", entity.TemplateId),
                        new("@CodigoRecebimento", entity.CodigoRecebimento),
                        new("@UsuarioID", entity.IdCadastro),
                    };
                return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, SalvarCarta, ListParans.ToArray()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override long Update(Carta entity, long usuarioID = 0)
        {
            try
            {
                // A lógica para update está na procedure SalvarMembro, passando Id > 0;
                return Add(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override int Delete(Carta entity, long usuarioID = 0)
        {
            try
            {
                var param = new SqlParameter("@Id", entity.Id);

                return Convert.ToInt32(MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, DeletarCartas, param));
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
                var param = new SqlParameter("@Id", id);

                return Convert.ToInt32(MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, DeletarCartas, param));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override Carta GetById(long id, long usuarioID)
        {
            var lstParameters = new List<SqlParameter>
                    {
                        new("@Id", id),
                        new("@UsuarioID", usuarioID)
                    };

            Carta carta = new();


            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure,
                       ConsultarCarta, lstParameters.ToArray());
            while (dr.Read())
            {
                carta = DataMapper.ExecuteMapping<Carta>(dr);
            }

            return carta;
        }

        public long ConsultarCodReceb(long pIdMembro)
        {
            long codReceb = 0;
            var param = new SqlParameter("@IdMembro", pIdMembro);

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ConsultarCodRecebimento, param);
            while (dr.Read())
            {
                codReceb = Convert.ToInt64(dr["CodRecebimento"]);
            }

            return codReceb;
        }

        public override IEnumerable<Carta> GetAll(long usuarioID)
        {
            List<Carta> lCartas = [];


            var param = new SqlParameter("@UsuarioID", usuarioID);
            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarCartas, param);
            while (dr.Read())
            {
                var carta = DataMapper.ExecuteMapping<Carta>(dr);
                lCartas.Add(carta);
            }

            return lCartas;
        }

        public long AprovarCarta(long pId, long usuarioID)
        {
            try
            {
                var lstParameters = new List<SqlParameter>
                    {
                        new("@Id", pId),
                        new("@UsuarioID", usuarioID)
                    };
                return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, AprovaCarta, lstParameters.ToArray()));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<Carta> VerificaCartaAguardandoRecebimento(long pIdMembro)
        {
            List<Carta> lCartas = [];

            var param = new SqlParameter("@MembroId", pIdMembro);
            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, VerifCartaAguardReceb, param);
            while (dr.Read())
            {
                var carta = DataMapper.ExecuteMapping<Carta>(dr);
                lCartas.Add(carta);
            }

            return lCartas;
        }

        public IEnumerable<Carta> GetAllTipoEStatus(Domain.Types.TipoDeCarta? pTipoCarta, Domain.Types.StatusCarta? pStatusCarta, long pUsuarioID)
        {
            List<Carta> lCartas = [];

            var lstParameters = new List<SqlParameter>
                    {
                        new("@TipoCarta", pTipoCarta != null? Convert.ToInt16(pTipoCarta) : 0),
                        new("@StatusCarta", pStatusCarta != null? Convert.ToInt16(pStatusCarta) : 0),
                        new("@UsuarioID", pUsuarioID)
                    };
            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarPorTipoEStatus, lstParameters.ToArray());
            while (dr.Read())
            {
                var carta = DataMapper.ExecuteMapping<Carta>(dr);
                lCartas.Add(carta);
            }

            return lCartas;
        }

        public bool TransferirSemCarta(IEnumerable<Membro> membros, long congregacaoDestino, long pUsuarioID)
        {
            using SqlConnection con = new(ConnectionString);
            con.Open();
            SqlTransaction sqlTran = con.BeginTransaction();

            foreach (var m in membros)
            {
                try
                {
                    var lstParameters = new List<SqlParameter>
                            {
                                new("@MembroID", m.Id),
                                new("@CongregacaoDestId", congregacaoDestino),
                                new("@UsuarioID", pUsuarioID)
                            };
                    MicrosoftSqlHelper.ExecuteScalar(sqlTran, CommandType.StoredProcedure, AlterarCongregacaoMembro, lstParameters.ToArray());
                }
                catch
                {
                    sqlTran.Rollback();
                    throw;
                }
            }
            sqlTran.Commit();
            return true;
        }

        public IEnumerable<Carta> ListarCartaPaginado(int pageSize, int rowStart, string sorting, string campo, string valor, StatusCarta? statusCarta, long usuarioID, out int rowCount)
        {
            rowCount = 0;
            List<Carta> lstCarta = [];

            var _campo = !string.IsNullOrWhiteSpace(campo) ? (object)campo : DBNull.Value;
            var _valor = !string.IsNullOrWhiteSpace(valor) ? (object)valor : DBNull.Value;
            var _status = statusCarta != null ? (object)statusCarta : DBNull.Value;

            var lstParameters = new List<SqlParameter>
                    {
                        new("@PAGESIZE", pageSize),
                        new("@ROWSTART", rowStart),
                        new("@SORTING", sorting),
                        new("@USUARIOID", usuarioID),
                        new("@CAMPO", _campo),
                        new("@VALOR", _valor),
                        new("@STATUSCARTA", _status)
                    };
            try
            {
                using DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, CartaPaginada, lstParameters.ToArray());
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var carta = new Carta()
                        {
                            Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                            Nome = ds.Tables[0].Rows[i]["Nome"].ToString(),
                            CongregacaoOrigem = ds.Tables[0].Rows[i]["CongregacaoOrigem"].ToString(),
                            CongregacaoDestId = Convert.ToInt16(ds.Tables[0].Rows[i]["CongregacaoDestId"]),
                            CongregacaoDest = ds.Tables[0].Rows[i]["CongregacaoDestino"].ToString(),
                            TipoCarta = ds.Tables[0].Rows[i]["TipoCarta"].ToString().ToEnum<TipoDeCarta>(),
                            StatusCarta = ds.Tables[0].Rows[i]["StatusCarta"].ToString().ToEnum<StatusCarta>(),
                            DataValidade = Convert.ToDateTime(ds.Tables[0].Rows[i]["DataValidade"].ToString()),
                            TemplateId = Convert.ToInt16(ds.Tables[0].Rows[i]["TemplateId"])
                        };
                        lstCarta.Add(carta);
                    }
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    rowCount = Convert.ToInt16(ds.Tables[1].Rows[0][0]);
                }

            }
            catch
            {
                throw;
            }

            return lstCarta;
        }

        public bool CancelarCarta(long pId, long usuarioID, out string erro)
        {
            try
            {
                erro = "";
                var lstParameters = new List<SqlParameter>
                    {
                        new("@IdCarta", pId),
                        new("@UsuarioID", usuarioID)
                    };
                using DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, CancCarta, lstParameters.ToArray());
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    throw new Erro($"Falha ao cancelar a Carta - Erro: {ds.Tables[0].Rows[0]["ErrorMessage"]}");
                return true;
            }
            catch (Erro ex)
            {
                erro = ex.Message;
                return false;
            }
        }

        public IEnumerable<Carta> ListarCartasPorMembroId(long membroId)
        {
            List<Carta> lCartas = [];

            var param = new SqlParameter("@MembroId", membroId);
            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarCartasMembroId, param);
            while (dr.Read())
            {
                var carta = DataMapper.ExecuteMapping<Carta>(dr);
                lCartas.Add(carta);
            }

            return lCartas;
        }
        #endregion
    }
}
