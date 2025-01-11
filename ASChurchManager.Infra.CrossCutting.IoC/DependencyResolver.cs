using Microsoft.Extensions.DependencyInjection;
using ASChurchManager.Application.AppServices;
using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Infra.Data.Repository;
using ASChurchManager.Infra.Data.Repository.EnterpriseLibrary;
using ASChurchManager.Domain.Entities;
using ASChurchManager.Domain.Interfaces;
using ASChurchManager.Domain.Interfaces.Repository;

namespace ASChurchManager.Infra.CrossCutting.IoC
{
    public class DependencyResolver
    {
        public static void Dependency(IServiceCollection services)
        {
            ResolveRespositories(services);
            ResolveApplications(services);
        }

        private static void ResolveRespositories(IServiceCollection services)
        {
            //services.AddScoped(typeof(IRepositoryDAO<>), typeof(RepositoryDAO<>));
            services.AddScoped<IMembroRepository, MembroRepository>();
            services.AddScoped<ICongregacaoRepository, CongregacaoRepository>();
            services.AddScoped<ICargoRepository, CargoRepository>();
            services.AddScoped<ITipoEventoRepository, TipoEventoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<INascimentoRepository, NascimentoRepository>();
            services.AddScoped<ICasamentoRepository, CasamentoRepository>();
            services.AddScoped<IRotinaRepository, RotinaRepository>();
            services.AddScoped<IBatismoRepository, BatismoRepository>();
            services.AddScoped<ICartaRepository, CartaRepository>();
            services.AddScoped<IPerfilRepository, PerfilRepository>();
            services.AddScoped<IEventosRepository, EventosRepository>();
            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
            services.AddScoped<IRelatoriosSecretariaRepository, RelatoriosSecretariaRepository>();
            services.AddScoped<IGrupoRepository, GrupoRepository>();
            services.AddScoped<ICursoRepository, CursoRepository>();
            services.AddScoped<ICursoArquivoMembroRepository, CursoArquivoMembroRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IPresencaRepository, PresencaRepository>();
            services.AddScoped<IPaisRepository, PaisRepository>();
            services.AddScoped<IEmailRepository, EmailRepository>();
        }

        private static void ResolveApplications(IServiceCollection services)
        {
            //services.AddScoped(typeof(IBaseAppService<>), typeof(BaseAppService<>));
            services.AddScoped<ICargoAppService, CargoAppService>();
            services.AddScoped<ICursoAppService, CursoAppService>();
            services.AddScoped<IGrupoAppService, GrupoAppService>();
            services.AddScoped<IPerfilAppService, PerfilAppService>();
            services.AddScoped<IRotinaAppService, RotinaAppService>();
            services.AddScoped<ITemplateAppService, TemplateAppService>();
            services.AddScoped<ITipoEventoAppService, TipoEventoAppService>();
            services.AddScoped<IUsuarioAppService, UsuarioAppService>();
            services.AddScoped<IMembroAppService, MembroAppService>();
            services.AddScoped<ICongregacaoAppService, CongregacaoAppService>();
            services.AddScoped<ICartaAppService, CartaAppService>();
            services.AddScoped<IPrinterAPIAppService, PrinterAPIAppService>();
            services.AddScoped<IClientAPIAppServices, ClientAPIAppServices>();
            services.AddScoped<IDashboardAppService, DashboardAppService>();
            services.AddScoped<INascimentoAppService, NascimentoAppService>();
            services.AddScoped<ICasamentoAppService, CasamentoAppService>();
            services.AddScoped<IBatismoAppService, BatismoAppService>();
            services.AddScoped<IEventosAppService, EventosAppService>();
            services.AddScoped<IPrinterAppService, PrinterAppService>();
            services.AddScoped<IAuditoriaAppService, AuditoriaAppService>();
            services.AddScoped<IRelatoriosSecretariaAppService, RelatoriosSecretariaAppService>();
            services.AddScoped<ICursoArquivoMembroAppService, CursoArquivoMembroAppService>();
            services.AddScoped<IArquivosAzureAppService, ArquivosAzureAppService>();
            services.AddScoped<IRelatorioAPISecretariaAppService, RelatorioAPISecretariaAppService>();
            services.AddScoped<IUsuarioLogado, UsuarioLogadoAppServices>();
            services.AddScoped<IPresencaAppService, PresencaAppService>();
            services.AddScoped<IImpressaoMembroAppService, ImpressaoMembroAppService>();
            services.AddScoped<IEmailAppService, EmailAppService>();
        }
    }
}
