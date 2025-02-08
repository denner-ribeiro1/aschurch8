using ASChurchManager.API.Membro.Controllers.Shared;
using ASChurchManager.API.Membro.Models;
using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

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
                    return ResponseOK(new { erro = true, mensagem = "Id é de preechimento obrigatório" });

                var membro = _membroAppService.GetById(id, 0);
                if (membro.Id != id)
                {
                    return ResponseOK(new { erro = true, mensagem = "Membro não encontrado" });
                }
                else
                {
                    var mem = new MembroDTO
                    {
                        rm = (int)membro.Id,
                        nome = membro.Nome,
                        email = membro.Email,
                        cpf = membro.Cpf,
                        congregacao = membro.Congregacao.Nome,
                        atualizarSenha = membro.AtualizarSenha,
                        foto = membro.FotoUrl,
                        membroAtualizado = membro.MembroAtualizado
                    };

                    var cargoMem = membro.Cargos.FirstOrDefault(c => c.DataCargo == membro.Cargos.Max(c => c.DataCargo));
                    if (cargoMem != null)
                    {
                        mem.cargo = cargoMem.TipoCarteirinha.ToString();
                    }
                    return ResponseOK(new { membro = mem, erro = false });
                }
            }
            catch (Exception ex)
            {
                return ResponseServerError(new Erro(ex.Message, ex));
            }
        }

        [HttpPatch("atualizarSenha")]
        [Authorize]
        public IActionResult AtualizarSenha(SenhaDTO senhaDTO)
        {
            try
            {
                _membroAppService.AtualizarSenha(senhaDTO.id, senhaDTO.novaSenha, false);
                return ResponseOK(new { mensagem = "Senha atualizada com sucesso.", erro = false });
            }
            catch (Exception ex)
            {
                return ResponseServerError(new Erro(ex.Message, ex));
            }
        }

        [HttpPatch("validarAtualizarSenha")]
        [Authorize]
        public IActionResult ValidarAtualizarSenha(ValidaAlteraSenhaDTO senhaDTO)
        {
            try
            {
                // Validar se a senha atual está correta
                if (!_membroAppService.ValidarSenha(senhaDTO.id, senhaDTO.senhaAtual))
                    return ResponseOK(new { erro = true, mensagem = "Senha atual está incorreta." });

                // Validar se a nova senha atende aos critérios de segurança
                if (!_membroAppService.ValidarSenhaForte(senhaDTO.novaSenha))
                    return ResponseOK(new
                    {
                        erro = true,
                        mensagem = "A nova senha deve ter pelo menos 6 caracteres, incluindo uma letra maiúscula, uma letra minúscula, um número e um caractere especial(!@#$%^&*()-_=+[]{};:'\",.<>?/\\|))."
                    });

                // Atualizar a senha
                _membroAppService.AtualizarSenha(senhaDTO.id, senhaDTO.senhaAtual, senhaDTO.novaSenha, false);
                _membroAppService.AtualizarSenha(senhaDTO.id, senhaDTO.novaSenha, false);
                return ResponseOK(new { mensagem = "Senha atualizada com sucesso.", erro = false });
            }
            catch (Exception ex)
            {
                return ResponseServerError(new Erro("Erro ao atualizar a senha.", ex));
            }
        }

        // Método para validar se a senha é forte

        [HttpPost("inscricao")]
        [Authorize]
        public IActionResult Inscricao([FromBody] InscricaoDTO inscricaoDTO)
        {
            try
            {
                var (validaOK, msg) = _membroAppService.InscricaoApp(inscricaoDTO.cpf, inscricaoDTO.nomeMae, inscricaoDTO.dataNascimento);
                if (!validaOK)
                {
                    return ResponseOK(new { erro = true, mensagem = msg });
                }
                return ResponseOK(new { erro = false, mensagem = msg });
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
                    return ResponseOK(new { erro = true, mensagem = msg });
                }
                return ResponseOK(new { erro = false, mensagem = msg });
            }
            catch (System.Exception ex)
            {
                return ResponseServerError(new Erro(ex.Message, ex));
            }
        }

        [HttpGet("carteirinhaFrente")]
        [Authorize]

        public IActionResult CarteirinhaFrente([FromQuery] int id)
        {
            try
            {
                var lMembro = _membroAppService.CarteirinhaMembros(id);
                if (lMembro.Count() == 0)
                    return ResponseOK(new { erro = true, mensagem = "Membro não localizado" });

                var membro = lMembro.FirstOrDefault();

                string carteirinha = "";

                switch (membro?.TipoCarteirinha)
                {
                    case Domain.Types.TipoCarteirinha.Diacono:
                        carteirinha = "obreiro_frente.png";
                        break;
                    case Domain.Types.TipoCarteirinha.Presbitero:
                        carteirinha = "obreiro_frente.png";
                        break;
                    case Domain.Types.TipoCarteirinha.Evangelista:
                        carteirinha = "obreiro_frente.png";
                        break;
                    case Domain.Types.TipoCarteirinha.Pastor:
                        carteirinha = "obreiro_frente.png";
                        break;
                    case Domain.Types.TipoCarteirinha.Cooperador:
                        carteirinha = "obreiro_frente.png";
                        break;
                    default:
                        carteirinha = "membro_frente.png";
                        break;
                }

                var caminho = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "carteirinhas", carteirinha);
                Image image = Image.Load(caminho);
                var (ok, images, mensagem) = _membroAppService.CarterinhaFrente(membro, image);
                if (ok)
                {
                    return ResponseOK(
                        new
                        {
                            frente = images,
                            mensagem
                        });
                }
                else
                {
                    return ResponseOK(new { erro = true, mensagem });
                }

            }
            catch (Exception ex)
            {
                return ResponseServerError(new { erro = false, mensagem = ex.Message });
            }

        }


        [HttpGet("carteirinhaVerso")]
        [Authorize]

        public IActionResult CarteirinhaVerso([FromQuery] int id)
        {
            try
            {
                var lMembro = _membroAppService.CarteirinhaMembros(id);
                if (lMembro.Count() == 0)
                    return ResponseOK(new { erro = true, mensagem = "Membro não localizado" });

                var membro = lMembro.FirstOrDefault();

                string carteirinha = "";

                switch (membro?.TipoCarteirinha)
                {
                    case Domain.Types.TipoCarteirinha.Membro:
                        carteirinha = "membro_verso.png";
                        break;
                    case Domain.Types.TipoCarteirinha.Diacono:
                        carteirinha = "obreiro_verso.png";
                        break;
                    case Domain.Types.TipoCarteirinha.Presbitero:
                        carteirinha = "obreiro_verso.png";
                        break;
                    case Domain.Types.TipoCarteirinha.Evangelista:
                        carteirinha = "obreiro_verso.png";
                        break;
                    case Domain.Types.TipoCarteirinha.Pastor:
                        carteirinha = "obreiro_verso.png";
                        break;
                    case Domain.Types.TipoCarteirinha.Cooperador:
                        carteirinha = "obreiro_verso.png";
                        break;
                }

                var caminho = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "carteirinhas", carteirinha);
                Image image = Image.Load(caminho);
                var (ok, images, mensagem) = _membroAppService.CarterinhaVerso(membro, image);
                if (ok)
                {
                    return ResponseOK(
                        new
                        {
                            verso = images,
                            mensagem
                        });
                }
                else
                {
                    return ResponseOK(new { erro = true, mensagem });
                }

            }
            catch (Exception ex)
            {
                return ResponseServerError(new Erro(ex.Message, ex));
            }

        }

        [HttpGet("carteirinhaQrcode")]
        [Authorize]

        public IActionResult GerarQrCode([FromQuery] int id)
        {
            try
            {
                var lMembro = _membroAppService.CarteirinhaMembros(id);
                if (lMembro.Count() == 0)
                    return ResponseBadRequest("Membro não localizado");

                var membro = lMembro.FirstOrDefault();

                var (ok, images, mensagem) = _membroAppService.GerarQrCode(membro);
                if (ok)
                {
                    return ResponseOK(
                        new
                        {
                            qrcode = images,
                            mensagem
                        });
                }
                else
                {
                    return ResponseOK(new { erro = true, mensagem });
                }

            }
            catch (Exception ex)
            {
                return ResponseServerError(new Erro(ex.Message, ex));
            }

        }


        [HttpGet("consultaCompletaMembro")]
        [Authorize]
        public IActionResult ConsultarCompletaMembro([FromQuery] int id, [FromServices] ICargoAppService cargoAppService)
        {
            try
            {
                if (id <= 0)
                    return ResponseOK(new { erro = true, mensagem = "Id é de preechimento obrigatório" });

                var membro = _membroAppService.GetById(id, 0);
                if (membro.Id != id)
                {
                    return ResponseOK(new { erro = true, mensagem = "Membro não encontrado" });
                }
                else
                {
                    var membroCompleto = new MembroCompletoDTO
                    {
                        rm = membro.Id,
                        cpf = membro.Cpf,
                        rg = membro.RG,
                        nome = membro.Nome,
                        nomePai = membro.NomePai,
                        nomeMae = membro.NomeMae,
                        email = membro.Email,
                        congregacao = membro.Congregacao?.Nome,
                        foto = membro.FotoUrl,
                        profissao = membro.Profissao,
                        telefone = membro.TelefoneCelular,
                        membroAtualizado = membro.MembroAtualizado
                    };

                    if (membro.Endereco != null)
                    {
                        membroCompleto.endereco = membro.Endereco.Logradouro;
                        membroCompleto.cep = membro.Endereco.Cep;
                        membroCompleto.numero = membro.Endereco.Numero;
                        membroCompleto.pais = membro.Endereco?.Pais;
                        membroCompleto.estado = membro.Endereco?.Estado;
                        membroCompleto.cidade = membro.Endereco?.Cidade;
                    }

                    var cargoMem = membro.Cargos.FirstOrDefault(c => c.DataCargo == membro.Cargos.Max(c => c.DataCargo));
                    if (cargoMem != null)
                    {
                        membroCompleto.cargo = cargoMem.TipoCarteirinha.ToString();
                    }
                    return ResponseOK(new { erro = true, membro = membroCompleto });
                }
            }
            catch (Exception ex)
            {
                return ResponseServerError(new Erro(ex.Message, ex));
            }
        }

        [HttpPatch("atualizarMembroAtualizado")]
        [Authorize]

        public IActionResult AtualizarMembroAtualizado([FromBody] MembroAtualizadoDTO membroAtualizado)
        {
            try
            {
                _membroAppService.AtualizarMembroAtualizado(membroAtualizado.id, membroAtualizado.membroAtualizado);
                return ResponseOK(new { erro = false, mensagem = "Membro atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                return ResponseServerError(new Erro(ex.Message, ex));
            }
        }
    }

}
