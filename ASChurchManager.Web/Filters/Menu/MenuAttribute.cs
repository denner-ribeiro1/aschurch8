namespace ASChurchManager.Web.Filters.Menu
{
    public class MenuAttribute : System.ComponentModel.DescriptionAttribute
    {
        public string Icon { get; set; }

        public MenuAttribute(string descricao, string icone = "") :
            base(descricao)
        {
            Icon = icone;
        }
    }
}