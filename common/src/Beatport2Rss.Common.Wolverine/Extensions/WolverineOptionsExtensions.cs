#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.Wolverine.Options;

using FluentValidation;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using Wolverine.FluentValidation;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

namespace Beatport2Rss.Common.Wolverine.Extensions;

public static class WolverineOptionsExtensions
{
    private const string MessagingSchemaSuffix = "messaging";

    extension(WolverineOptions options)
    {
        public WolverineOptions Publishes<TIntegrationEvent>()
            where TIntegrationEvent : IIntegrationEvent
        {
            options
                .PublishMessage<TIntegrationEvent>()
                .ToRabbitTopic(
                    IntegrationEventTopicResolver.GetTopicName<TIntegrationEvent>(),
                    RabbitMQTopology.IntegrationEventsExchange)
                .UseDurableOutbox();
            return options;
        }

        public WolverineOptions Retries<TException>()
            where TException : Exception
        {
            options
                .OnException<TException>()
                .RetryWithCooldown(
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(60))
                .WithBoundedJitter(0.25);
            return options;
        }

        internal void ConfigureWolverine<TDbContext>(IConfiguration configuration)
            where TDbContext : DbContext, IBeatport2RssDbContext
        {
            var rabbitMQOptions =
                configuration.GetRequiredSection(nameof(RabbitMQOptions)).Get<RabbitMQOptions>() ??
                throw new InvalidOperationException($"Configuration section '{nameof(RabbitMQOptions)}' is required.");

            options
                .UseRabbitMq(connectionFactory =>
                {
                    connectionFactory.HostName = rabbitMQOptions.HostName;
                    connectionFactory.Port = rabbitMQOptions.Port;
                    connectionFactory.UserName = rabbitMQOptions.UserName;
                    connectionFactory.Password = rabbitMQOptions.Password;
                    connectionFactory.VirtualHost = rabbitMQOptions.VirtualHost;
                })
                .DeclareExchange(
                    RabbitMQTopology.IntegrationEventsExchange,
                    exchange => exchange.ExchangeType = ExchangeType.Topic);

            var connectionStringName = typeof(TDbContext).Name;
            var connectionString =
                configuration.GetConnectionString(connectionStringName) ??
                throw new InvalidOperationException($"Connection string '{connectionStringName}' is required.");
            var schemaName = $"{TDbContext.Schema}_{MessagingSchemaSuffix}";
            options.PersistMessagesWithPostgresql(connectionString, schemaName);
            options.UseEntityFrameworkCoreTransactions();
            options.Policies.AutoApplyTransactions();

            options.UseFluentValidation();
            options.Policies.OnException<ValidationException>().MoveToErrorQueue();
        }
    }
}