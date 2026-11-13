#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Builder.Application;
using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Builder.Application.UseCases.Releases.Commands;
using Beatport2Rss.Builder.Infrastructure.Persistence;
using Beatport2Rss.Builder.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Common.EntityFrameworkCore;
using Beatport2Rss.Common.EntityFrameworkCore.Extensions;
using Beatport2Rss.Common.IntegrationEvents.Feeds;
using Beatport2Rss.Common.IntegrationEvents.Releases;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;
using Beatport2Rss.Common.IntegrationEvents.Tags;
using Beatport2Rss.Common.Wolverine;
using Beatport2Rss.Common.Wolverine.Extensions;

using FluentValidation;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Beatport2Rss.Builder.Infrastructure;

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
                .AddTransient(provider => provider.GetRequiredService<BuilderDbContext>().GetService<IMigrator>());

        private IServiceCollection AddDbContext(IConfiguration configuration) =>
            services
                .AddDbContext<BuilderDbContext>(builder => builder
                    .UseNpgsql(
                        configuration.GetConnectionString(nameof(BuilderDbContext)),
                        BuilderDbContext.Schema));

        private IServiceCollection AddMessaging(IConfiguration configuration) =>
            services
                .AddTransient<IValidator<CreateFeedCommand>, CreateFeedCommandValidator>()
                .AddTransient<IValidator<UpdateFeedCommand>, UpdateFeedCommandValidator>()
                .AddTransient<IValidator<DeleteFeedCommand>, DeleteFeedCommandValidator>()
                .AddTransient<IValidator<DeleteStaleReleasesCommand>, DeleteStaleReleasesCommandValidator>()
                .AddWolverine<BuilderDbContext, BuilderApplication>(
                    configuration,
                    configurePublisher: null,
                    configureListener: listener => listener
                        .ListensTo<FeedCreated>()
                        .ListensTo<FeedUpdated>()
                        .ListensTo<FeedDeleted>()
                        .ListensTo<TagCreated>()
                        .ListensTo<TagUpdated>()
                        .ListensTo<TagDeleted>()
                        .ListensTo<ReleaseCollected>()
                        .ListensTo<SubscriptionCreated>());

        private IServiceCollection AddPersistence(IConfiguration configuration) =>
            services
                .AddDbContext(configuration)
                .AddUnitOfWork<BuilderDbContext>()
                .AddTransient(provider => provider.GetRequiredService<BuilderDbContext>().Feeds)
                .AddTransient(provider => provider.GetRequiredService<BuilderDbContext>().Releases)
                .AddTransient(provider => provider.GetRequiredService<BuilderDbContext>().Subscriptions)
                .AddTransient(provider => provider.GetRequiredService<BuilderDbContext>().Tags)
                .AddTransient<IFeedCommandRepository, FeedCommandRepository>()
                .AddTransient<ITagCommandRepository, TagCommandRepository>()
                .AddTransient<IReleaseCommandRepository, ReleaseCommandRepository>()
                .AddTransient<ISubscriptionCommandRepository, SubscriptionCommandRepository>();
    }
}