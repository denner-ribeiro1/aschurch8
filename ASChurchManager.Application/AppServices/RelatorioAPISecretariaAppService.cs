using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities.Relatorios.API.In;
using ASChurchManager.Domain.Entities.Relatorios.API.Out;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Types;
using ASChurchManager.WebApi.Oauth.Client;
using Newtonsoft.Json;
using System;

namespace ASChurchManager.Application.AppServices
{
    public class RelatorioAPISecretariaAppService : IRelatorioAPISecretariaAppService
    {
        private readonly IRelatoriosSecretariaRepository _relatorioService;
        private readonly IUsuarioRepository _usuarioService;
        private readonly IClientAPIAppServices _clientService;

        public RelatorioAPISecretariaAppService(IRelatoriosSecretariaRepository relatorioService,
            IUsuarioRepository usuarioService,
            IClientAPIAppServices clientService)
        {
            _usuarioService = usuarioService;
            _relatorioService = relatorioService;
            _clientService = clientService;
        }

        public bool RelatorioAniversariantes(int congregacaoId, DateTime dataInicio, DateTime dataFinal, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType, TipoMembro tipoMembro)
        {
            var param = new InAniversario()
            {
                CongregacaoId = congregacaoId,
                DataInicio = dataInicio,
                DataFinal = dataFinal,
                TipoSaida = tipoSaida,
                TipoMembro = tipoMembro
            };

            var retorno =
                JsonConvert.DeserializeObject<OutRelatorio>(
                    _clientService.RequisicaoWebApi("RelatorioSecretaria/Aniversariantes", TipoRequisicaoWebApi.Post, JsonConvert.SerializeObject(param), usuarioId));

            if (retorno.Erros.Count > 0)
            {
                string erro = "";
                retorno.Erros.ForEach(e => erro += e + Environment.NewLine);
                throw new Exception($"Erro ao gerar Relatório de Aniversariantes. {erro}");
            }
            else
            {
                relatorio = retorno.Relatorio;
                mimeType = retorno.MimeType;
                return true;
            }
        }

        public bool RelatorioCandidatosBatismo(int congregacaoId, long batismoId, DateTime dataBatismo, SituacaoCandidatoBatismo situacao, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType)
        {
            var param = new InCandidatosBatismo()
            {
                CongregacaoId = congregacaoId,
                BatismoId = batismoId,
                DataBatismo = dataBatismo,
                TipoSaida = tipoSaida,
                Situacao = situacao
            };

            var retorno =
                JsonConvert.DeserializeObject<OutRelatorio>(
                    _clientService.RequisicaoWebApi("RelatorioSecretaria/CandidatosBatismo", TipoRequisicaoWebApi.Post, JsonConvert.SerializeObject(param), usuarioId));

            if (retorno.Erros.Count > 0)
            {
                string erro = "";
                retorno.Erros.ForEach(e => erro += e + Environment.NewLine);
                throw new Exception($"Erro ao gerar Relatório de Candidatos ao Batismo. {erro}");
            }
            else
            {
                relatorio = retorno.Relatorio;
                mimeType = retorno.MimeType;
                return true;
            }
        }

        public bool RelatorioTransferencia(int congregacaoId, DateTime dataInicio, DateTime dataFinal, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType)
        {
            var param = new InTransferencia()
            {
                CongregacaoId = congregacaoId,
                DataInicio = dataInicio,
                DataFinal = dataFinal,
                TipoSaida = tipoSaida
            };

            var retorno =
                JsonConvert.DeserializeObject<OutRelatorio>(
                    _clientService.RequisicaoWebApi("RelatorioSecretaria/Transferencia", TipoRequisicaoWebApi.Post, JsonConvert.SerializeObject(param), usuarioId));

            if (retorno.Erros.Count > 0)
            {
                string erro = "";
                retorno.Erros.ForEach(e => erro += e + Environment.NewLine);
                throw new Exception($"Erro ao gerar Relatório de Transferências. {erro}");
            }
            else
            {
                relatorio = retorno.Relatorio;
                mimeType = retorno.MimeType;
                return true;
            }
        }

        public bool RelatorioNascimentos(int congregacaoId, DateTime dataInicio, DateTime dataFinal, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType)
        {
            var param = new InNascimentos()
            {
                CongregacaoId = congregacaoId,
                DataInicio = dataInicio,
                DataFinal = dataFinal,
                TipoSaida = tipoSaida
            };

            var retorno =
                JsonConvert.DeserializeObject<OutRelatorio>(
                    _clientService.RequisicaoWebApi("RelatorioSecretaria/Nascimentos", TipoRequisicaoWebApi.Post, JsonConvert.SerializeObject(param), usuarioId));

            if (retorno.Erros.Count > 0)
            {
                string erro = "";
                retorno.Erros.ForEach(e => erro += e + Environment.NewLine);
                throw new Exception($"Erro ao gerar Relatório de Nascimentos. {erro}");
            }
            else
            {
                relatorio = retorno.Relatorio;
                mimeType = retorno.MimeType;
                return true;
            }
        }

