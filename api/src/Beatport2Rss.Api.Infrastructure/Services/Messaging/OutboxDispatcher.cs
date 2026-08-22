using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Linq.Expressions;
using System.Text.Json;

using Beatport2Rss.Api.Application.Interfaces.Services.Misc;
using Beatport2Rss.Api.Infrastructure.Persistence;
using Beatport2Rss.Api.Infrastructure.Persistence.Outbox;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.Messaging.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Beatport2Rss.Api.Infrastructure.Services.Messaging;

internal sealed partial class OutboxDispatcher(
    IServiceScopeFactory serviceScopeFactory,
    IPublisher publisher,
    IClock clock,
    ILogger<OutboxDispatcher> logger) :
    BackgroundService
{
    private const int BatchSize = 50;
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);

    private static readonly FrozenDictionary<string, Type> IntegrationEventTypes = typeof(IIntegrationEvent).Assembly
        .GetTypes()
        .Where(type =>
            type is { IsClass: true, IsAbstract: false } &&
            typeof(IIntegrationEvent).IsAssignableFrom(type))
        .ToFrozenDictionary(type => type.Name);

    private static readonly ConcurrentDictionary<Type, Func<IPublisher, object, CancellationToken, Task>> PublishDispatchers = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await DispatchPendingMessagesAsync(stoppingToken);
            await Task.Delay(PollingInterval, stoppingToken);
        }
    }

    private async Task DispatchPendingMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
        var messages = await dbContext.OutboxMessages
            .Where(message => message.PublishedAt == null)
            .OrderBy(message => message.OccurredAt)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                await PublishAsync(message, cancellationToken);
                message.MarkPublished(clock.UtcNow);
            }
            catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
            {
                LogPublishFailure(logger, exception, message.Id, message.Type);
                message.MarkFailed(exception.Message);
            }
        }

        if (messages.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        if (!IntegrationEventTypes.TryGetValue(message.Type, out var eventType))
        {
            throw new InvalidOperationException($"Unsupported outbox message type '{message.Type}'.");
        }

        var integrationEvent = Deserialize(message, eventType);
        var dispatch = PublishDispatchers.GetOrAdd(eventType, CreatePublishDispatcher);
        return dispatch(publisher, integrationEvent, cancellationToken);
    }

    private static object Deserialize(OutboxMessage message, Type eventType) =>
        JsonSerializer.Deserialize(message.Payload.RootElement.GetRawText(), eventType) ??
        throw new InvalidOperationException($"Cannot deserialize outbox message '{message.Id}'.");

    private static Func<IPublisher, object, CancellationToken, Task> CreatePublishDispatcher(Type eventType)
    {
        var publisherParameter = Expression.Parameter(typeof(IPublisher), "publisher");
        var messageParameter = Expression.Parameter(typeof(object), "message");
        var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

        var publishMethod = typeof(IPublisher)
            .GetMethod(nameof(IPublisher.PublishAsync))!
            .MakeGenericMethod(eventType);

        var call = Expression.Call(
            publisherParameter,
            publishMethod,
            Expression.Convert(messageParameter, eventType),
            cancellationTokenParameter);

        return Expression
            .Lambda<Func<IPublisher, object, CancellationToken, Task>>(call, publisherParameter, messageParameter, cancellationTokenParameter)
            .Compile();
    }

    [LoggerMessage(LogLevel.Error, "Unable to publish outbox message {OutboxMessageId} of type {OutboxMessageType}")]
    private static partial void LogPublishFailure(ILogger logger, Exception exception, Guid outboxMessageId, string outboxMessageType);
}