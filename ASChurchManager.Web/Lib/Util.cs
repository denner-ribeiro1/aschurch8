using ASChurchManager.Domain.Entities;
using System.IO;

namespace ASChurchManager.Web.Lib
{
    public static class Util
    {
        public static string RetornaTipoArquivo(ArquivoAzure item)
        {
            string tipo = item.Tipo.ToLower();
            if (tipo.IndexOf("pdf") > 0)
                return "pdf";
            else if (tipo.IndexOf("word") > 0)
                return "doc";
            else if (tipo.IndexOf("excel") > 0 || tipo.IndexOf("sheet") > 0)
                return "xls";
            else
            {
                string extensao = Path.GetExtension(item.Nome).ToLower();
                switch (extensao)
                {
                    case ".xls":
                    case ".xlsx":
                        return "xls";
                    case ".doc":
                    case ".docx":
                        return "doc";
                    case ".zip":
                        return "zip";
                    case ".pdf":
                        return "pdf";
                    default:
                        return "";
                }
            }
        }
    }
}
