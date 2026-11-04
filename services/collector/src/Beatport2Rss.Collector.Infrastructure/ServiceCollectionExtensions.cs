#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Collector.Application;
using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Infrastructure.Persistence;
using Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Common.EntityFrameworkCore;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;
using Beatport2Rss.Common.Wolverine;
using Beatport2Rss.Common.Wolverine.Extensions;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Collector.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration) =>
            services
                .AddPersistence(configuration)
                .AddMessaging(configuration);

        public IServiceCollection AddMigrator(IConfiguration configuration) =>
            services
                .AddDbContext(configuration)
                .AddTransient(provider => provider.GetRequiredService<CollectorDbContext>().GetService<IMigrator>());

        private IServiceCollection AddDbContext(IConfiguration configuration) =>
            services
                .AddDbContext<CollectorDbContext>(builder => builder
                    .UseNpgsql(
                        configuration.GetConnectionString(nameof(CollectorDbContext)),
                        CollectorDbContext.Schema));

        private IServiceCollection AddMessaging(IConfiguration configuration) =>
            services
                .AddWolverineListener<CollectorDbContext>(
                    configuration,
                    typeof(CollectorApplication).Assembly,
                    listener => listener.ListensTo<SubscriptionCreated>());

        private IServiceCollection AddPersistence(IConfiguration configuration) =>
            services
                .AddDbContext(configuration)
                .AddUnitOfWork<CollectorDbContext>()
                .AddTransient(provider => provider.GetRequiredService<CollectorDbContext>().Subscriptions)
                .AddTransient<ISubscriptionCommandRepository, SubscriptionCommandRepository>();
    }
}