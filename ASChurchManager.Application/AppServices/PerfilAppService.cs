using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Types;
using System.Collections.Generic;
using System.Linq;

namespace ASChurchManager.Application.AppServices
{
    public class PerfilAppService : BaseAppService<Perfil>, IPerfilAppService
    {
        private readonly IPerfilRepository _perfilService;
        private readonly IRotinaAppService _rotinaAppService;

        public PerfilAppService(IPerfilRepository perfilService, IRotinaAppService rotinaAppService)
            : base(perfilService)
        {
            _perfilService = perfilService;
            _rotinaAppService = rotinaAppService;
        }

        public Dictionary<Rotina, bool> GetRotinasPerfil(Perfil perfil)
        {
            var lstRotinasPerfil = new Dictionary<Rotina, bool>();
            var rotinas = _rotinaAppService.GetAll(0).ToList();

            if (perfil != null && perfil.TipoPerfil == TipoPerfil.Administrador)
            {
                rotinas.ForEach(r => lstRotinasPerfil.Add(r, true));
            }
            else
            {
                foreach (var rotina in rotinas)
                {
                    var selected = perfil != null && perfil.Id > 0 && perfil.Rotinas.FirstOrDefault(a => a.Id == rotina.Id) != null;
                    lstRotinasPerfil.Add(rotina, selected);
                }
            }

            return lstRotinasPerfil;

        }

        public override long Add(Perfil entity, long usuarioID = 0)
        {
            var perfilId = _perfilService.Add(entity);

            if (perfilId > 0)
            {
                if (entity.TipoPerfil != TipoPerfil.Administrador)
                {
                    entity.Rotinas.ForEach(rotina => this.AddRotinaPerfil(perfilId, rotina.Id));
                }
            }

            return perfilId;
        }

        public long AddRotinaPerfil(long perfilId, long rotinaId)
        {
            return this._perfilService.AddRotinaPerfil(perfilId, rotinaId);
        }
    }
}