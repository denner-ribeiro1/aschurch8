using System;
using System.Net.Http;
using System.Text;
using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces.Repository;
using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;

namespace ASChurchManager.Application.AppServices;

public class EmailAppService : IEmailAppService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailRepository _emailRepository;
    public EmailAppService(IConfiguration configuration, IEmailRepository emailRepository)
    {
        _configuration = configuration;
        _emailRepository = emailRepository;
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

    public Email GetEmail(long id)
    {
        return _emailRepository.GetById(id, 0);
    }

    public void RequisitarEnvioEmail(int id)
    {

        byte[] arrayId = Encoding.ASCII.GetBytes(Convert.ToString(id));
        string id64 = System.Convert.ToBase64String(arrayId);
        string url = $"{_configuration["Email:AzureFunction"]}?id={id64}";

        var req = new HttpClient();
        req.GetStringAsync(url);

    }

    public long SalvarEmail(Email email)
    {
        return _emailRepository.Add(email);
    }


}
