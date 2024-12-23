using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASChurchManager.Application.Interfaces
{
    public interface IPrinterAPIAppService
    {
        bool Printer(long templateId, string targetModel, long modelId, int usuarioId, out byte[] relatorio, out string mimeType);
    }
}
