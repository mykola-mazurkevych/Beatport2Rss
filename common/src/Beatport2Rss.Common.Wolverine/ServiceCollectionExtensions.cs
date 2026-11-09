#pragma warning disable CA1034 // Nested types should not be visible

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
        public IServiceCollection AddWolverine<TDbContext, THandlersMarker>(
            IConfiguration configuration,
            Action<WolverineOptions>? configurePublisher = null,
            Action<RabbitMqListenerConfiguration>? configureListener = null,
            Action<WolverineOptions>? configureRetries = null)
            where TDbContext : DbContext, IBeatport2RssDbContext =>
            services.AddWolverineHost<TDbContext>(configuration, options =>
            {
                if (configurePublisher is not null)
                {
                    services
                        .AddTransient<IIntegrationEventOutbox, WolverineIntegrationEventOutbox<TDbContext>>()
                        .AddTransient<IUnitOfWork, WolverineUnitOfWork<TDbContext>>();
                    configurePublisher(options);
                }

                if (configureListener is not null)
                {
                    options.Durability.KeepAfterMessageHandling = TimeSpan.FromDays(DefaultMessageRetentionInDays);
                    options.Policies.UseDurableLocalQueues();
                    options.Discovery.IncludeAssembly(typeof(THandlersMarker).Assembly);

                    var listener = options.ListenToRabbitQueue($"{TDbContext.Schema}_events").UseDurableInbox();
                    configureListener(listener);
                }

                configureRetries?.Invoke(options);
            });

        private IServiceCollection AddWolverineHost<TDbContext>(
            IConfiguration configuration,
            Action<WolverineOptions> configure)
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
