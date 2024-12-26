using System;
using ASChurchManager.Application.AppServices;
using ASChurchManager.Application.Interfaces;
using ASChurchManager.Domain.Intefaces.Repository;
using ASChurchManager.Domain.Interfaces.Repository;
using ASChurchManager.Infra.Data.Repository;
using ASChurchManager.Infra.Data.Repository.EnterpriseLibrary;

namespace ASChurchManager.API.Membro.Services;

public class DependencyResolverServices
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
        services.AddScoped<IPaisRepository, PaisRepository>();

    }

    private static void ResolveApplications(IServiceCollection services)
    {
        //services.AddScoped(typeof(IBaseAppService<>), typeof(BaseAppService<>));
        services.AddScoped<IMembroAppService, MembroAppService>();
        services.AddScoped<IClientAPIAppServices, ClientAPIAppServices>();

    }
}