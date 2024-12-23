using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASChurchManager.Application.AppServices
{
    public class BatismoAppService : BaseAppService<Batismo>, IBatismoAppService
    {
        #region Variaveis
        private readonly IBatismoRepository _batismoService;
        #endregion

        #region Construtor
        public BatismoAppService(IBatismoRepository batismoService)
            : base(batismoService)
        {
            _batismoService = batismoService;
        }
        #endregion

        private bool ValidarBatismo(Batismo entity, long usuarioID = 0)
        {
            if (entity.Status == StatusBatismo.EmAberto)
            {
                var batismos = GetAll(usuarioID);
                var batismosFinalizados = batismos.Where(b => b.Id != entity.Id && b.Status != StatusBatismo.EmAberto);
                if (batismosFinalizados.Any(b => b.DataBatismo >= entity.DataBatismo))
                    throw new Exception($"Data de batismo deve ser maior que a data do último batismo finalizado.");
                var batismoAberto = batismos.Where(b => b.Id != entity.Id && b.Status == StatusBatismo.EmAberto);
                if (batismoAberto.Any(b => b.DataBatismo.Date == entity.DataBatismo.Date && b.DataBatismo.TimeOfDay == entity.DataBatismo.TimeOfDay))
                    throw new Exception($"Existe um batismo com Data e Hora em aberto já cadastrado.");
            }
            return true;
        }
        #region Publicos
        public override long Add(Batismo entity, long usuarioID = 0)
        {
            try
            {
                ValidarBatismo(entity, usuarioID);
                entity.Id = base.Add(entity);
                return entity.Id;
            }
            catch (Exception ex)
            {
                entity.PreencherStatusErro(ex);
                return 0;
            }
        }

        public override long Update(Batismo entity, long usuarioID = 0)
        {
            try
            {
                ValidarBatismo(entity, usuarioID);
                return base.Update(entity);
            }
            catch (Exception ex)
            {
                entity.PreencherStatusErro(ex);
                return 0;
            }
        }

        public void AtualizarMembroBatismo(IEnumerable<BatismoMembro> candidatoBatismo, long batismoId)
        {
            _batismoService.AtualizarMembroBatismo(candidatoBatismo, batismoId);
        }

        public IEnumerable<Membro> SelecionaMembrosParaBatismo(long batismoId, StatusBatismo status, long usuarioID)
        {
            return _batismoService.SelecionaMembrosParaBatismo(batismoId, status, usuarioID);
        }

        public Batismo SelecionaUltimoDataBatismo()
        {
            return _batismoService.SelecionaUltimoDataBatismo();
        }

        public IEnumerable<Membro> ListarPastorCelebrante(long batismoId)
        {
            return _batismoService.ListarPastorCelebrante(batismoId);
        }

        public IEnumerable<Membro> ListarCandidatosBatismoPaginada(int pageSize, int rowStart, string sorting, string campo, string valor, long usuarioID, out int rowCount)
        {
            return _batismoService.ListarCandidatosBatismoPaginada(pageSize, rowStart, sorting, campo, valor, usuarioID, out rowCount);
        }

        public IEnumerable<Batismo> ListarBatismoPaginada(int pageSize, int rowStart, string sorting, out int rowCount)
        {
            return _batismoService.ListarBatismoPaginada(pageSize, rowStart, sorting, out rowCount);
        }

        public void AtualizarStatusBatismo(long batismoId, SituacaoCandidatoBatismo situacao)
        {
            throw new System.NotImplementedException();
        }
        #endregion

    }
}
