using System.Security.Cryptography.X509Certificates;
using ASChurchManager.API.Membro.Models;
using ASChurchManager.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASChurchManager.API.Membro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private IMembroAppService _membroAppService;
        public LoginController(IMembroAppService membroAppService)
        {
            _membroAppService = membroAppService;
        }

        [HttpPost()]
        public ActionResult<MembroDTO> Autentica(LoginDTO login)
        {
            var membro = _membroAppService.GetByCPF(login.Cpf, true);
            if (membro == null)
                return BadRequest("");
            return Ok();
        }
    }
}
