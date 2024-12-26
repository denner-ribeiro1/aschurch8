using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using ASChurchManager.API.Membro.Models;
using ASChurchManager.API.Membro.Services;
using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Lib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASChurchManager.API.Membro.Controllers
{
    [Route("api/oauth")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private IMembroAppService _membroAppService;
        public LoginController(IMembroAppService membroAppService)
        {
            _membroAppService = membroAppService;
        }


        [HttpPost("token")]
        public ActionResult<string> Autentica(LoginDTO login)
        {
            try
            {
                var (senhaOk, membro) = _membroAppService.ValidarLogin(login.Cpf, login.Senha);
                if (senhaOk)
                {
                    var token = new TokenServices().Generate(membro);

                    return Ok(JsonSerializer.Serialize(new { Result = "OK", Token = token }));
                }
                return Unauthorized(new Erro("Membro não localizado"));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new Erro(ex.Message, ex));
            }
        }
    }








}
