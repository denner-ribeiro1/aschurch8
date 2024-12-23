using ASChurchManager.Infra.Data.Repository;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;

namespace ASChurchManager.Functions
{
    public class ControlePresenca
    {
        [FunctionName("ControlePresenca")]
        public void Run([TimerTrigger("0 0 */3 * * *")] TimerInfo myTimer, ILogger log)
        {
            try
            {
                //"0 0 */3 * * *"
                //"0 */1 * * * *"
                //"0 0 10-23 * * *"
                log.LogInformation($"Trigger - Controle de Presen�a - Cursos/Eventos. Inicio: {DateTime.Now}");
                var strConn = GetSqlAzureConnectionString("sql");//"Server=tcp:q2bbdo4cxr.database.windows.net,1433;Database=ieadmaua;User ID=aschurchmanager@q2bbdo4cxr;Password=L0ngh0rn;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";//Environment.GetEnvironmentVariable("ConnectionDB");//ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;

                var rep = new PresencaRepository(strConn);
                var itens = rep.ListarPresencaDatasEmAndamento();
                log.LogInformation($"Quantidade de Cursos/Eventos para finalizar: {itens.Count}");

                if (itens.Count > 0)
                {
                    var conn = new SqlConnection(strConn);
                    conn.Open();
                    foreach (var item in itens)
                    {
                        SqlCommand cmd = new SqlCommand
                        {
                            Connection = conn,
                            CommandType = System.Data.CommandType.StoredProcedure,
                            CommandText = "AtualizarStatusDatas"
                        };
                        cmd.Parameters.Add(new SqlParameter("@DataId", item.Id));
                        cmd.Parameters.Add(new SqlParameter("@Status", Domain.Types.StatusPresenca.EmAberto));
                        
                        var rows = cmd.ExecuteNonQuery();
                        log.LogInformation($"{rows} Curso/Evento(s) finalizado(s).");
                    }
                    conn.Close();
                }
                log.LogInformation($"Trigger - Controle de Presenca - Cursos/Eventos. Fim: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Erro - Controle de Presenca - Cursos/Eventos Mensagem Original: {ex.Message}");
                throw;
            }
        }

        public static string GetSqlAzureConnectionString(string name)
        {
            string conStr = Environment.GetEnvironmentVariable($"ConnectionStrings:{name}", EnvironmentVariableTarget.Process);
            if (string.IsNullOrEmpty(conStr)) // Azure Functions App Service naming convention
                conStr = System.Environment.GetEnvironmentVariable($"SQLAZURECONNSTR_{name}", EnvironmentVariableTarget.Process);
            return conStr;
        }
    }
}
