using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ASChurchManager.Infra.Data.Repository;
using Azure.Communication.Email;
using Azure;

namespace ASChurchManager.Functions.Email
{
    public static class EnviarEmail
    {
        [FunctionName("EnviarEmail")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Inicio da Function de envio de email.");

            string idBase64 = req.Query["id"];
            int id = Convert.ToInt32(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(idBase64)));
            string strConn = GetSqlAzureConnectionString();


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


            log.LogInformation("Fim da Function de envio de email.");

            return new OkObjectResult("Email enviado com sucesso!");
        }
        public static string GetSqlAzureConnectionString()
        {
            string conStr = Environment.GetEnvironmentVariable($"ConnectionStrings", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = System.Environment.GetEnvironmentVariable($"SQLAZURECONNSTR_", EnvironmentVariableTarget.Process);
            return conStr;
        }

        public static string GetConnectionEmail()
        {
            string conStr = Environment.GetEnvironmentVariable($"ConnectionEmail", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = System.Environment.GetEnvironmentVariable($"SQLAZUREConnectionEmail", EnvironmentVariableTarget.Process);
            return conStr;
        }
        public static string GetSenderAddress()
        {
            string conStr = Environment.GetEnvironmentVariable($"SenderAddress", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = System.Environment.GetEnvironmentVariable($"SQLAZURESenderAddress", EnvironmentVariableTarget.Process);
            return conStr;
        }
    }
}
