#pragma warning disable CA1034 // Nested types should not be visible

using System.Reflection;

using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.Wolverine.Extensions;
using Beatport2Rss.Common.Wolverine.Persistence;

using JasperFx.CodeGeneration.Model;
using JasperFx.Resources;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Wolverine;
using Wolverine.RabbitMQ;

namespace Beatport2Rss.Common.Wolverine;

public static class ServiceCollectionExtensions
{
    private const int DefaultMessageRetentionInDays = 100;

    extension(IServiceCollection services)
    {
        public IServiceCollection AddWolverinePublisher<TDbContext>(
            IConfiguration configuration,
            Action<WolverineOptions> configurePublishedEvents)
            where TDbContext : DbContext, IBeatport2RssDbContext =>
            services
                .AddTransient<IIntegrationEventOutbox, WolverineIntegrationEventOutbox<TDbContext>>()
                .AddTransient<IUnitOfWork, WolverineUnitOfWork<TDbContext>>()
                .AddWolverine<TDbContext>(configuration, configurePublishedEvents);

        public IServiceCollection AddWolverineListener<TDbContext>(
            IConfiguration configuration,
            Assembly handlersAssembly,
            Action<RabbitMqListenerConfiguration> configureTopics,
            Action<WolverineOptions>? configurePolicies = null)
            where TDbContext : DbContext, IBeatport2RssDbContext
        {
            return services.AddWolverine<TDbContext>(configuration, options =>
            {
                options.Durability.KeepAfterMessageHandling = TimeSpan.FromDays(DefaultMessageRetentionInDays);
                options.Policies.UseDurableLocalQueues();
                options.Discovery.IncludeAssembly(handlersAssembly);
                configurePolicies?.Invoke(options);
                configureTopics(
                    options.ListenToRabbitQueue($"{TDbContext.Schema}_events")
                        .UseDurableInbox());
            });
        }

        private IServiceCollection AddWolverine<TDbContext>(IConfiguration configuration, Action<WolverineOptions> configure)
            where TDbContext : DbContext, IBeatport2RssDbContext =>
            services
                .AddWolverine(options =>
                {
                    options.ServiceLocationPolicy = ServiceLocationPolicy.AllowedButWarn;
                    options.ConfigureWolverine<TDbContext>(configuration);
                    configure(options);
                })
                .AddResourceSetupOnStartup();
    }
}
