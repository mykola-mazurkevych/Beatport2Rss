#pragma warning disable CA1034 // Nested types should not be visible

using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Application;

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
                .AddRequireUserBehaviors()
                .AddRequireFeedBehaviors()
                .AddRequireTagBehaviors()
                .AddRequireSubscriptionBehaviors()
                .AddValidators();
    }

    private static partial IServiceCollection AddRequireFeedBehaviors(this IServiceCollection services);
    private static partial IServiceCollection AddRequireSubscriptionBehaviors(this IServiceCollection services);
    private static partial IServiceCollection AddRequireTagBehaviors(this IServiceCollection services);
    private static partial IServiceCollection AddRequireUserBehaviors(this IServiceCollection services);
    private static partial IServiceCollection AddValidators(this IServiceCollection services);
}