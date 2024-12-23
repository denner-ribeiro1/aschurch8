
namespace ASChurchManager.Domain.Entities
{
    public class Rotina : BaseEntity
    {
        public string Area { get; set; }
        public string AreaDescricao { get; set; }
        public string AreaIcone { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string MenuDescricao { get; set; }
        public string MenuIcone { get; set; }
        public string SubMenuDescricao { get; set; }
        public string SubMenuIcone { get; set; }
        
        
    }
}
