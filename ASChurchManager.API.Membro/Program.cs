using System.Text;
using ASChurchManager.API.Membro.Infra;
using ASChurchManager.API.Membro.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(opt =>
    {
        opt.AddPolicy("CorsPolicy", policy =>
        {
            policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
        });
    });

builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.PrivateKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddTransient<TokenServices>();
builder.Services.AddScoped<SiteExceptionFilter>();
// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Adiciona variáveis de ambiente (útil para servidores em produção)
builder.Configuration.AddEnvironmentVariables();

/*Injeção de dependência das classes que serão utilizadas no projeto*/
var config = builder.Configuration;
builder.Services.AddSingleton<IConfiguration>(config);
ASChurchManager.Infra.CrossCutting.IoC.DependencyResolver.Dependency(builder.Services);
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

var settings = config;
var databaseLog = settings.GetValue<string>("ParametrosSistema:DatabaseMongoDB");
var collectionLog = settings.GetValue<string>("ParametrosSistema:CollectionLog");

var logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.MongoDB($"{settings.GetConnectionString("BaseLog")}/{databaseLog}",
                     restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
                     collectionName: collectionLog ?? "")
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.MapControllers();

// Usa o Middleware de autenticação
app.UseAuthentication();
app.UseAuthorization();

app.Run();
