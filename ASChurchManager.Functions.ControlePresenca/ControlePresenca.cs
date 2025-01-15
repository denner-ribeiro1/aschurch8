using System;
using System.Data.SqlClient;
using ASChurchManager.Infra.Data.Repository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ASChurchManager.Function.ControlePresenca
{
    public class ControlePresenca
    {
        private readonly ILogger _logger;

        public ControlePresenca(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ControlePresenca>();
        }

        [Function("ControlePresenca")]
        public void Run([TimerTrigger("0 0 */3 * * *")] TimerInfo myTimer)
        {
            try
            {
                //"0 0 */3 * * *"
                //"0 */1 * * * *"
                //"0 0 10-23 * * *"
                _logger.LogInformation($"Trigger - Controle de Presença - Cursos/Eventos. Inicio: {DateTime.Now}");
                var strConn = GetConnectionDB();  //"Server=tcp:q2bbdo4cxr.database.windows.net,1433;Database=ieadmaua;User ID=aschurchmanager@q2bbdo4cxr;Password=L0ngh0rn;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";//Environment.GetEnvironmentVariable("ConnectionDB");//ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;

                var rep = new PresencaRepository(strConn);
                var itens = rep.ListarPresencaDatasEmAndamento();
                _logger.LogInformation($"Quantidade de Cursos/Eventos para finalizar: {itens.Count}");

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
                        _logger.LogInformation($"{rows} Curso/Evento(s) finalizado(s).");
                    }
                    conn.Close();
                }
                _logger.LogInformation($"Trigger - Controle de Presenca - Cursos/Eventos. Fim: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro - Controle de Presenca - Cursos/Eventos Mensagem Original: {ex.Message}");
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
    }
}
