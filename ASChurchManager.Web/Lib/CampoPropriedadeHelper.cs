using ASChurchManager.Web.ViewModels.Search;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ASChurchManager.Web.Lib
{
    public static class CampoPropriedadeHelper
    {

        public static IHtmlContent CampoPropriedadeHTMLString(this IHtmlHelper htmlHelper, IHtmlContent campo, bool ultimocampo)
            => new HtmlString($"{GetString(campo)}{(ultimocampo ? string.Empty : ", ")}");

        public static string GetString(IHtmlContent content)
        {
            using var writer = new System.IO.StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }
    }
}
