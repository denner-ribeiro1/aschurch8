namespace ASChurchManager.Web.Filters.Menu
{
    public class ActionAttribute : System.Attribute
    {
        public Menu Menu { get; set; }
        public Menu SubMenu { get; set; }

        public ActionAttribute(Menu menu, Menu subMenu = Menu.NaoDefinido)
        {
            this.Menu = menu;
            this.SubMenu = subMenu;
        }
    }
}