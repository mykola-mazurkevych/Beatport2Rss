#pragma warning disable CA1034 // Nested types should not be visible

using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Application;

public static partial class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication() =>
            services
                .AddMediator(o =>
                {
                    o.GenerateTypesAsInternal = true;
                    o.ServiceLifetime = ServiceLifetime.Transient;
                })
                .AddValidators()
                .AddValidationBehaviors()
                .AddUserValidationBehaviors()
                .AddFeedValidationBehaviors();
    }

    private static partial IServiceCollection AddFeedValidationBehaviors(this IServiceCollection services);
    private static partial IServiceCollection AddUserValidationBehaviors(this IServiceCollection services);
    private static partial IServiceCollection AddTagValidationBehaviors(this IServiceCollection services);
    private static partial IServiceCollection AddValidationBehaviors(this IServiceCollection services);
    private static partial IServiceCollection AddValidators(this IServiceCollection services);
}