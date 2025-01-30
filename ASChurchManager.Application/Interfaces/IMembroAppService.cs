using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Intefaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASChurchManager.Application.Interfaces
{
    public interface IMembroAppService : IMembroRepository
    {
        IEnumerable<Carteirinha> CarteirinhaMembro(long membroId);
        Carteirinha CarteirinhaMembro(int membroId, bool atualizaValidade);
        bool FichaMembro(int id, int usuarioId, out byte[] relatorio, out string mimeType);
        Task DeleteAndDeleteFilesAsync(long id);
        IEnumerable<Pais> ConsultarPaises();
        (bool, Membro) ValidarLogin(string cpf, string senha);
        bool ValidarSenha(int id, string senhaAtual);
        (bool, string) InscricaoApp(string cpf, string nomeMae, DateTime dataNascimento);
        (bool, string) RecuperarSenha(string cpf);
        (bool, byte[], string) CarterinhaFrente(Carteirinha membro, SixLabors.ImageSharp.Image image);
        (bool, byte[], string) CarterinhaVerso(Carteirinha membro, SixLabors.ImageSharp.Image image);
        (bool, byte[], string) GerarQrCode(Carteirinha membro);

    }
}