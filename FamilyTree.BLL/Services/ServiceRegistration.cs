using Microsoft.Extensions.DependencyInjection;

namespace FamilyTree.BLL.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddServices(this IServiceCollection services) => services
        .AddScoped<IFamilyTreeService, FamilyService>()
    ;
}