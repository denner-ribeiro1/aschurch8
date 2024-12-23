using ASChurchManager.Domain.Lib;
using System;
using System.Collections.Generic;

namespace ASChurchManager.Domain.Entities
{
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            ErrosRetorno = new List<ErroRetorno>();
            StatusRetorno = TipoStatusRetorno.OK;
        }

        public long Id { get; set; }
        public DateTimeOffset DataCriacao { get; set; }
        public DateTimeOffset DataAlteracao { get; set; }
        public TipoStatusRetorno StatusRetorno { get; set; }
        public List<ErroRetorno> ErrosRetorno { get; set; }

        public void PreencherStatusErro(Erro pEx)
        {
            StatusRetorno = TipoStatusRetorno.VALIDACOES;
            ErrosRetorno.Add(new ErroRetorno() { Codigo = 999, Mensagem = pEx.Message});
        }

        public void PreencherStatusErro(Exception pEx)
        {
            PreencherStatusErro(999, pEx.Message);
        }
        public void PreencherStatusErro(int pCodigoErro, string pMensagemErro)
        {
            StatusRetorno = TipoStatusRetorno.ERRO;
            ErrosRetorno.Add(new ErroRetorno() { Codigo = pCodigoErro, Mensagem = pMensagemErro });
        }
    }
}