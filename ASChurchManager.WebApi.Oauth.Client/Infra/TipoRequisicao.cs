using System.ComponentModel;

namespace ASChurchManager.WebApi.Oauth.Client
{
    public enum TipoRequisicaoWebApi
    {
        [Description("Get")]
        [DefaultValue("G")]
        Get,
        [Description("Post")]
        [DefaultValue("P")]
        Post
    }
}