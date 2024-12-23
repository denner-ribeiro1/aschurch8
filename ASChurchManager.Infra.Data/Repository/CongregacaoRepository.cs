using ASBaseLib.Data.Helpers.Microsoft.ApplicationBlocks;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Types;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ASChurchManager.Infra.Data.Repository.EnterpriseLibrary
{
    public class CongregacaoRepository : RepositoryDAO<Congregacao>, ICongregacaoRepository
    {
        #region Variaveis Private
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        #endregion

        #region Construtor
        public CongregacaoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Constantes 
        private const string ProcSalvarCongregacao = "dbo.SalvarCongregacao";
        private const string ProcListarCongregacoes = "dbo.ListarCongregacoes";
        private const string ProcConsultarCongregacao = "dbo.ConsultarCongregacao";
        private const string ProcDeletarCongregacao = "dbo.DeletarCongregacao";
        private const string ProcDefinirCongregacaoSede = "dbo.DefinirCongregacaoSede";
        private const string ConsultarCongregacaoSemFiltroUsuario = "dbo.ConsultarCongregacaoSemFiltroUsuario";
        private const string ListarCongregacoesSemFiltroUsuario = "dbo.ListarCongregacoesSemFiltroUsuario";
        private const string AdicionarGrupoGongregacao = "SalvarCongregacaoGrupo";
        private const string ListarGruposCongregacao = "ListarCongregacaoGrupo";
        private const string ExcluirGrupoCongregacao = "DeletarCongregacaoGrupo";
        private const string AdicionarObreiroCongregacao = "SalvarCongregacaoObreiro";
        private const string AdicionarObservacaoCongregacao = "SalvarCongregacaoObservacao";
        private const string ListarObreiroCongregacao = "ListarCongregacaoObreiro";
        private const string ListarCongregacaoObservacao = "ListarCongregacaoObservacao";
        private const string ExcluirObreiroCongregacao = "DeletarCongregacaoObreiro";
        private const string ExcluirObservacaoCongregacao = "DeletarCongregacaoObservacao";
        private const string ListarObreirosCongrMembroId = "ListarObreirosCongrMembroId";
        private const string BuscarCongregacoes = "BuscarCongregacoes";
        private const string CongregacaoPaginada = "ListarCongregacaoPaginada";
        private const string BuscarCongregacaoSede = "BuscarCongregacaoSede";
        private const string ConsultaQtdMembPorCongreg = "ConsultarQtdMembrosCongregacao";
        #endregion

        #region Metodos Public
        public override long Add(Congregacao entity, long usuarioID = 0)
        {
            using SqlConnection con = new(ConnectionString);
            con.Open();
            SqlTransaction sqlTran = con.BeginTransaction();

            try
            {
                var lparams = new List<SqlParameter>
                    {
                        new("@Id", entity.Id),
                        new("@Nome", entity.Nome),
                        new("@Logradouro", entity.Endereco.Logradouro),
                        new("@Numero", entity.Endereco.Numero),
                        new("@Complemento", entity.Endereco.Complemento),
                        new("@Bairro", entity.Endereco.Bairro),
                        new("@Cidade", entity.Endereco.Cidade),
                        new("@Estado", entity.Endereco.Estado),
                        new("@Pais", entity.Endereco.Pais),
                        new("@Cep", entity.Endereco.Cep),
                        new("@CongregacaoResponsavelId", entity.CongregacaoResponsavelId),
                        new("@PastorResponsavelId", entity.PastorResponsavelId),
                        new("@CNPJ", entity.CNPJ)
                    };

                long CongregacaoId = Convert.ToInt64(MicrosoftSqlHelper.ExecuteScalar(sqlTran, CommandType.StoredProcedure, ProcSalvarCongregacao, lparams.ToArray()));
                entity.Id = CongregacaoId;

                var paramCong = new List<SqlParameter>
                    {
                        new("@CongregacaoId", entity.Id)
                    };

                MicrosoftSqlHelper.ExecuteScalar(sqlTran, CommandType.StoredProcedure, ExcluirGrupoCongregacao, paramCong.ToArray());
                MicrosoftSqlHelper.ExecuteScalar(sqlTran, CommandType.StoredProcedure, ExcluirObreiroCongregacao, paramCong.ToArray());
                MicrosoftSqlHelper.ExecuteScalar(sqlTran, CommandType.StoredProcedure, ExcluirObservacaoCongregacao, paramCong.ToArray());

                foreach (var cong in entity.Grupos)
                {
                    var paramGrp = new List<SqlParameter>
                        {
                            new("@CongregacaoId", entity.Id),
                            new("@GrupoId", cong.GrupoId),
                            new("@NomeGrupo", cong.NomeGrupo),
                            new("@ResponsavelId", cong.ResponsavelId)
                        };
                    MicrosoftSqlHelper.ExecuteScalar(sqlTran, CommandType.StoredProcedure, AdicionarGrupoGongregacao, paramGrp.ToArray());
                };

                foreach (var obreiros in entity.Obreiros)
                {
                    var paraObr = new List<SqlParameter>
                        {
                            new("@CongregacaoId", entity.Id),
                            new("@MembroId", obreiros.ObreiroId),
                            new("@Dirigente", 0)
                        };
                    MicrosoftSqlHelper.ExecuteScalar(sqlTran, CommandType.StoredProcedure, AdicionarObreiroCongregacao, paraObr.ToArray());
                }

                //INICIO - Tratamento para Incluir o Pastor Responsavel como Dirigente na Lista de CongregacaoObreiro
                if (entity.PastorResponsavelId > 0)
                {
                    var paraDir = new List<SqlParameter>
                        {
                            new("@CongregacaoId", entity.Id),
                            new("@MembroId", entity.PastorResponsavelId),
                            new("@Dirigente", 1)
                        };
                    MicrosoftSqlHelper.ExecuteScalar(sqlTran, CommandType.StoredProcedure, AdicionarObreiroCongregacao, paraDir.ToArray());
                }
                //FIM Tratamento para Incluir o Pastor Responsavel como Dirigente na Lista de CongregacaoObreiro

                foreach (var obs in entity.Observacoes)
                {
                    var dtCad = obs.DataCadastro == DateTime.MinValue ? (object)DBNull.Value : obs.DataCadastro;
                    var paraObs = new List<SqlParameter>
                        {
                            new("@CongregacaoId", entity.Id),
                            new("@Observacao", obs.Observacao),
                            new("@UsuarioId", obs.Usuario.Id),
                            new("@DataCadastro", dtCad)
                        };
                    MicrosoftSqlHelper.ExecuteScalar(sqlTran, CommandType.StoredProcedure, AdicionarObservacaoCongregacao, paraObs.ToArray());
                }

                sqlTran.Commit();
                return CongregacaoId;
            }
            catch (Exception)
            {
                sqlTran.Rollback();
                throw;
            }
        }

        public override long Update(Congregacao entity, long usuarioID = 0)
        {
            try
            {
                return Add(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override int Delete(Congregacao entity, long usuarioID = 0)
        {
            return Delete(entity.Id);
        }

        public override int Delete(long id, long usuarioID = 0)
        {
            return Delete(id, 0, usuarioID);
        }

        public int Delete(long id, long congregacaoId, long usuarioID)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@Id", id),
                    new("@CongregacaoDestId", congregacaoId),
                    new("@UsuarioID", usuarioID)
                };

            return Convert.ToInt16(MicrosoftSqlHelper.ExecuteNonQuery(this.ConnectionString, CommandType.StoredProcedure, ProcDeletarCongregacao, lstParameters.ToArray()));
        }

        public override Congregacao GetById(long id, long usuarioID = 0)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@Id", id),
                    new("@UsuarioID", usuarioID)
                };

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ProcConsultarCongregacao, lstParameters.ToArray());
            var congregacao = new Congregacao();

            while (dr.Read())
            {
                congregacao = DataMapper.ExecuteMapping<Congregacao>(dr);
                congregacao.Endereco = DataMapper.ExecuteMapping<Endereco>(dr);
            }
            congregacao.Grupos = ListarGrupoCongregacao(congregacao.Id).ToList();
            congregacao.Obreiros = ListarObreirosCongregacao(congregacao.Id).ToList();
            congregacao.Observacoes = ListarObservacaoCongregacao(congregacao.Id).ToList();
            return congregacao;
        }


        public override IEnumerable<Congregacao> GetAll(long usuarioID)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@UsuarioID", usuarioID)
                };

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ProcListarCongregacoes, lstParameters.ToArray());
            var lstCongregacao = new List<Congregacao>();
            while (dr.Read())
            {
                var congregacao = DataMapper.ExecuteMapping<Congregacao>(dr);
                congregacao.Endereco = DataMapper.ExecuteMapping<Endereco>(dr);
                lstCongregacao.Add(congregacao);
            }
            return lstCongregacao;
        }

        public short DefinirSede(long id)
        {
            try
            {
                var param = new SqlParameter("@CongregacaoId", id);
                return Convert.ToInt16(MicrosoftSqlHelper.ExecuteScalar(this.ConnectionString, CommandType.StoredProcedure, ProcDefinirCongregacaoSede, param));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<Congregacao> GetAll(bool completo = true)
        {
            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ListarCongregacoesSemFiltroUsuario, null);
            var lstCongregacao = new List<Congregacao>();
            while (dr.Read())
            {
                var congregacao = DataMapper.ExecuteMapping<Congregacao>(dr);
                congregacao.Endereco = DataMapper.ExecuteMapping<Endereco>(dr);

                if (completo)
                {
                    congregacao.Grupos = ListarGrupoCongregacao(congregacao.Id).ToList();
                    congregacao.Obreiros = ListarObreirosCongregacao(congregacao.Id).ToList();
                    congregacao.Observacoes = ListarObservacaoCongregacao(congregacao.Id).ToList();

                }
                lstCongregacao.Add(congregacao);
            }
            return lstCongregacao;
        }

        public Congregacao GetById(long id)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@Id", id)
                };

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ConsultarCongregacaoSemFiltroUsuario, lstParameters.ToArray());
            var congregacao = new Congregacao();

            while (dr.Read())
            {
                congregacao = DataMapper.ExecuteMapping<Congregacao>(dr);
                congregacao.Endereco = DataMapper.ExecuteMapping<Endereco>(dr);
            }
            congregacao.Grupos = ListarGrupoCongregacao(congregacao.Id).ToList();
            congregacao.Obreiros = ListarObreirosCongregacao(congregacao.Id).ToList();
            congregacao.Observacoes = ListarObservacaoCongregacao(congregacao.Id).ToList();

            return congregacao;

        }
        #endregion

        public IEnumerable<CongregacaoGrupo> ListarGrupoCongregacao(long congregacaoId)
        {
            var grupos = new List<CongregacaoGrupo>();

            var param = new SqlParameter("@CongregacaoId", congregacaoId);
            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ListarGruposCongregacao, param))
            {
                while (dr.Read())
                    grupos.Add(DataMapper.ExecuteMapping<CongregacaoGrupo>(dr));
            }

            return grupos;
        }

        public IEnumerable<CongregacaoObreiro> ListarObreirosCongregacao(long congregacaoId)
        {
            var obreiros = new List<CongregacaoObreiro>();

            var param = new SqlParameter("@CongregacaoId", congregacaoId);
            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ListarObreiroCongregacao, param))
            {
                while (dr.Read())
                {
                    obreiros.Add(new CongregacaoObreiro()
                    {
                        CongregacaoId = Convert.ToInt64(dr["CongregacaoId"].ToString()),
                        ObreiroId = Convert.ToInt64(dr["ObreiroId"].ToString()),
                        ObreiroNome = dr["ObreiroNome"].ToString(),
                        ObreiroCargo = dr["ObreiroCargo"].ToString()
                    });
                }
            }

            return obreiros;
        }

        public IEnumerable<ObservacaoCongregacao> ListarObservacaoCongregacao(long congregacaoId)
        {
            var obs = new List<ObservacaoCongregacao>();
            var param = new SqlParameter("@CongregacaoId", congregacaoId);
            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ListarCongregacaoObservacao, param))
            {
                while (dr.Read())
                {
                    obs.Add(new ObservacaoCongregacao()
                    {
                        CongregacaoId = Convert.ToInt64(dr["CongregacaoId"].ToString()),
                        Usuario = new Usuario()
                        {
                            Id = Convert.ToInt64(dr["UsuarioId"].ToString()),
                            Nome = dr["UsuarioNome"].ToString()
                        },
                        Observacao = dr["Observacao"].ToString(),
                        DataCadastro = dr["DataCadastro"].TryConvertTo<DateTime>()
                    });
                }
            }

            return obs;
        }

        public FichaCongregacao RelatorioFichaCongregacao(long id)
        {
            var rel = new FichaCongregacao();

            var congr = GetById(id);

            if (congr.Id == id)
            {
                rel.Congregacao.Add(congr);
                congr.Grupos.ForEach(g => rel.Grupos.Add(g));
                congr.Obreiros.ForEach(o => rel.Obreiros.Add(o));
            }
            return rel;
        }

        public IEnumerable<CongregacaoObreiro> ListarObreirosCongregacaoPorMembroId(long membroId)
        {
            var obreiros = new List<CongregacaoObreiro>();

            var param = new SqlParameter("@MembroId", membroId);
            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ListarObreirosCongrMembroId, param))
            {
                while (dr.Read())
                {
                    obreiros.Add(new CongregacaoObreiro()
                    {
                        CongregacaoId = Convert.ToInt64(dr["CongregacaoId"].ToString()),
                        ObreiroId = Convert.ToInt64(dr["ObreiroId"].ToString()),
                        CongregacaoNome = dr["CongregacaoNome"].ToString(),
                        ObreiroNome = dr["ObreiroNome"].ToString(),
                        ObreiroCargo = dr["ObreiroCargo"].ToString()
                    });
                }
            }

            return obreiros;
        }

        public IEnumerable<Congregacao> BuscarCongregacao(int pageSize, int rowStart, string sorting, out int rowCount, string campo, string valor, long usuarioId)
        {
            rowCount = 0;
            var lstCongr = new List<Congregacao>();
            var _campo = !string.IsNullOrWhiteSpace(campo) ? (object)campo : DBNull.Value;
            var _valor = !string.IsNullOrWhiteSpace(valor) ? (object)valor : DBNull.Value;

            var lstParameters = new List<SqlParameter>
                {
                    new("@PAGESIZE", pageSize),
                    new("@ROWSTART", rowStart),
                    new("@SORTING", sorting),
                    new("@CAMPO", _campo),
                    new("@VALOR", _valor),
                    new("@USUARIOID", usuarioId)
                };


            using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, BuscarCongregacoes, lstParameters.ToArray()))
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var congr = new Congregacao()
                        {
                            Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                            Nome = ds.Tables[0].Rows[i]["Nome"].ToString()
                        };
                        lstCongr.Add(congr);
                    }
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    rowCount = Convert.ToInt16(ds.Tables[1].Rows[0][0]);
                }
            }

            return lstCongr;
        }

        public IEnumerable<Congregacao> ListarCongregacaoPaginado(int pageSize, int rowStart, string sorting, string filtro, string conteudo, long usuarioID, out int rowCount)
        {
            rowCount = 0;
            var lCongr = new List<Congregacao>();

            var lstParameters = new List<SqlParameter>
                {
                    new("@PAGESIZE", pageSize),
                    new("@ROWSTART", rowStart),
                    new("@SORTING", sorting),
                    new("@USUARIOID", usuarioID),
                    new("@CAMPO", filtro),
                    new("@VALOR", conteudo)
                };

            try
            {
                using DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, CongregacaoPaginada, lstParameters.ToArray());
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var congr = new Congregacao()
                        {
                            Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                            Nome = ds.Tables[0].Rows[i]["Nome"].ToString(),
                            CongregacaoResponsavelNome = ds.Tables[0].Rows[i]["CongregacaoResponsavelNome"].ToString(),
                            PastorResponsavelNome = ds.Tables[0].Rows[i]["PastorResponsavelNome"].ToString()
                        };
                        lCongr.Add(congr);
                    }
                }
                if (ds.Tables[1].Rows.Count > 0)
                    rowCount = Convert.ToInt16(ds.Tables[1].Rows[0][0]);

            }
            catch
            {
                throw;
            }

            return lCongr;
        }

        public Congregacao GetSede()
        {

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, BuscarCongregacaoSede);
            var congregacao = new Congregacao();

            while (dr.Read())
            {
                congregacao = DataMapper.ExecuteMapping<Congregacao>(dr);
                congregacao.Endereco = DataMapper.ExecuteMapping<Endereco>(dr);
            }
            congregacao.Grupos = ListarGrupoCongregacao(congregacao.Id).ToList();
            congregacao.Obreiros = ListarObreirosCongregacao(congregacao.Id).ToList();
            congregacao.Observacoes = ListarObservacaoCongregacao(congregacao.Id).ToList();

            return congregacao;

        }

        public IEnumerable<QuantidadePorCongregacao> ConsultarQtdMembrosCongregacao(long congregacaoId)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@Id", congregacaoId)
                };

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(this.ConnectionString, CommandType.StoredProcedure, ConsultaQtdMembPorCongreg, lstParameters.ToArray());
            var retorno = new List<QuantidadePorCongregacao>();

            while (dr.Read())
            {
                var situacao = DataMapper.ExecuteMapping<QuantidadePorCongregacao>(dr);
                retorno.Add(situacao);
            }

            return retorno;
        }
    }
}