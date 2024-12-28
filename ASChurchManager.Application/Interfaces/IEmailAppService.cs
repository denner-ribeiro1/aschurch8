using System;

namespace ASChurchManager.Application.Interfaces;

public interface IEmailAppService
{
    void EnviarEmail(string email, string titulo, string mensagemHtml, string mensagemPadrao = "");
}
