using Microsoft.AspNetCore.Hosting;
using PdfSharpCore.Fonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASChurchManager.Web.Lib
{
    public class FileFontResolver : IFontResolver
    {
        private readonly IWebHostEnvironment _env;
        public FileFontResolver(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string DefaultFontName => "Arial";

        public byte[] GetFont(string faceName)
        {
            using (var ms = new MemoryStream())
            {
                using (var fs = File.Open(faceName, FileMode.Open))
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            var webRoot = _env.WebRootPath;
            if (familyName.Equals("Verdana", StringComparison.CurrentCultureIgnoreCase))
            {
                if (isBold && isItalic)
                {
                    return new FontResolverInfo($"{webRoot}/fonts/verdana-bolditalic.ttf");
                }
                else if (isBold)
                {
                    return new FontResolverInfo($"{webRoot}/fonts/verdana-bold.ttf");
                }
                else if (isItalic)
                {
                    return new FontResolverInfo($"{webRoot}/fonts/verdana-italic.ttf");
                }
                else
                {
                    return new FontResolverInfo($"{webRoot}/fonts/verdana.ttf");
                }
            }
            else if (familyName.Equals("Arial", StringComparison.CurrentCultureIgnoreCase))
            {
                if (isBold && isItalic)
                {
                    return new FontResolverInfo($"{webRoot}/fonts/arial-bolditalic.ttf");
                }
                else if (isBold)
                {
                    return new FontResolverInfo($"{webRoot}/fonts/arial-bold.ttf");
                }
                else if (isItalic)
                {
                    return new FontResolverInfo($"{webRoot}/fonts/arial-italic.ttf");
                }
                else
                {
                    return new FontResolverInfo($"{webRoot}/fonts/arial.ttf");
                }
            }
            return null;
        }
    }
}
