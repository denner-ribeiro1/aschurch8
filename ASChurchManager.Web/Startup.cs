using ASChurchManager.Infra.CrossCutting.IoC;
using ASChurchManager.Web.AutoMapperConfiguration;
using ASChurchManager.Web.Filters;
using ASChurchManager.Web.Lib;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PdfSharpCore.Fonts;
using System;
using System.Collections.Generic;
using System.Globalization;
using Wkhtmltopdf.NetCore;

namespace ASChurchManager.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddMemoryCache();

            //services.AddDistributedRedisCache(options =>
            //{
            //    options.Configuration =
            //        Configuration.GetConnectionString("ConexaoRedis");
            //    options.InstanceName = "ASChurch-";
            //});

            // The following line enables Application Insights telemetry collection.
            services.AddApplicationInsightsTelemetry();

            services.Configure<FormOptions>(x =>
            {
                x.MultipartBodyLengthLimit = 1209715200;
            });

            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("pt-BR"),
                        };

                    options.DefaultRequestCulture = new RequestCulture(culture: "pt-BR", uiCulture: "pt-BR");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                });

            services.AddWkhtmltopdf();

            services.AddMvc()
                //INCLUSAO PARA AO SERIALIZAR ALGO EM JSON O MESMO NÃO FIQUE EM LOWERCASE
                .AddJsonOptions(op => op.JsonSerializerOptions.PropertyNamingPolicy = null);

            //services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IMemoryCache

            var expireMinutes = Configuration["UserSessionExpirationMinutes"].TryConvertTo<int>();

            //SUPORTE A SESSION
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(expireMinutes);
            });

            //AUTENTICACAO
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/Auth/NaoAutorizado";
                    options.LoginPath = "/Auth/Login";
                });

            services.AddCors(options => options.AddPolicy("Cors",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("Content-Disposition");
                }));

            //CONFIGURAÇÃO - APPSETTINGS
            services.AddSingleton(Configuration);

            //MAPEAMENTO DO AUTOMAPPER
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ViewModelToDomainMappingProfile());
                mc.AddProfile(new DomainToViewModelMappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();

            //INJECAO DE DEPENDENCIA
            services.AddSingleton(mapper);

            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<SiteExceptionFilter>();

            // ASP.NET HttpContext dependency
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            DependencyResolver.Dependency(services);

            services.AddScoped<LogAuditoriaAttribute>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors();

            IOptions<RequestLocalizationOptions> localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOptions.Value);

            // IMPORTANT: This session call MUST go before UseMvc()
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "Administracao",
                    areaName: "Admin",
                    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "Secretaria",
                    areaName: "Secretaria",
                    pattern: "Secretaria/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            GlobalFontSettings.FontResolver = new FileFontResolver(env);
        }
    }
}
