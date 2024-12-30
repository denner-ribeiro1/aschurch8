using System.Text.Json;
using ASChurchManager.API.Membro.Controllers.Shared;
using ASChurchManager.API.Membro.Models;
using ASChurchManager.API.Membro.Services;
using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Lib;
using Microsoft.AspNetCore.Mvc;

namespace ASChurchManager.API.Membro.Controllers
{
    [Route("api/oauth")]
    public class LoginController : ApiController
    {
        private IMembroAppService _membroAppService;
        public LoginController(IMembroAppService membroAppService)
        {
            _membroAppService = membroAppService;
        }

        [HttpPost("token")]
        public IActionResult Token([FromServices] IConfiguration configuration,
          LoginDTO login)
        {
            try
            {
                if (login.cpf == configuration["ParametrosSistema:AcessoAPIUsuario"])
                {
                    if (login.senha == configuration["ParametrosSistema:AcessoAPISenha"])
                    {
                        var token = new TokenServices().Generate(new Domain.Entities.Membro()
                        {
                            Id = 1,
                            Nome = "API",
                            Congregacao = new Domain.Entities.Congregacao()
                            {
                                Nome = "API"
                            }
                        });

                        return ResponseOK(JsonSerializer.Serialize(new { Token = token }));
                    }
                    return ResponseUnauthorized(new Erro("Membro não localizado"));
                }
                else
                {
                    var (senhaOk, membro) = _membroAppService.ValidarLogin(login.cpf, login.senha);
                    if (senhaOk)
                    {
                        var token = new TokenServices().Generate(membro);

                        return ResponseOK(JsonSerializer.Serialize(new { Result = "OK", Token = token }));
                    }
                    return ResponseUnauthorized(new Erro("Membro não localizado"));
                }

            }
            catch (Exception ex)
            {
                return ResponseServerError(new Erro(ex.Message, ex));
            }
        }
    }
}
