using System.Net;

namespace ASChurchManager.API.Membro.Controllers.Shared;

public class CustomResult
{
    public HttpStatusCode StatusCode { get; set; }
    public bool Sucess { get; set; }
    public object Data { get; set; }
    public IEnumerable<string> Errors { get; set; }

    public CustomResult(HttpStatusCode statusCode, bool sucess, object data, IEnumerable<string> errors)
    {
        StatusCode = statusCode;
        Sucess = sucess;
        Data = data;
        Errors = errors;
    }

    public CustomResult(HttpStatusCode statusCode, bool sucess, object data)
    {
        StatusCode = statusCode;
        Sucess = sucess;
        Data = data;
    }

    public CustomResult(HttpStatusCode statusCode, bool sucess)
    {
        StatusCode = statusCode;
        Sucess = sucess;
    }

    public CustomResult(HttpStatusCode statusCode, bool sucess, IEnumerable<string> errors)
    {
        StatusCode = statusCode;
        Sucess = sucess;
        Errors = errors;
    }
}
