using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;

namespace ASChurchManager.Application.AppServices
{
    public class CargoAppService : BaseAppService<Cargo>, ICargoAppService
    {
        #region Variaveis
        private readonly ICargoRepository _cargoService;
        #endregion

        #region Construtor
        public CargoAppService(ICargoRepository cargoService)
            : base(cargoService)
        {
            _cargoService = cargoService;
        }
        #endregion
    }
}
