using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ASChurchManager.Infra.Data.Repository;
using Azure.Communication.Email;
using Azure;

namespace ASChurchManager.Functions.Email
{
    public class EnviarEmail
    {
        private readonly ILogger<EnviarEmail> _logger;

        public EnviarEmail(ILogger<EnviarEmail> logger)
        {
            _logger = logger;
        }

        [Function("EnviarEmail")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            try
            {
                _logger.LogInformation($"Inicio da Function de envio de email.{DateTime.Now}");

                string? idBase64 = req.Query["id"].ToString();
                int id = 0;
                if (idBase64 != null)
                {
                    id = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(idBase64)));
                }


                string strConn = GetConnectionDB();
                var emailRep = new EmailRepository(strConn);
                var email = emailRep.GetById(id, 0);

                string connectionString = GetConnectionEmail();
                var emailClient = new EmailClient(connectionString);
                var emailMessage = new EmailMessage(
                    senderAddress: GetSenderAddress(),
                    content: new EmailContent(email.Assunto)
                    {
                        Html = email.Corpo
                    },
                    recipients: new EmailRecipients([new EmailAddress(email.Endereco)]));

                emailClient.Send(
                    WaitUntil.Completed,
                    emailMessage);

                _logger.LogInformation($"Fim da Function de envio de email.{DateTime.Now}");
                return new OkObjectResult("Email Enviado com Sucesso!");
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Erro na Function de envio de email.{ex.Message}");
                throw;
            }
        }

        public static string GetConnectionDB()
        {
            string? conStr = Environment.GetEnvironmentVariable("ConnectionStrings:ConnectionDB", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = Environment.GetEnvironmentVariable("ConnectionDB", EnvironmentVariableTarget.Process);
            return conStr ?? string.Empty;
        }

        public static string GetConnectionEmail()
        {
            string? conStr = Environment.GetEnvironmentVariable("ConnectionStrings:ConnectionEmail", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = Environment.GetEnvironmentVariable("ConnectionEmail", EnvironmentVariableTarget.Process);
            return conStr ?? string.Empty;
        }
        public static string GetSenderAddress()
        {
            string? conStr = Environment.GetEnvironmentVariable("ConnectionStrings:SenderAddress", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = System.Environment.GetEnvironmentVariable("SenderAddress", EnvironmentVariableTarget.Process);
            return conStr ?? string.Empty;
        }
    }
}
