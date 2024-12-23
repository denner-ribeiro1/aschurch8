
namespace ASChurchManager.Application.Interfaces
{
    public interface IPrinterAppService
    {
        string GetHtmlToPrint(long templateId, string modelType, long modelId, long usuarioId);
    }
}
