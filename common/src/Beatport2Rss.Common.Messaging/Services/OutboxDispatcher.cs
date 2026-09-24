using System.Collections.Frozen;
using System.Text.Json;

using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.Messaging.Interfaces;
using Beatport2Rss.Common.Messaging.Options;
using Beatport2Rss.Common.Messaging.Persistence.Interfaces.Repositories;
using Beatport2Rss.Common.Miscellaneous.Interfaces;
using Beatport2Rss.Common.RabbitMQ.Interfaces;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Beatport2Rss.Common.Messaging.Services;

internal sealed class OutboxDispatcher(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<OutboxDispatcher> logger,
    IOptions<OutboxDispatcherOptions> outboxDispatcherOptions) :
    IOutboxDispatcher
{
    private static readonly FrozenDictionary<string, Type> IntegrationEventTypes = typeof(IIntegrationEvent).Assembly
        .GetTypes()
        .Where(type =>
            type is { IsClass: true, IsAbstract: false } &&
            typeof(IIntegrationEvent).IsAssignableFrom(type))
        .ToFrozenDictionary(type => type.Name);

    private readonly OutboxDispatcherOptions _outboxDispatcherOptions = outboxDispatcherOptions.Value;

    public async Task DispatchAsync(CancellationToken cancellationToken = default)
    {
        var dispatchInterval = TimeSpan.FromSeconds(_outboxDispatcherOptions.DispatchIntervalInSeconds);
        while (!cancellationToken.IsCancellationRequested)
        {
            await DispatchInternalAsync(cancellationToken);
            await Task.Delay(dispatchInterval, cancellationToken);
        }
    }

    private async Task DispatchInternalAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();

        var clock = scope.ServiceProvider.GetRequiredService<IClock>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
        var outboxMessageRepository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var messages = await outboxMessageRepository.GetNotPublishedAsync(_outboxDispatcherOptions.BatchSize, cancellationToken);
        foreach (var message in messages)
        {
            try
            {
                if (!IntegrationEventTypes.TryGetValue(message.Type, out var eventType))
                {
                    throw new InvalidOperationException($"Unsupported outbox message type '{message.Type}'.");
                }

                var payload =
                    JsonSerializer.Deserialize(message.Payload.RootElement.GetRawText(), eventType) ??
                    throw new InvalidOperationException($"Cannot deserialize outbox message '{message.Id}'.");

                if (payload is not IIntegrationEvent integrationEvent)
                {
                    throw new InvalidOperationException($"Deserialized outbox message '{message.Id}' is not an integration event.");
                }

                await publisher.PublishAsync(integrationEvent, cancellationToken);

                message.MarkPublished(clock.UtcNow);
            }
            catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
            {
                OutboxDispatcherLoggerMessages.LogPublishFailure(logger, exception, message.Id, message.Type);
                message.MarkFailed(exception.Message);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

internal static partial class OutboxDispatcherLoggerMessages
{
    [LoggerMessage(LogLevel.Error, "Unable to publish outbox message {OutboxMessageId} of type {OutboxMessageType}")]
    public static partial void LogPublishFailure(ILogger logger, Exception exception, Guid outboxMessageId, string outboxMessageType);
}