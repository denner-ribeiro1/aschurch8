using ASChurchManager.API.Membro.Controllers.Shared;
using ASChurchManager.API.Membro.Models;
using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASChurchManager.API.Membro.Controllers
{
    [Route("api/membro")]
    public class MembroController : ApiController
    {
        private IMembroAppService _membroAppService;
        public MembroController(IMembroAppService membroAppService)
        {
            _membroAppService = membroAppService;

        }

        [HttpGet("consultarMembro")]
        [Authorize]
        public IActionResult ConsultarMembro([FromQuery] int id, [FromServices] ICargoAppService cargoAppService)
        {
            try
            {
                if (id <= 0)
                    return ResponseBadRequest("Id é de preechimento obrigatório");

                var membro = _membroAppService.GetById(id, 0);
                if (membro.Id != id)
                {
                    return ResponseBadRequest("Membro não encontrado");
                }
                else
                {
                    var mem = new MembroDTO
                    {
                        rm = (int)membro.Id,
                        nome = membro.Nome,
                        email = membro.Email,
                        congregacao = membro.Congregacao.Nome,
                        atualizarSenha = membro.AtualizarSenha,
                        foto = membro.FotoUrl
                    };

                    var cargoMem = membro.Cargos.FirstOrDefault(c => c.DataCargo == membro.Cargos.Max(c => c.DataCargo));
                    if (cargoMem != null)
                    {
                        mem.cargo = cargoMem.TipoCarteirinha.ToString();
                    }
                    return ResponseOK(mem);
                }
            }
            catch (Exception ex)
            {
                return ResponseBadRequest(new Erro(ex.Message, ex));
            }
        }


        [HttpPatch("atualizarSenha")]
        [Authorize]
        public IActionResult AtualizarSenha(SenhaDTO senhaDTO)
        {
            try
            {
                if (!_membroAppService.ValidarSenha(senhaDTO.id, senhaDTO.senhaAtual))
                    return ResponseBadRequest("Senha atual está incorreta.");

                _membroAppService.AtualizarSenha(senhaDTO.id, senhaDTO.senhaAtual, senhaDTO.novaSenha, false);
                return ResponseOK("Senha atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                return ResponseServerError(new Erro(ex.Message, ex));
            }
        }

        [HttpPost("inscricao")]
        [Authorize]
        public IActionResult Inscricao(InscricaoDTO inscricaoDTO)
        {
            try
            {
                var (validaOK, msg) = _membroAppService.InscricaoApp(inscricaoDTO.cpf, inscricaoDTO.nomeMae, inscricaoDTO.dataNascimento, inscricaoDTO.email);
                if (!validaOK)
                {
                    return ResponseBadRequest(msg);
                }
                return ResponseOK(msg);
            }
            catch (Exception ex)
            {
                return ResponseServerError(new Erro(ex.Message, ex));
            }
        }

        [HttpPost("recuperarSenha")]
        [Authorize]
        public IActionResult RecuperarSenha(RecuperarSenhaDTO recuperarSenhaDTO)
        {
            try
            {
                var (validaOK, msg) = _membroAppService.RecuperarSenha(recuperarSenhaDTO.cpf);
                if (!validaOK)
                {
                    return ResponseBadRequest(msg);
                }
                return ResponseOK(msg);
            }
            catch (System.Exception ex)
            {
                return ResponseServerError(new Erro(ex.Message, ex));
            }
        }
    }
}
