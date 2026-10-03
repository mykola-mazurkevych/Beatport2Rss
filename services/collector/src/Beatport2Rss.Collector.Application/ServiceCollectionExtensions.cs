using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Collector.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services) =>
        services.AddMediator(options =>
        {
            options.GenerateTypesAsInternal = true;
            options.ServiceLifetime = ServiceLifetime.Transient;
        });
}