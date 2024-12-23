using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace ASChurchManager.Web
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var hostBuilder = CreateHostBuilder(args).Build();
                hostBuilder.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"Host encerrado inesperadamente. {ex.Message} {Environment.NewLine}{ex.StackTrace}");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var settings = config.Build();
                    var databaseLog = settings.GetValue<string>("ParametrosSistema:DatabaseMongoDB");
                    var collectionLog = settings.GetValue<string>("ParametrosSistema:CollectionLog");
                    Log.Logger = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .WriteTo.MongoDB($"{settings.GetConnectionString("BaseLog")}/{databaseLog}",
                            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
                            collectionName: collectionLog)
                        .CreateLogger();
                })
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(serverOptions =>
                    {
                        serverOptions.Limits.MinRequestBodyDataRate =
                            new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                        serverOptions.Limits.MinResponseDataRate =
                            new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));

                        serverOptions.Limits.MaxRequestBodySize = 209715200;
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
