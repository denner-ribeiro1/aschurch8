using ASBaseLib.Data.Helpers.Microsoft.ApplicationBlocks;
using ASChurchManager.Domain.Entities.Relatorios.Secretaria;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Types;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ASChurchManager.Infra.Data.Repository
{
    public class RelatoriosSecretariaRepository : IRelatoriosSecretariaRepository
    {
        private readonly IConfiguration _configuration;
        private string ConnectionString => _configuration["ConnectionStrings:ConnectionDB"];
        private readonly IMembroRepository _membroRepository;
        
        public RelatoriosSecretariaRepository(IConfiguration configuration, IMembroRepository membroRepository)
        {
            _configuration = configuration;
            _membroRepository = membroRepository;
        }

        #region Constantes
        private const string RelatorioAniversariantes = "RelatorioAniversariantes";
        private const string RelatorioTransferencia = "RelatorioHistoricoTrans";
        private const string RelatorioNascimentos = "RelatorioNascimentos";
        private const string RelatorioCasamentos = "RelatorioCasamentos";
        private const string RelatorioCongregacoes = "RelatorioCongregacoes";
        private const string RelatorioObreiros = "RelatorioObreiros";
        private const string RelMensalCasamento = "RelMensalCasamento";
        private const string RelMensalReconciliacao = "RelMensalReconciliacao";
        private const string RelMensalRecebidoMudanca = "RelMensalRecebidoCartaMudança";
        private const string RelMensalRecebidoAclamacao = "RelMensalRecebidoAclamacao";
        private const string RelMensalRecebidoTransferencia = "RelMensalRecebidoTranferencia";
        private const string RelMensalSaidaPor = "RelMensalSaidaPor";
        private const string RelMensalFuneral = "RelMensalFuneral";
        private const string RelMensalSaidaTranferencia = "RelMensalSaidaTransferencia";
        private const string RelMensalSaidaMudanca = "RelMensalSaidaMudanca";
        private const string RelMensalCriancasApresentadas = "RelMensalCriancasApresentadas";
        private const string RelMensalTotalMembros = "RelMensalTotalMembros";
        private const string RelMensalTotalCriancasApresentadas = "RelMensalTotalCriancasApresentadas";
        private const string RelMensalTotalCriancas = "RelMensalTotalCriancas";
        private const string RelMensalTotalCongregados = "RelMensalTotalCongregados";
        private const string RelMensalTotalPessoasAceitaramJesus = "RelMensalPessoasAceitaramJesus";
        private const string RelMensalTotalDemissoes = "RelMensalTotalDemissoes";
        private const string RelMensalTotalAdesoes = "RelMensalTotalAdesoes";
        private const string RelMensalTotRecAclamacao = "RelMensalTotalRecAclamacao";
        private const string RelMensalTotalRecebidoMudanca = "RelMensalTotalRecMudanca";
        private const string RelMensalTotalRecBatismo = "RelMensalTotalBatismo";
        private const string RelMembros = "ListarMembrosCompleta";
        private const string RelCursosMembro = "RelatorioCursosMembro";
        private const string RelBatismo = "RelBatismo";
        private const string RelBatismoCandidatos = "RelBatismoCandidatos";
        private const string RelBatismoCelebrantes = "RelBatismoCelebrantes";
        private const string RelEventos = "RelatorioEventos";
        private const string RelPresencaLista = "RelPresencaLista";
        private const string RelPresencaInscrito = "RelPresencaInscrito";
        private const string RelPresencaFrequenciaLista = "RelPresencaFrequenciaLista";
        private const string RelListaCarteirinhasGrid = "ListaCarteirinhasGrid";
        #endregion

        #region Publicos
        public IEnumerable<Aniversariantes> Aniversariantes(DateTimeOffset dataInicio, DateTimeOffset dataFinal, long congregacaoId, long usuarioID, TipoMembro tipoMembro)
        {
            var lAniver = new List<Aniversariantes>();

            var lParans = new List<SqlParameter>
                {
                    new("@DataInicio", dataInicio.Date),
                    new("@DataFinal", dataFinal.Date),
                    new("@Congregacao", congregacaoId),
                    new("@TipoMembro", tipoMembro),
                    new("@UsuarioID", usuarioID)
                };

            SqlDataReader sqlDataReader = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelatorioAniversariantes, lParans.ToArray());
            using SqlDataReader dr = sqlDataReader;
            while (dr.Read())
            {
                var aniver = DataMapper.ExecuteMapping<Aniversariantes>(dr);
                lAniver.Add(aniver);
            }

            return lAniver;
        }

        public IEnumerable<Transferencia> Transferencia(DateTimeOffset dataInicio, DateTimeOffset dataFinal, long congregacaoId, long usuarioID)
        {
            var lTrans = new List<Transferencia>();

            var lParans = new List<SqlParameter>
                {
                    new("@DataInicio", dataInicio),
                    new("@DataFinal", dataFinal),
                    new("@Congregacao", congregacaoId),
                    new("@UsuarioID", usuarioID)
                };

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelatorioTransferencia, lParans.ToArray());
            while (dr.Read())
            {
                var trans = DataMapper.ExecuteMapping<Transferencia>(dr);
                lTrans.Add(trans);
            }

            return lTrans;
        }

        public IEnumerable<Nascimentos> Nascimento(DateTimeOffset dataInicio, DateTimeOffset dataFinal, long congregacaoId, long usuarioID)
        {
            var lNasc = new List<Nascimentos>();

            var lParans = new List<SqlParameter>
                {
                    new("@DataInicio", dataInicio),
                    new("@DataFinal", dataFinal),
                    new("@Congregacao", congregacaoId),
                    new("@UsuarioID", usuarioID)
                };

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelatorioNascimentos, lParans.ToArray());
            while (dr.Read())
            {
                var trans = DataMapper.ExecuteMapping<Nascimentos>(dr);
                trans.Sexo = trans.Sexo == "M" ? "Masculino" : "Feminino";
                lNasc.Add(trans);
            }

            return lNasc;
        }

        public IEnumerable<Congregacoes> Congregacao(long congregacaoId, long usuarioID)
        {
            var lCong = new List<Congregacoes>();

            var lParans = new List<SqlParameter>
                {
                    new("@Congregacao", congregacaoId),
                    new("@UsuarioID", usuarioID)
                };

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelatorioCongregacoes, lParans.ToArray());
            while (dr.Read())
            {
                //var trans = DataMapper.ExecuteMapping<Congregacoes>(dr);
                var cong = new Congregacoes()
                {
                    Id = dr["Id"].ToString(),
                    Congregacao = dr["Congregacao"].ToString(),
                    Dirigente = dr["Dirigente"].ToString(),
                    CNPJ = dr["CNPJ"].ToString(),
                    Logradouro = dr["Logradouro"].ToString(),
                    Numero = dr["Numero"].ToString(),
                    Bairro = dr["Bairro"].ToString(),
                    Cidade = dr["Cidade"].ToString(),
                    Cep = dr["Cep"].ToString(),
                    Estado = dr["Estado"].ToString(),
                    Complemento = dr["Complemento"].ToString(),
                    Pais = dr["Pais"].ToString(),
                    QtdMembrosAtivos = dr["QtdMembrosAtivos"].TryConvertTo<int>(),
                    QtdObreiros = dr["QtdObreiros"].TryConvertTo<int>()
                };

                lCong.Add(cong);
            }

            return lCong;
        }

        public IEnumerable<Obreiros> Obreiros(long congregacaoId, long usuarioID)
        {
            var lObr = new List<Obreiros>();

            var lParans = new List<SqlParameter>
                {
                    new("@Congregacao", congregacaoId),
                    new("@UsuarioID", usuarioID)
                };

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelatorioObreiros, lParans.ToArray());
            while (dr.Read())
            {
                var trans = DataMapper.ExecuteMapping<Obreiros>(dr);
                lObr.Add(trans);
            }

            return lObr;
        }

        public IEnumerable<RelatorioMensal> RelatorioMensal(string mes, int ano, long congregacaoId, long usuarioID)
        {
            var relatorioMensal = new List<RelatorioMensal>();
            var relMensal = new RelatorioMensal();
            var relMensalTotais = new Totais();

            var lParans = new List<SqlParameter>
                {
                    new("@Mes", mes),
                    new("@Ano", ano),
                    new("@CongregacaoId", congregacaoId)
                };
            try
            {
                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalCasamento, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<Domain.Entities.Relatorios.Secretaria.Casamento>(dr);
                        relMensal.Casamento.Add(trans);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalReconciliacao, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<NomeData>(dr);
                        relMensal.Reconciliacao.Add(trans);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalRecebidoMudanca, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<NomeData>(dr);
                        relMensal.RecebidoCartaMudanca.Add(trans);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalRecebidoAclamacao, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<NomeData>(dr);
                        relMensal.RecebidoAclamacao.Add(trans);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalRecebidoTransferencia, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<NomeData>(dr);
                        relMensal.RecebidoTransferencia.Add(trans);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalSaidaPor, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<SaidaPor>(dr);
                        relMensal.SaidaPor.Add(trans);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalFuneral, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<NomeData>(dr);
                        relMensal.Funeral.Add(trans);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalSaidaTranferencia, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<SaidaPorTranferencia>(dr);
                        relMensal.SaidaPorTranferencia.Add(trans);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalSaidaMudanca, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<SaidaPorTranferencia>(dr);
                        relMensal.SaidaPorMudanca.Add(trans);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalCriancasApresentadas, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<CriancasApresentadas>(dr);
                        relMensal.CriancasApresentadas.Add(trans);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalTotalMembros, lParans.ToArray()))
                {
                    while (dr.Read())
                    {

                        var trans = DataMapper.ExecuteMapping<Totais>(dr);
                        relMensalTotais.TotalMembros = Convert.ToInt32(trans.TotalMembros);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalTotalCriancasApresentadas, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<Totais>(dr);
                        relMensalTotais.TotalCriancasApresentadas = Convert.ToInt32(trans.TotalCriancasApresentadas);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalTotalCriancas, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<Totais>(dr);
                        relMensalTotais.TotalCriancas = Convert.ToInt32(trans.TotalCriancas);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalTotalCongregados, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<Totais>(dr);
                        relMensalTotais.TotalCongregados = Convert.ToInt32(trans.TotalCongregados);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalTotalPessoasAceitaramJesus, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<Totais>(dr);
                        relMensalTotais.TotalNovosConvertidos = Convert.ToInt32(trans.TotalNovosConvertidos);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalTotalDemissoes, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<Totais>(dr);
                        relMensalTotais.TotalDemissoes = Convert.ToInt32(trans.TotalDemissoes);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalTotalAdesoes, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<Totais>(dr);
                        relMensalTotais.TotalAdmissoes = Convert.ToInt32(trans.TotalAdmissoes);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalTotRecAclamacao, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<Totais>(dr);
                        relMensalTotais.TotalRecAclamacao = Convert.ToInt32(trans.TotalRecAclamacao);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalTotalRecebidoMudanca, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<Totais>(dr);
                        relMensalTotais.TotalRecMudanca = Convert.ToInt32(trans.TotalRecMudanca);
                    }
                }

                using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMensalTotalRecBatismo, lParans.ToArray()))
                {
                    while (dr.Read())
                    {
                        var trans = DataMapper.ExecuteMapping<Totais>(dr);
                        relMensalTotais.TotalRecebidoBatismo = Convert.ToInt32(trans.TotalRecebidoBatismo);
                    }
                }

                relMensal.Totais.Add(relMensalTotais);
            }
            catch (Exception)
            {
                throw;
            }
            relatorioMensal.Add(relMensal);
            return relatorioMensal;
        }

        public IEnumerable<RelatorioMembros> RelatorioMembros(long congregacaoId, Status status, TipoMembro tipoMembro, EstadoCivil estadoCivil, bool abedabe,
            bool filtrarConf, bool ativosConf, long usuarioId)
        {
            var param = new List<SqlParameter>
                {
                    new("@CongregacaoID", congregacaoId),
                    new("@Status", status),
                    new("@TipoMembro", tipoMembro),
                    new("@EstadoCivil", estadoCivil),
                    new("@ABEDABE", abedabe),
                    new("@UsuarioID", usuarioId),
                    new("@FiltrarConf", filtrarConf),
                    new("@AtivosConf", ativosConf)

                };

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelMembros, param.ToArray());
            var membros = new List<RelatorioMembros>();
            while (dr.Read())
            {
                membros.Add(new RelatorioMembros()
                {
                    CongregacaoNome = dr["CongregacaoNome"].TryConvertTo<string>(),
                    Membro = dr["Membro"].TryConvertTo<string>(),
                    MembroId = dr["MembroId"].TryConvertTo<int>(),
                    MembroNome = dr["MembroNome"].TryConvertTo<string>(),
                    EstadoCivil = dr["EstadoCivil"].TryConvertTo<int>() > 0 ? ((EstadoCivil)dr["EstadoCivil"].TryConvertTo<int>()).GetDisplayAttributeValue() : "",
                    Natural = dr["Natural"].TryConvertTo<string>(),
                    NomeMae = dr["NomeMae"].TryConvertTo<string>(),
                    NomePai = dr["NomePai"].TryConvertTo<string>(),
                    Telefones = dr["Telefones"].TryConvertTo<string>(),
                    Situacao = dr["TipoMembro"].TryConvertTo<int>() == 3 ? dr["Situacao"].TryConvertTo<int>() > 0 ? ((MembroSituacao)dr["Situacao"].TryConvertTo<int>()).GetDisplayAttributeValue()
                                                                                                                  : ""
                                                                         : dr["TipoMembro"].TryConvertTo<int>() > 0 ? ((TipoMembro)dr["TipoMembro"].TryConvertTo<int>()).GetDisplayAttributeValue()
                                                                                                                    : "",
                    Status = dr["Status"].TryConvertTo<int>() > 0 ? ((Status)dr["Status"].TryConvertTo<int>()).GetDisplayAttributeValue() : "",
                    TipoMembro = dr["TipoMembro"].TryConvertTo<int>() > 0 ? ((TipoMembro)dr["TipoMembro"].TryConvertTo<int>()).GetDisplayAttributeValue() : "",
                    DataNascimento = dr["DataNascimento"].TryConvertTo<DateTimeOffset>() != DateTimeOffset.MinValue ? dr["DataNascimento"].TryConvertTo<DateTimeOffset>().ToString("dd/MM/yyyy") : "",
                    MembroAbedabe = dr["MembroAbedabe"].TryConvertTo<string>()
                });
            }
            return membros;

        }

        public IEnumerable<Domain.Entities.CursoMembro> CursosMembro(long congregacaoId, int cursoId, long usuarioID)
        {
            var cursos = new List<Domain.Entities.CursoMembro>();

            var lParans = new List<SqlParameter>
                {
                    new("@CongregacaoId", congregacaoId),
                    new("@CursoId", cursoId),
                    new("@UsuarioID", usuarioID)
                };

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelCursosMembro, lParans.ToArray());
            while (dr.Read())
            {
                var trans = DataMapper.ExecuteMapping<Domain.Entities.CursoMembro>(dr);
                cursos.Add(trans);
            }

            return cursos;
        }

        public IEnumerable<Domain.Entities.Casamento> Casamento(DateTimeOffset dataInicio, DateTimeOffset dataFinal, long congregacaoId, long usuarioID)
        {
            var lCasa = new List<Domain.Entities.Casamento>();

            var lParans = new List<SqlParameter>
                {
                    new("@DataInicio", dataInicio),
                    new("@DataFinal", dataFinal),
                    new("@Congregacao", congregacaoId),
                    new("@UsuarioID", usuarioID)
                };

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelatorioCasamentos, lParans.ToArray());
            while (dr.Read())
            {
                var trans = DataMapper.ExecuteMapping<Domain.Entities.Casamento>(dr);
                lCasa.Add(trans);
            }

            return lCasa;
        }

        IEnumerable<Casamento> IRelatoriosSecretariaRepository.Casamento(DateTimeOffset dataInicio, DateTimeOffset dataFinal, long congregacaoId, long usuarioID)
        {
            var lCasa = new List<Domain.Entities.Relatorios.Secretaria.Casamento>();

            var lParans = new List<SqlParameter>
                {
                    new("@DataInicio", dataInicio),
                    new("@DataFinal", dataFinal),
                    new("@Congregacao", congregacaoId),
                    new("@UsuarioID", usuarioID)
                };

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelatorioCasamentos, lParans.ToArray());
            while (dr.Read())
            {
                var trans = DataMapper.ExecuteMapping<Domain.Entities.Relatorios.Secretaria.Casamento>(dr);
                lCasa.Add(trans);
            }

            return lCasa;
        }

        IEnumerable<CursoMembro> IRelatoriosSecretariaRepository.CursosMembro(long congregacaoId, int cursoId, long usuarioID)
        {
            var cursos = new List<Domain.Entities.Relatorios.Secretaria.CursoMembro>();

            var lParans = new List<SqlParameter>
                {
                    new("@CongregacaoId", congregacaoId),
                    new("@CursoId", cursoId),
                    new("@UsuarioID", usuarioID)
                };

            using SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelCursosMembro, lParans.ToArray());
            while (dr.Read())
            {
                var trans = DataMapper.ExecuteMapping<Domain.Entities.Relatorios.Secretaria.CursoMembro>(dr);
                cursos.Add(trans);
            }

            return cursos;
        }

        public long Add(Aniversariantes entity)
        {
            throw new NotImplementedException();
        }

        public long Update(Aniversariantes entity)
        {
            throw new NotImplementedException();
        }

        public int Delete(Aniversariantes entity)
        {
            throw new NotImplementedException();
        }

        public int Delete(long id)
        {
            throw new NotImplementedException();
        }

        public Aniversariantes GetById(long id, long usuarioID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Aniversariantes> GetAll(long usuarioID)
        {
            throw new NotImplementedException();
        }

        public long Add(Aniversariantes entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public long Update(Aniversariantes entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public int Delete(Aniversariantes entity, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public int Delete(long id, long usuarioID = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Batismo> CandidatosBatismo(long batismoId, DateTimeOffset dataBatismo, long congregacaoId, SituacaoCandidatoBatismo situacao, long usuarioID)
        {
            object dtbatismo = dataBatismo != DateTimeOffset.MinValue ? dataBatismo.Date : (object)DBNull.Value;

            var lParans = new List<SqlParameter>
            {
                new("@BatismoId", batismoId),
                new("@DataBatismo", dtbatismo),
                new("@UsuarioID", usuarioID)
            };

            var batismo = new List<Domain.Entities.Relatorios.Secretaria.Batismo>();
            using (SqlDataReader dr = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelBatismo, lParans.ToArray()))
            {
                while (dr.Read())
                {
                    var bat = new Domain.Entities.Relatorios.Secretaria.Batismo()
                    {
                        Id = Convert.ToInt32(dr["Id"].ToString()),
                        DataBatismo = dr["DataBatismo"].TryConvertTo<DateTimeOffset>()
                    };
                    Enum.TryParse(dr["Status"].ToString(), out StatusBatismo status);
                    bat.Status = status;

                    var sit = situacao == SituacaoCandidatoBatismo.Nulo ? (object)DBNull.Value : situacao;
                    var lParamCand = new List<SqlParameter>
                    {
                        new("@BatismoId", bat.Id),
                        new("@Situacao", sit),
                        new("@Congregacao", congregacaoId),
                        new("@UsuarioID", usuarioID)
                    };
                    using (SqlDataReader dr2 = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelBatismoCandidatos, lParamCand.ToArray()))
                    {
                        while (dr2.Read())
                        {
                            var cand = new CandidatoBatismo()
                            {
                                Id = Convert.ToInt32(dr2["Id"].ToString()),
                                Nome = dr2["Nome"].ToString(),
                                DataNascimento = string.IsNullOrEmpty(dr2["DataNascimento"].ToString()) ? DateTime.MinValue : Convert.ToDateTime(dr2["DataNascimento"].ToString()),
                                CongregacaoId = Convert.ToInt32(dr2["CongregacaoId"].ToString()),
                                CongregacaoNome = dr2["CongregacaoNome"].ToString(),
                                Situacao = dr2["Situacao"].ToString()
                            };
                            if (Enum.TryParse(typeof(Tamanho), dr2["TamanhoCapa"].TryConvertTo<string>(), out object tam))
                                cand.TamanhoCapa = (Tamanho)tam;

                            cand.Observacoes = _membroRepository.ListarObservacaoMembro(cand.Id).ToList();
                            bat.Candidatos.Add(cand);
                        }
                    }

                    var lParamPastores = new List<SqlParameter>
                    {
                        new("@BatismoId", bat.Id)
                    };
                    using (SqlDataReader dr2 = MicrosoftSqlHelper.ExecuteReader(ConnectionString, CommandType.StoredProcedure, RelBatismoCelebrantes, lParamPastores.ToArray()))
                    {
                        while (dr2.Read())
                        {
                            bat.PastorCelebrante.Add(new Pastor()
                            {
                                Id = dr2["Id"].TryConvertTo<int>(),
                                Nome = dr2["Nome"].ToString(),
                            });
                        }
                    }

                    batismo.Add(bat);
                }
            }
            return batismo;

        }

        public EventosFeriados RelatorioEventos(long congregacaoId, int mes, int ano, Domain.Entities.Evento.TipoEvento tipoEvento)
        {
            var evFer = new EventosFeriados();

            var lstParameters = new List<SqlParameter>
                {
                    new("@CongregacaoId", congregacaoId),
                    new("@Mes", mes),
                    new("@Ano", ano),
                    new("@TipoEvento", tipoEvento),
                };

            using var ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, RelEventos, lstParameters.ToArray());
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    var evento = new Evento()
                    {
                        Descricao = ds.Tables[0].Rows[i]["Descricao"].ToString(),
                        Congregacao = ds.Tables[0].Rows[i]["CongregacaoNome"].ToString(),

                    };
                    DateTime.TryParse(ds.Tables[0].Rows[i]["DataHoraFim"].ToString(), out DateTime dataFim);
                    evento.DataHoraFim = dataFim;
                    DateTime.TryParse(ds.Tables[0].Rows[i]["DataHoraInicio"].ToString(), out DateTime dataIni);
                    evento.DataHoraInicio = dataIni;
                    int.TryParse(ds.Tables[0].Rows[i]["Id"].ToString(), out int id);
                    evento.Id = id;
                    evFer.Eventos.Add(evento);
                }
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    var feriado = new Feriado()
                    {
                        Data = Convert.ToDateTime(ds.Tables[1].Rows[i]["DataFeriado"]),
                        Descricao = ds.Tables[1].Rows[i]["Descricao"].ToString(),
                    };
                    evFer.Feriados.Add(feriado);
                }
            }

            return evFer;
        }

        public List<PresencaLista> RelatorioPresencaLista(int idPresenca, int idData, long usuarioId = 0)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@PresencaId", idPresenca),
                    new("@DataId", idData),
                    new("@UsuarioID", usuarioId)
                };
            try
            {
                var lpresenca = new List<PresencaLista>();
                using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, RelPresencaLista, lstParameters.ToArray()))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var presenca = new PresencaLista()
                            {
                                Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                                Descricao = ds.Tables[0].Rows[i]["Descricao"].ToString(),
                                MembroId = Convert.ToInt16(ds.Tables[0].Rows[i]["MembroId"]),
                                Nome = ds.Tables[0].Rows[i]["Nome"].ToString(),
                                CPF = ds.Tables[0].Rows[i]["CPF"].ToString(),
                                CongregacaoId = ds.Tables[0].Rows[i]["CongregacaoId"].TryConvertTo<int>(),
                                Igreja = ds.Tables[0].Rows[i]["Igreja"].ToString(),
                                Cargo = ds.Tables[0].Rows[i]["Cargo"].ToString(),
                                Justificativa = ds.Tables[0].Rows[i]["Justificativa"].ToString(),
                            };

                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<string>()))
                                presenca.DataHoraInicio = ds.Tables[0].Rows[i]["DataHoraInicio"].TryConvertTo<DateTime>();
                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["DataHoraFim"].TryConvertTo<string>()))
                                presenca.DataHoraFim = ds.Tables[0].Rows[i]["DataHoraFim"].TryConvertTo<DateTime>();
                            if (Enum.TryParse(typeof(SituacaoPresenca), ds.Tables[0].Rows[i]["Situacao"].TryConvertTo<string>(), out object situacao))
                                presenca.Situacao = (SituacaoPresenca)situacao;

                            lpresenca.Add(presenca);
                        }
                    }
                }
                return lpresenca;
            }
            catch
            {
                throw;
            }
        }

        public List<PresencaLista> RelatorioInscricoes(int idPresenca, int idCongregacao, int tipo, long usuarioID = 0)
        {
            var lstParameters = new List<SqlParameter>
                {
                    new("@PresencaId", idPresenca),
                    new("@CongregacaoId", idCongregacao),
                    new("@Tipo", tipo),
                    new("@UsuarioId", usuarioID)
                };
            try
            {
                var lpresenca = new List<PresencaLista>();
                using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, RelPresencaInscrito, lstParameters.ToArray()))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var presenca = new PresencaLista()
                            {
                                CongregacaoId = ds.Tables[0].Rows[i]["CongregacaoId"].TryConvertTo<int>(),
                                Igreja = ds.Tables[0].Rows[i]["Igreja"].ToString(),
                                Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                                Descricao = ds.Tables[0].Rows[i]["Descricao"].ToString(),
                                MembroId = Convert.ToInt16(ds.Tables[0].Rows[i]["MembroId"]),
                                Nome = ds.Tables[0].Rows[i]["Nome"].ToString(),
                                CPF = ds.Tables[0].Rows[i]["CPF"].ToString(),
                                Cargo = ds.Tables[0].Rows[i]["Cargo"].ToString(),
                                Pago = ds.Tables[0].Rows[i]["Pago"].TryConvertTo<int>() == 1,
                                Valor = ds.Tables[0].Rows[i]["Valor"].TryConvertTo<float>()
                            };
                            lpresenca.Add(presenca);
                        }
                    }
                }
                return lpresenca;
            }
            catch
            {
                throw;
            }
        }

        public RelatorioFrequencia RelatorioFrequencia(int congregacaoId, DateTime dataInicial, DateTime dataFinal, string cargos, string tipoEvento)
        {
            object dtInicial = dataInicial != DateTime.MinValue ? dataInicial.Date : (object)DBNull.Value;
            object dtFinal = dataInicial != DateTime.MinValue ? dataFinal.Date : (object)DBNull.Value;

            var lstParameters = new List<SqlParameter>
                {
                    new("@Congregacao", congregacaoId),
                    new("@DataInicial", dtInicial),
                    new("@DataFinal", dtFinal),
                    new("@Cargos", cargos),
                    new("@TipoEvento", tipoEvento)
                };
            try
            {
                var lpresenca = new RelatorioFrequencia();
                using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, RelPresencaFrequenciaLista, lstParameters.ToArray()))
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var membro = new MembroSimplificado()
                            {
                                Id = Convert.ToInt16(ds.Tables[0].Rows[i]["Id"]),
                                Nome = ds.Tables[0].Rows[i]["Nome"].ToString(),
                                Cargo = ds.Tables[0].Rows[i]["Cargo"].ToString(),
                                CongregacaoId = ds.Tables[0].Rows[i]["CongregacaoId"].TryConvertTo<int>(),
                                Congregacao = ds.Tables[0].Rows[i]["Congregacao"].ToString(),
                                CPF = ds.Tables[0].Rows[i]["CPF"].ToString(),
                            };
                            lpresenca.Membros.Add(membro);
                        }
                    }

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        {
                            var insc = new InscricaoCurso()
                            {
                                Id = Convert.ToInt16(ds.Tables[1].Rows[i]["Id"]),
                                PresencaId = Convert.ToInt16(ds.Tables[1].Rows[i]["PresencaId"]),
                                MembroId = Convert.ToInt16(ds.Tables[1].Rows[i]["MembroId"]),
                                CPF = ds.Tables[1].Rows[i]["CPF"].ToString()
                            };
                            lpresenca.InscricoesCurso.Add(insc);
                        }
                    }

                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[2].Rows.Count; i++)
                        {
                            var data = new PresencaData()
                            {
                                Id = Convert.ToInt16(ds.Tables[2].Rows[i]["Id"]),
                                PresencaId = Convert.ToInt16(ds.Tables[2].Rows[i]["PresencaId"]),
                                Data = ds.Tables[2].Rows[i]["DataHoraInicio"].TryConvertTo<DateTime>(),

                            };
                            lpresenca.Datas.Add(data);
                        }
                    }

                    if (ds.Tables[3].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[3].Rows.Count; i++)
                        {
                            var pres = new Presenca()
                            {
                                InscricaoId = Convert.ToInt16(ds.Tables[3].Rows[i]["InscricaoId"]),
                                DataId = Convert.ToInt16(ds.Tables[3].Rows[i]["DataId"]),
                                Situacao = Convert.ToInt16(ds.Tables[3].Rows[i]["Situacao"]),

                            };
                            lpresenca.Presencas.Add(pres);
                        }
                    }
                }
                return lpresenca;
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<Membro> MembrosGrid(bool gridBatismo, List<string> congregacoes, List<string> cargos, int idBatismo, bool imprimirObrVinc)
        {
            List<Membro> lCarteirinha = [];
            var lstParameters = new List<SqlParameter>
                {
                    new("@TipoConsulta", gridBatismo ? 0: 1),
                    new("@Congregacao", string.Join(',', congregacoes)),
                    new("@Cargos", string.Join(',', cargos)),
                    new("@BatismoId", idBatismo),
                    new("@ImprimirObrVinc", imprimirObrVinc ? 1: 0),
                };


            using (DataSet ds = MicrosoftSqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, RelListaCarteirinhasGrid, lstParameters.ToArray()))
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var data = ds.Tables[0].Rows[i]["DataValidadeCarteirinha"].ToString();
                        var carteirinha = new Membro
                        {
                            Id = Convert.ToInt64(ds.Tables[0].Rows[i]["Id"].ToString()),
                            Nome = ds.Tables[0].Rows[i]["Nome"].TryConvertTo<string>(),
                            Cpf = ds.Tables[0].Rows[i]["Cpf"].TryConvertTo<string>(),
                            Congregacao = ds.Tables[0].Rows[i]["Congregacao"].TryConvertTo<string>(),
                            DataValidadeCarteirinha = !string.IsNullOrEmpty(data) ? DateTimeOffset.Parse(data).Date.ToShortDateString() : string.Empty,
                            Cargo = ds.Tables[0].Rows[i]["Cargo"].TryConvertTo<string>()
                        };
                        lCarteirinha.Add(carteirinha);
                    }
                }
            }
            return lCarteirinha;
        }
    }
    #endregion
}