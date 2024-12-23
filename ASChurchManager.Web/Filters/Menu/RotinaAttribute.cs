namespace ASChurchManager.Web.Filters.Menu
{
    public class RotinaAttribute : System.Attribute
    {
        public Area Area { get; set; }
        
        public RotinaAttribute(Area area)
        {
            this.Area = area;
        }
    }
}