using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASChurchManager.Application.AppServices
{
    public class CartaAppService : BaseAppService<Carta>, ICartaAppService
    {
        private readonly ICartaRepository _cartaRepository;
        private readonly IMembroRepository _membroService;

        public CartaAppService(ICartaRepository cartaService,
            IMembroRepository membroService)
            : base(cartaService)
        {
            _cartaRepository = cartaService;
            _membroService = membroService;
        }

        public TipoStatusRetorno AprovarCarta(long pId, string codigoRecebimento, long usuarioId, out List<ErroRetorno> ErrosRetorno)
        {
            ErrosRetorno = new List<ErroRetorno>();
            try
            {
                var carta = GetById(pId, usuarioId);
                if (carta.TipoCarta == TipoDeCarta.Transferencia)
                {
                    if (codigoRecebimento != carta.CodigoRecebimento)
                        throw new Erro("Código de Recebimento não encontrado ou inválido!");
                    if (carta.DataValidade < DateTime.Now.Date)
                        throw new Erro("Carta Expirada! Favor solicitar o ajuste da carta na Congregação de Origem.");
                }
                _cartaRepository.AprovarCarta(pId, usuarioId);
                return TipoStatusRetorno.OK;
            }
            catch (Exception ex)
            {
                ErrosRetorno.Add(new ErroRetorno() { Codigo = 999, Mensagem = ex.Message });
                return TipoStatusRetorno.ERRO;
            }
        }

        public long ConsultarCodReceb(long pIdMembro)
        {
            return _cartaRepository.ConsultarCodReceb(pIdMembro);
        }

        public IEnumerable<Carta> VerificaCartaAguardandoRecebimento(long pIdMembro)
        {
            return _cartaRepository.VerificaCartaAguardandoRecebimento(pIdMembro);
        }

        public void ValidarCarta(Carta carta, long usuarioID = 0, bool create = false)
        {
            if (carta.MembroId == 0)
                throw new Erro("Registro do Membro deve ser maior que zero");
            if (string.IsNullOrWhiteSpace(carta.Nome))
                throw new Erro("Nome do Membro é de preenchimento obrigatório");
            if (string.IsNullOrWhiteSpace(carta.CongregacaoOrigem))
                throw new Erro("Nome da Congregação é de preenchimento obrigatório");
            if (carta.CongregacaoDestId > 0 && carta.CongregacaoDestId == carta.CongregacaoOrigemId)
                throw new Erro("Congregação Destino deve ser diferente da Congregação Origem");
            if (carta.DataValidade < DateTime.Now.Date)
                throw new Erro("A Data de Validade da Carta deve ser maior que a Data Atual");
            if (carta.TipoCarta == TipoDeCarta.Transferencia && string.IsNullOrWhiteSpace(carta.CongregacaoDest))
                throw new Erro("Congregação de Destino é de preenchimento obrigatório");
            if (carta.TemplateId == 0)
                throw new Erro("Template de Carta é obrigatório");

            ////// Validação da Carta
            if (create)
            {
                var situacoes = _membroService.ListarSituacoesMembro(carta.MembroId);
                var ultSit = situacoes.FirstOrDefault(p => p.Id == situacoes.Max(m => m.Id));

                if (ultSit != null && ultSit.Situacao != MembroSituacao.Comunhao)
                    throw new Erro("Membro não está em Comunhão.");

                var membro = _membroService.GetById(carta.MembroId, usuarioID);
                if (membro.TipoMembro != TipoMembro.Membro)
                    throw new Erro("Código de Registro não pertence a um Membro.");

                var cartas = _cartaRepository.VerificaCartaAguardandoRecebimento(carta.MembroId);
                if (cartas.Count() > 0)
                    throw new Erro("Existe uma Carta em aberto para o Membro");
            }
        }

        public override long Add(Carta entity, long usuarioID = 0)
        {
            try
            {
                entity.StatusCarta = StatusCarta.AguardandoRecebimento;
                if (entity.TipoCarta != TipoDeCarta.Transferencia)
                    entity.CongregacaoDestId = 0;
                entity.CodigoRecebimento = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();
                entity.IdCadastro = Convert.ToInt16(usuarioID);

                ValidarCarta(entity, usuarioID, true);
                return base.Add(entity);
            }
            catch (Exception ex)
            {
                entity.PreencherStatusErro(ex);
                return 0;
            }
        }

        public override long Update(Carta entity, long usuarioID = 0)
        {
            try
            {
                if (entity.TipoCarta == TipoDeCarta.Mudanca || entity.TipoCarta == TipoDeCarta.Recomendacao)
                    entity.StatusCarta = StatusCarta.Finalizado;
                if (entity.TipoCarta != TipoDeCarta.Transferencia)
                    entity.CongregacaoDestId = 0;
                entity.CodigoRecebimento = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();

                ValidarCarta(entity, usuarioID);
                return base.Add(entity);
            }
            catch (Exception ex)
            {
                entity.PreencherStatusErro(ex);
                return 0;
            }
        }

        public IEnumerable<Carta> GetAllTipoEStatus(TipoDeCarta? pTipoCarta, StatusCarta? pStatusCarta, long pUsuarioID)
        {
            return _cartaRepository.GetAllTipoEStatus(pTipoCarta, pStatusCarta, pUsuarioID);
        }

        public bool TransferirSemCarta(IEnumerable<Membro> membros, long congregacaoDestino, long pUsuarioID)
        {
            if (membros.Count() == 0)
                throw new Erro("Não há membros para transferência.");
            return _cartaRepository.TransferirSemCarta(membros, congregacaoDestino, pUsuarioID);
        }

        public IEnumerable<Carta> ListarCartaPaginado(int pageSize, int rowStart, string sorting, string campo, string valor, StatusCarta? statusCarta, long usuarioID, out int rowCount)
        {
            return _cartaRepository.ListarCartaPaginado(pageSize, rowStart, sorting, campo, valor, statusCarta, usuarioID, out rowCount);
        }

        public long AprovarCarta(long pId, long usuarioID)
        {
            throw new NotImplementedException();
        }

        public bool CancelarCarta(long pId, long usuarioID, out string erro)
        {
            try
            {
                if (pId == 0)
                    throw new Erro("Id da Carta igual a 0");

                var carta = GetById(pId, usuarioID);
                if (carta == null)
                    throw new Erro("Carta não encontrada");

                var todasCartas = ListarCartasPorMembroId(carta.MembroId);
                if (todasCartas.Any(p => p.Id > pId && p.StatusCarta != StatusCarta.Cancelado &&
                                         (p.TipoCarta == TipoDeCarta.Mudanca || p.TipoCarta == TipoDeCarta.Transferencia)))
                {
                    throw new Erro("Existe(m) carta(s) emitidas para o Membro após a Carta que está sendo cancelada.");
                }
                else
                {
                    return _cartaRepository.CancelarCarta(pId, usuarioID, out erro);
                }
            }
            catch(Erro ex)
            {
                erro = ex.Message;
                return false;
            }
        }

        public IEnumerable<Carta> ListarCartasPorMembroId(long membroId)
        {
            return _cartaRepository.ListarCartasPorMembroId(membroId);
        }
    }
}
