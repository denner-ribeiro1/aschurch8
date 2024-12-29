using System.Net;

namespace ASChurchManager.API.Membro.Infra;

public static class HttpExtensions
{
    public static bool IsSucess(this HttpStatusCode statusCode) =>
        new HttpResponseMessage(statusCode).IsSuccessStatusCode;
}
