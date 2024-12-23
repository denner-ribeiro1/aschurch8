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

    public class EventosRepository : RepositoryDAO<Evento>, IEventosRepository
    {
        #region Variaveis Private
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        #endregion

        #region Construtor
        public EventosRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Constantes
        private const string ListarEventos = "ListarEventos";
        private const string SalvarEvento = "SalvarEvento";
        private const string ConsultarEvento = "ConsultarEvento";
        private const string DeletarEventos = "DeletarEvento";
        private const string ListarEventosObrig = "ListarEventosObrigatorio";
        private const string ListarEventosData = "ListarEventosPorData";
        #endregion

        #region Metodos Public
        public override long Add(Evento entity, long usuarioID = 0)
        {
            try
            {
                var lstParans = new List<SqlParameter>
                 {
                      new("@Id", entity.Id),
                         new("@CongregacaoId", entity.CongregacaoId),
                         new("@TipoEventoId", entity.TipoEventoId),
                         new("@Descricao", entity.Descricao),
                         new("@DataHoraInicio", entity.DataHoraInicio),
                         new("@DataHoraFim", entity.DataHoraFim),
                         new("@Observacoes", entity.Observacoes),
                         new("@Frequencia", entity.Frequencia),
                         new("@Quantidade", entity.Quantidade),
                         new("@AlertarEventoMesmoDia", entity.AlertarEventoMesmoDia)
                 };

                return Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure, SalvarEvento,
                    lstParans.ToArray()));
            }
            catch
            {
                throw;
            }
        }

        public override long Update(Evento entity, long usuarioID = 0)
        {
            return Add(entity);
        }

        public override int Delete(Evento entity, long usuarioID = 0)
        {
            return Delete(entity.Id);
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            return Delete(id);
        }

        public int Delete(long id, bool excluirVinc = false)
        {
            var lstParameters = new List<SqlParameter>
                 {
                     new("@Id",id),
                     new("@ExcluirVinc", excluirVinc)
                 };

            return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(ConnectionString, CommandType.StoredProcedure,
                                                                    DeletarEventos, lstParameters.ToArray()));
        }

        public override Evento GetById(long id, long usuarioID)
        {
            var evento = new Evento();

            var lstParameters = new List<SqlParameter>
                 {
                     new("@Id",id),
                     new("@UsuarioID", usuarioID)
                 };

            using var dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure,
                                                                ConsultarEvento, lstParameters.ToArray());
            while (dr.Read())
            {
                evento.Id = dr["Id"].TryConvertTo<long>();
                evento.IdEventoOriginal = dr["IdEventoOriginal"].TryConvertTo<int>();
                evento.CongregacaoId = dr["CongregacaoId"].TryConvertTo<int>();
                evento.TipoEventoId = dr["TipoEventoId"].TryConvertTo<int>();
                evento.Descricao = dr["Descricao"].ToString();

                if (!string.IsNullOrEmpty(dr["DataHoraInicio"].ToString()))
                    evento.DataHoraInicio = Convert.ToDateTime(dr["DataHoraInicio"].ToString().Replace("+00:00", "").Replace("+03:00", ""));

                if (!string.IsNullOrEmpty(dr["DataHoraFim"].ToString()))
                    evento.DataHoraFim = Convert.ToDateTime(dr["DataHoraFim"].ToString().Replace("+00:00", "").Replace("+03:00", ""));

                evento.Observacoes = dr["Observacoes"].ToString();
                evento.DataCriacao = dr["DataCriacao"].TryConvertTo<DateTimeOffset>();
                evento.DataAlteracao = dr["DataAlteracao"].TryConvertTo<DateTimeOffset>();
                evento.Congregacao.Id = Convert.ToInt64(dr["CongregacaoId"].ToString());
                evento.Congregacao.Nome = dr["CongregacaoNome"].TryConvertTo<string>();
                evento.Frequencia = (Evento.TipoFrequencia)Enum.Parse(typeof(Evento.TipoFrequencia), dr["Frequencia"].ToString());
                evento.Quantidade = dr["Quantidade"].TryConvertTo<int>();
                evento.AlertarEventoMesmoDia = dr["AlertarEventoMesmoDia"].TryConvertTo<bool>();

            }
            return evento;
        }

        public override IEnumerable<Evento> GetAll(long usuarioID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Evento> GetEventos(int ano, int mes, int tipoPesquisa, int codigoCongr, out List<Feriado> feriados)
        {
            var lstEventos = new List<Evento>();
            feriados = [];

            var lstParameters = new List<SqlParameter>
                 {
                     new("@MESREF", mes),
                     new("@ANOREF", ano),
                     new("@TIPOPESQUISA", tipoPesquisa),
                     new("@CODIGOCONGR", codigoCongr)
                 };

            using var ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ListarEventos, lstParameters.ToArray());
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var evento = new Evento()
                    {
                        Descricao = ds.Tables[0].Rows[i]["Descricao"].ToString(),
                        Observacoes = ds.Tables[0].Rows[i]["Observacoes"].ToString(),

                    };
                    bool.TryParse(ds.Tables[0].Rows[i]["AlertarEventoMesmoDia"].ToString(), out bool alertar);
                    evento.AlertarEventoMesmoDia = alertar;

                    int.TryParse(ds.Tables[0].Rows[i]["CongregacaoId"].ToString(), out int congId);
                    bool.TryParse(ds.Tables[0].Rows[i]["CongregacaoSede"].ToString(), out bool sede);
                    evento.Congregacao = new Congregacao()
                    {
                        Id = congId,
                        Nome = ds.Tables[0].Rows[i]["CongregacaoNome"].ToString(),
                        Sede = sede

                    };
                    evento.CongregacaoId = congId;
                    DateTime.TryParse(ds.Tables[0].Rows[i]["DataHoraFim"].ToString(), out DateTime dataFim);
                    evento.DataHoraFim = dataFim;
                    DateTime.TryParse(ds.Tables[0].Rows[i]["DataHoraInicio"].ToString(), out DateTime dataIni);
                    evento.DataHoraInicio = dataIni;
                    Enum.TryParse(ds.Tables[0].Rows[i]["Frequencia"].ToString(), out Evento.TipoFrequencia freq);
                    evento.Frequencia = freq;
                    int.TryParse(ds.Tables[0].Rows[i]["Id"].ToString(), out int id);
                    evento.Id = id;
                    int.TryParse(ds.Tables[0].Rows[i]["IdEventoOriginal"].ToString(), out int idEvOri);
                    evento.IdEventoOriginal = idEvOri;
                    int.TryParse(ds.Tables[0].Rows[i]["Quantidade"].ToString(), out int qtd);
                    evento.Quantidade = qtd;
                    Enum.TryParse(ds.Tables[0].Rows[i]["TipoEvento"].ToString(), out Evento.TipoEvento tpEv);
                    evento.Tipo = tpEv;
                    int.TryParse(ds.Tables[0].Rows[i]["TipoEventoId"].ToString(), out int tpEvId);
                    evento.TipoEventoId = tpEvId;
                    evento.DescrTipoEventoId = ds.Tables[0].Rows[i]["DescrTipoEventoId"].ToString();

                    lstEventos.Add(evento);
                }
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    var feriado = new Feriado()
                    {
                        DataFeriado = Convert.ToDateTime(ds.Tables[1].Rows[i]["DataFeriado"]),
                        Descricao = ds.Tables[1].Rows[i]["Descricao"].ToString(),
                    };
                    feriados.Add(feriado);
                }
            }
            return lstEventos;
        }

        public IEnumerable<Evento> ListarEventosObrigatorio(DateTimeOffset dataHoraInicio, DateTimeOffset dataHoraFim, int congregacao,
            Evento.TipoFrequencia frequencia, int quantidade)
        {
            var lstEventos = new List<Evento>();

            var lstParameters = new List<SqlParameter>
                 {
                     new("@DataHoraInicio", dataHoraInicio),
                     new("@DataHoraFim", dataHoraFim),
                     new("@Congregacao", congregacao),
                     new("@Frequencia", frequencia),
                     new("@Quantidade", quantidade),
                 };


            using var dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, ListarEventosObrig, lstParameters.ToArray());
            while (dr.Read())
            {
                var evento = DataMapper.ExecuteMapping<Evento>(dr);
                evento.Tipo = (Evento.TipoEvento)dr["TipoEvento"].TryConvertTo<int>();

                lstEventos.Add(evento);
            }
            return lstEventos;
        }

        public IEnumerable<Evento> ListarEventosPorData(DateTimeOffset dataHoraInicio, DateTimeOffset dataHoraFim, out List<Feriado> feriados)
        {
            var lstEventos = new List<Evento>();
            feriados = [];

            var lstParameters = new List<SqlParameter>
                 {
                     new("@DataIni", dataHoraInicio),
                     new("@DataFim", dataHoraFim),
                 };

            using var ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, ListarEventosData, lstParameters.ToArray());
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var evento = new Evento()
                    {
                        Descricao = ds.Tables[0].Rows[i]["Descricao"].ToString(),
                        Observacoes = ds.Tables[0].Rows[i]["Observacoes"].ToString(),

                    };
                    bool.TryParse(ds.Tables[0].Rows[i]["AlertarEventoMesmoDia"].ToString(), out bool alertar);
                    evento.AlertarEventoMesmoDia = alertar;

                    int.TryParse(ds.Tables[0].Rows[i]["CongregacaoId"].ToString(), out int congId);
                    bool.TryParse(ds.Tables[0].Rows[i]["CongregacaoSede"].ToString(), out bool sede);
                    evento.Congregacao = new Congregacao()
                    {
                        Id = congId,
                        Nome = ds.Tables[0].Rows[i]["CongregacaoNome"].ToString(),
                        Sede = sede
                    };
                    evento.CongregacaoId = congId;
                    DateTime.TryParse(ds.Tables[0].Rows[i]["DataHoraFim"].ToString().Replace("+00:00", "").Replace("+03:00", ""), out DateTime dataFim);
                    evento.DataHoraFim = dataFim;
                    DateTime.TryParse(ds.Tables[0].Rows[i]["DataHoraInicio"].ToString().Replace("+00:00", "").Replace("+03:00", ""), out DateTime dataIni);
                    evento.DataHoraInicio = dataIni;
                    Enum.TryParse(ds.Tables[0].Rows[i]["Frequencia"].ToString(), out Evento.TipoFrequencia freq);
                    evento.Frequencia = freq;
                    int.TryParse(ds.Tables[0].Rows[i]["Id"].ToString(), out int id);
                    evento.Id = id;
                    int.TryParse(ds.Tables[0].Rows[i]["IdEventoOriginal"].ToString(), out int idEvOri);
                    evento.IdEventoOriginal = idEvOri;
                    int.TryParse(ds.Tables[0].Rows[i]["Quantidade"].ToString(), out int qtd);
                    evento.Quantidade = qtd;
                    Enum.TryParse(ds.Tables[0].Rows[i]["TipoEvento"].ToString(), out Evento.TipoEvento tpEv);
                    evento.Tipo = tpEv;
                    int.TryParse(ds.Tables[0].Rows[i]["TipoEventoId"].ToString(), out int tpEvId);
                    evento.TipoEventoId = tpEvId;
                    evento.DescrTipoEventoId = ds.Tables[0].Rows[i]["DescrTipoEventoId"].ToString();

                    lstEventos.Add(evento);
                }
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    var feriado = new Feriado()
                    {
                        DataFeriado = Convert.ToDateTime(ds.Tables[1].Rows[i]["DataFeriado"]),
                        Descricao = ds.Tables[1].Rows[i]["Descricao"].ToString(),
                    };
                    feriados.Add(feriado);
                }
            }
            return lstEventos;
        }
        #endregion
    }
}
