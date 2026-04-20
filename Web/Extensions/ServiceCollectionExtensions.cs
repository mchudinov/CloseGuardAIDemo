using CloseGuardAIDemo.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CloseGuardAIDemo.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<DataService>();
        return services;
    }
}
