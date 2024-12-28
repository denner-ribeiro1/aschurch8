using System;
using System.Collections.Generic;
using ASChurchManager.Application.Interfaces;
using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;

namespace ASChurchManager.Application.AppServices;

public class EmailAppService : IEmailAppService
{
    private readonly IConfiguration _configuration;
    public EmailAppService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void EnviarEmail(string email, string titulo, string mensagemHtml, string mensagemPadrao = "")
    {
        string connectionString = _configuration["Email:ConnectionEmail"];
        var emailClient = new EmailClient(connectionString);

        var emailMessage = new EmailMessage(
            senderAddress: _configuration["Email:SenderAddress"],
            content: new EmailContent(titulo)
            {
                PlainText = mensagemPadrao,
                Html = mensagemHtml
            },
            recipients: new EmailRecipients([new EmailAddress(email)]));


        emailClient.Send(
            WaitUntil.Completed,
            emailMessage);

    }

}
