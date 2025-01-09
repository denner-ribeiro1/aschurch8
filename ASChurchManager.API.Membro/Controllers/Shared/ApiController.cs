using System.Net;
using ASChurchManager.API.Membro.Infra;
using Microsoft.AspNetCore.Mvc;

namespace ASChurchManager.API.Membro.Controllers.Shared;

[ApiController]
[ServiceFilter(typeof(SiteExceptionFilter))]
public abstract class ApiController : ControllerBase
{
   protected IActionResult ResponseOK() =>
       Response(HttpStatusCode.OK);

   protected IActionResult ResponseOK(object result) =>
      Response(HttpStatusCode.OK, result);

   protected IActionResult ResponseCreated() =>
           Response(HttpStatusCode.Created);

   protected IActionResult ResponseCreated(object result) =>
      Response(HttpStatusCode.Created, result);

   protected IActionResult ResponseNoContent() =>
           Response(HttpStatusCode.NoContent);

   protected IActionResult ResponseNoContent(object result) =>
      Response(HttpStatusCode.NoContent, result);

   protected IActionResult ResponseBadRequest() =>
           Response(HttpStatusCode.BadRequest, "Requisição inválida.");

   protected IActionResult ResponseBadRequest(object result) =>
      Response(HttpStatusCode.BadRequest, result);

   protected IActionResult ResponseBadRequest(string errorMessage) =>
      Response(HttpStatusCode.BadRequest, errorMessage);

   protected IActionResult ResponseNotFound() =>
           Response(HttpStatusCode.NotFound, "Recurso não encontrado.");

   protected IActionResult ResponseNotFound(object result) =>
      Response(HttpStatusCode.NotFound, result);

   protected IActionResult ResponseNotFound(string errorMessage) =>
      Response(HttpStatusCode.NotFound, errorMessage);

   protected IActionResult ResponseUnauthorized() =>
           Response(HttpStatusCode.Unauthorized, "Usuário não autorizado.");

   protected IActionResult ResponseUnauthorized(object result) =>
      Response(HttpStatusCode.Unauthorized, result);

   protected IActionResult ResponseUnauthorized(string errorMessage) =>
      Response(HttpStatusCode.Unauthorized, errorMessage);

   protected IActionResult ResponseForbidden() =>
           Response(HttpStatusCode.Forbidden);

   protected IActionResult ResponseForbidden(object result) =>
      Response(HttpStatusCode.Forbidden, result);

   protected IActionResult ResponseForbidden(string errorMessage) =>
      Response(HttpStatusCode.Forbidden, errorMessage);

   protected IActionResult ResponseServerError() =>
           Response(HttpStatusCode.InternalServerError, "Erro interno no servidor.");

   protected IActionResult ResponseServerError(object result) =>
      Response(HttpStatusCode.InternalServerError, result);

   protected IActionResult ResponseServerError(string errorMessage) =>
      Response(HttpStatusCode.InternalServerError, errorMessage);

   protected new JsonResult Response(HttpStatusCode status, object? data, string? errorMessage)
   {
      CustomResult? result = null;

      if (string.IsNullOrWhiteSpace(errorMessage))
      {
         var sucess = status.IsSucess();

         if (data != null)
            result = new CustomResult(status, sucess, data);
         else
            result = new CustomResult(status, sucess);
      }
      else
      {
         var errors = new List<string>();
         if (!string.IsNullOrWhiteSpace(errorMessage))
            errors.Add(errorMessage);

         result = new CustomResult(status, false, errors);
      }
      return new JsonResult(result) { StatusCode = (int)result.StatusCode };
   }

   protected new JsonResult Response(HttpStatusCode status, object data) => Response(status, data, null);

   protected new JsonResult Response(HttpStatusCode status, string errorMessage) => Response(status, null, errorMessage);

   protected new JsonResult Response(HttpStatusCode status) => Response(status, null, null);



}
