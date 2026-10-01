#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.SourceGenerator;

using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Builder.Application;

[GenerateValidators]
public static partial class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication() =>
            services
                .AddMediator(options =>
                {
                    options.GenerateTypesAsInternal = true;
                    options.ServiceLifetime = ServiceLifetime.Transient;
                })
                .AddValidators();
    }

    private static partial IServiceCollection AddValidators(this IServiceCollection services);
}