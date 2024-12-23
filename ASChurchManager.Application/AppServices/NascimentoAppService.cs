using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using System.Collections.Generic;

namespace ASChurchManager.Application.AppServices
{
    public class NascimentoAppService : BaseAppService<Nascimento>, INascimentoAppService
    {
        #region Variaveis
        private readonly INascimentoRepository _nascimentoService;
        #endregion

        #region Construtor
        public NascimentoAppService(INascimentoRepository nascimentoService)
            : base(nascimentoService)
        {
            _nascimentoService = nascimentoService;
        }

        #endregion

        public IEnumerable<Nascimento> ListarNascimentoPaginado(int pageSize, int rowStart, out int rowCount, string sorting, string campo, string valor, long usuarioID)
        {
            return _nascimentoService.ListarNascimentoPaginado(pageSize, rowStart, out rowCount, sorting, campo, valor, usuarioID);
        }
    }
}
