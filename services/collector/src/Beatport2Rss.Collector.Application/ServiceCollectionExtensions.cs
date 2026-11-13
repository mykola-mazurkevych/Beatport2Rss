#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Collector.Application.Interfaces.Services;
using Beatport2Rss.Collector.Application.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Collector.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication() =>
            services.AddTransient<ISubscriptionService, SubscriptionService>();
    }
}