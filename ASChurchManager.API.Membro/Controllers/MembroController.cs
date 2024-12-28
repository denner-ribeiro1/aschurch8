using ASChurchManager.API.Membro.Models;
using ASChurchManager.API.Membro.Services;
using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Lib;
using ASChurchManager.Domain.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASChurchManager.API.Membro.Controllers
{
    [Route("api/membro")]
    [ApiController]
    public class MembroController : ControllerBase
    {
        private IMembroAppService _membroAppService;
        public MembroController(IMembroAppService membroAppService)
        {
            _membroAppService = membroAppService;

        }

        [HttpGet("consultarMembro")]
        [Authorize]
        public ObjectResult ConsultarMembro([FromQuery] int id, [FromServices] ICargoAppService cargoAppService)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Id é de preechimento obrigatório");

                var membro = _membroAppService.GetById(id, 0);
                if (membro.Id != id)
                {
                    return StatusCode(400, new Erro("Membro não encontrado"));
                }
                else
                {
                    var mem = new MembroDTO
                    {
                        rm = (int)membro.Id,
                        nome = membro.Nome,
                        email = membro.Email,
                        congregacao = membro.Congregacao.Nome,
                        atualizarSenha = membro.AtualizarSenha
                    };

                    var cargoMem = membro.Cargos.FirstOrDefault(c => c.DataCargo == membro.Cargos.Max(c => c.DataCargo));
                    if (cargoMem != null)
                    {
                        mem.cargo = cargoMem.TipoCarteirinha.ToString();
                    }
                    return Ok(mem);
                }
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new Erro(ex.Message, ex));
            }
        }


        [HttpPatch("atualizarSenha")]
        [Authorize]
        public ObjectResult AtualizarSenha(SenhaDTO senhaDTO)
        {
            try
            {
                if (!_membroAppService.ValidarSenha(senhaDTO.id, senhaDTO.senhaAtual))
                    return BadRequest("Senha atual está incorreta.");

                _membroAppService.AtualizarSenha(senhaDTO.id, senhaDTO.senhaAtual, senhaDTO.novaSenha, false);
                return Ok("Senha atualizada com sucesso.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new Erro(ex.Message, ex));
            }
        }

        [HttpPost("inscricao")]
        [Authorize]
        public ObjectResult Inscricao(InscricaoDTO inscricaoDTO)
        {
            try
            {
                var (validaOK, msgErro) = _membroAppService.InscricaoApp(inscricaoDTO.cpf, inscricaoDTO.nomeMae, inscricaoDTO.dataNascimento, inscricaoDTO.email);
                if (!validaOK)
                {
                    return BadRequest(msgErro);
                }
                return Ok("Inscrição realizada com sucesso!");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new Erro(ex.Message, ex));
            }
        }

        [HttpPost("recuperarSenha")]
        [Authorize]
        public ObjectResult RecuperarSenha(RecuperarSenhaDTO recuperarSenhaDTO)
        {
            try
            {
                var (validaOK, msgErro) = _membroAppService.RecuperarSenha(recuperarSenhaDTO.cpf);
                if (!validaOK)
                {
                    return BadRequest(msgErro);
                }
                return Ok("Senha enviada com sucesso!");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new Erro(ex.Message, ex));
            }
        }
    }
}