        public bool RelatorioCongregacoes(int congregacaoId, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType)
        {
            var param = new InCongregacoes()
            {
                CongregacaoId = congregacaoId,
                TipoSaida = tipoSaida
            };

            var retorno =
                JsonConvert.DeserializeObject<OutRelatorio>(
                    _clientService.RequisicaoWebApi("RelatorioSecretaria/Congregacoes", TipoRequisicaoWebApi.Post, JsonConvert.SerializeObject(param), usuarioId));

            if (retorno.Erros.Count > 0)
            {
                string erro = "";
                retorno.Erros.ForEach(e => erro += e + Environment.NewLine);
                throw new Exception($"Erro ao gerar Relatório de Congregações. {erro}");
            }
            else
            {
                relatorio = retorno.Relatorio;
                mimeType = retorno.MimeType;
                return true;
            }
        }

        public bool RelatorioObreiros(int congregacaoId, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType)
        {
            var param = new InObreiros()
            {
                CongregacaoId = congregacaoId,
                TipoSaida = tipoSaida
            };

            var retorno =
                JsonConvert.DeserializeObject<OutRelatorio>(
                    _clientService.RequisicaoWebApi("RelatorioSecretaria/Obreiros", TipoRequisicaoWebApi.Post, JsonConvert.SerializeObject(param), usuarioId));

            if (retorno.Erros.Count > 0)
            {
                string erro = "";
                retorno.Erros.ForEach(e => erro += e + Environment.NewLine);
                throw new Exception($"Erro ao gerar Relatório de Obreiros. {erro}");
            }
            else
            {
                relatorio = retorno.Relatorio;
                mimeType = retorno.MimeType;
                return true;
            }
        }

        public bool RelatorioMensal(int congregacaoId, string mes, int ano, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType)
        {
            var param = new InMensal()
            {
                CongregacaoId = congregacaoId,
                Mes = mes,
                Ano = ano,
                TipoSaida = tipoSaida
            };

            var retorno =
                JsonConvert.DeserializeObject<OutRelatorio>(
                    _clientService.RequisicaoWebApi("RelatorioSecretaria/Mensal", TipoRequisicaoWebApi.Post, JsonConvert.SerializeObject(param), usuarioId));

            if (retorno.Erros.Count > 0)
            {
                string erro = "";
                retorno.Erros.ForEach(e => erro += e + Environment.NewLine);
                throw new Exception($"Erro ao gerar Relatório Mensal. {erro}");
            }
            else
            {
                relatorio = retorno.Relatorio;
                mimeType = retorno.MimeType;
                return true;
            }
        }

        public bool RelatorioMembros(int congregacaoId, Status status, TipoMembro tipoMembro, EstadoCivil estadoCivil, bool completo, bool abedabe, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType)
        {
            var param = new InMembros()
            {
                CongregacaoId = congregacaoId,
                Status = status,
                TipoMembro = tipoMembro,
                EstadoCivil = estadoCivil,
                Completo = completo,
                ABEDABE = abedabe,
                TipoSaida = tipoSaida
            };

            var retorno =
                JsonConvert.DeserializeObject<OutRelatorio>(
                    _clientService.RequisicaoWebApi("RelatorioSecretaria/Membros", TipoRequisicaoWebApi.Post, JsonConvert.SerializeObject(param), usuarioId));

            if (retorno.Erros.Count > 0)
            {
                string erro = "";
                retorno.Erros.ForEach(e => erro += e + Environment.NewLine);
                throw new Exception($"Erro ao gerar Relatório de Membros. {erro}");
            }
            else
            {
                relatorio = retorno.Relatorio;
                mimeType = retorno.MimeType;
                return true;
            }
        }

        public bool RelatorioCursosMembro(int congregacaoId, int cursoId, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType)
        {
            var param = new InCursoMembro()
            {
                CongregacaoId = congregacaoId,
                CursoId = cursoId,
                TipoSaida = tipoSaida
            };

            var retorno =
                JsonConvert.DeserializeObject<OutRelatorio>(
                    _clientService.RequisicaoWebApi("RelatorioSecretaria/CursoMembro", TipoRequisicaoWebApi.Post, JsonConvert.SerializeObject(param), usuarioId));

            if (retorno.Erros.Count > 0)
            {
                string erro = "";
                retorno.Erros.ForEach(e => erro += e + Environment.NewLine);
                throw new Exception($"Erro ao gerar Relatório de Cursos Membro. {erro}");
            }
            else
            {
                relatorio = retorno.Relatorio;
                mimeType = retorno.MimeType;
                return true;
            }
        }

        public bool RelatorioCasamentos(int congregacaoId, DateTime dataInicio, DateTime dataFinal, int usuarioId, string tipoSaida, out byte[] relatorio, out string mimeType)
        {
            var param = new InCasamentos()
            {
                CongregacaoId = congregacaoId,
                DataInicio = dataInicio,
                DataFinal = dataFinal,
                TipoSaida = tipoSaida
            };

            var retorno =
                JsonConvert.DeserializeObject<OutRelatorio>(
                    _clientService.RequisicaoWebApi("RelatorioSecretaria/Casamentos", TipoRequisicaoWebApi.Post, JsonConvert.SerializeObject(param), usuarioId));

            if (retorno.Erros.Count > 0)
            {
                string erro = "";
                retorno.Erros.ForEach(e => erro += e + Environment.NewLine);
                throw new Exception($"Erro ao gerar Relatório de Casamentos. {erro}");
            }
            else
            {
                relatorio = retorno.Relatorio;
                mimeType = retorno.MimeType;
                return true;
            }
        }
    }
}
