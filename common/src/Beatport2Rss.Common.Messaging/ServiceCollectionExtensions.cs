#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Common.Messaging.Interfaces;
using Beatport2Rss.Common.Messaging.Options;
using Beatport2Rss.Common.Messaging.Persistence.Interfaces;
using Beatport2Rss.Common.Messaging.Persistence.Interfaces.Repositories;
using Beatport2Rss.Common.Messaging.Persistence.Repositories;
using Beatport2Rss.Common.Messaging.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Common.Messaging;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMessaging(IConfiguration configuration) =>
            services
                .ConfigureOptions(configuration)
                .AddSingleton<IOutboxDispatcher, OutboxDispatcher>()
                .AddTransient<IIntegrationEventOutbox, IntegrationEventOutbox>()
                .AddTransient<IOutboxMessageRepository, OutboxMessageRepository>();

        public IServiceCollection AddOutboxDbContext<TOutboxDbContext>()
            where TOutboxDbContext : class, IOutboxDbContext =>
            services
                .AddTransient<IOutboxDbContext, TOutboxDbContext>();

        private IServiceCollection ConfigureOptions(IConfiguration configuration) =>
            services
                .Configure<OutboxDispatcherOptions>(options => configuration.GetSection(nameof(OutboxDispatcherOptions)).Bind(options));
    }
}