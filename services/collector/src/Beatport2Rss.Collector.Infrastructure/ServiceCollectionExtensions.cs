#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Collector.Application;
using Beatport2Rss.Collector.Application.Exceptions;
using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Infrastructure.Persistence;
using Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Common.Beatport;
using Beatport2Rss.Common.BeatportTokenProvider;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;
using Beatport2Rss.Common.IntegrationEvents.Feeds;
using Beatport2Rss.Common.IntegrationEvents.Releases;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;
using Beatport2Rss.Common.Miscellaneous;
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
                .AddBeatportClient(configuration)
                .AddBeatportTokenProvider(configuration)
                .AddPersistence(configuration)
                .AddBeatportTokenProvider(configuration)
                .AddMiscellaneous()
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
                .AddWolverine<CollectorDbContext, CollectorApplication>(
                    configuration,
                    configurePublisher: options => options
                        .Publishes<SubscriptionCreated>()
                        .Publishes<ReleaseCollected>(),
                    configureListener: listener => listener
                        .ListensTo<FeedSubscriptionCreated>()
                        .ListensTo<FeedSubscriptionDeleted>()
                        .ListensTo<SubscriptionRequested>(),
                    configureRetries: options => options
                        .Retries<HttpRequestException>()
                        .Retries<RetryableBeatportRequestException>());

        private IServiceCollection AddPersistence(IConfiguration configuration) =>
            services
                .AddDbContext(configuration)
                .AddTransient(provider => provider.GetRequiredService<CollectorDbContext>().Subscriptions)
                .AddTransient(provider => provider.GetRequiredService<CollectorDbContext>().Releases)
                .AddTransient<IReleaseCommandRepository, ReleaseCommandRepository>()
                .AddTransient<ISubscriptionCommandRepository, SubscriptionCommandRepository>();
    }
}