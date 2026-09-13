using Beatport2Rss.Collector.Application.UseCases.Subscriptions;
using Beatport2Rss.Common.IntegrationEvents.V1.Feeds;
using Beatport2Rss.Common.IntegrationEvents.V1.Subscriptions;
using Beatport2Rss.Common.Messaging.Interfaces;

using Mediator;

namespace Beatport2Rss.Collector.Worker;

internal abstract class SubscriptionRabbitMqWorker<TMessage>(
    IConsumer<TMessage> consumer,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<SubscriptionRabbitMqWorker<TMessage>> logger) :
    BackgroundService
    where TMessage : class
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await consumer.ConsumeAsync(HandleMessageAsync, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                SubscriptionRabbitMqWorkerLogMessages.ConsumerStopped(logger, exception, typeof(TMessage).Name);

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    protected abstract Task HandleAsync(TMessage message, IServiceProvider serviceProvider, CancellationToken cancellationToken);

    private async Task HandleMessageAsync(TMessage message, CancellationToken cancellationToken)
    {
        using var serviceScope = serviceScopeFactory.CreateScope();
        await HandleAsync(message, serviceScope.ServiceProvider, cancellationToken);
    }
}

internal sealed class SubscriptionCreatedRabbitMqWorker(
    IConsumer<SubscriptionCreatedV1> consumer,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<SubscriptionRabbitMqWorker<SubscriptionCreatedV1>> logger) :
    SubscriptionRabbitMqWorker<SubscriptionCreatedV1>(consumer, serviceScopeFactory, logger)
{
    protected override async Task HandleAsync(SubscriptionCreatedV1 message, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var command = new CreateSubscriptionCommand(
            message.SubscriptionId,
            message.Type,
            message.BeatportId,
            message.BeatportSlug);
        var result = await serviceProvider.GetRequiredService<ISender>().Send(command, cancellationToken);

        if (result.IsFailed)
        {
            throw new InvalidOperationException(result.Errors[0].Message);
        }
    }
}

internal sealed class FeedSubscriptionCreatedRabbitMqWorker(
    IConsumer<FeedSubscriptionCreatedV1> consumer,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<SubscriptionRabbitMqWorker<FeedSubscriptionCreatedV1>> logger) :
    SubscriptionRabbitMqWorker<FeedSubscriptionCreatedV1>(consumer, serviceScopeFactory, logger)
{
    protected override async Task HandleAsync(FeedSubscriptionCreatedV1 message, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var command = new IncreaseSubscribersCountCommand(message.SubscriptionId);
        var result = await serviceProvider.GetRequiredService<ISender>().Send(command, cancellationToken);

        if (result.IsFailed)
        {
            throw new InvalidOperationException(result.Errors[0].Message);
        }
    }
}

internal sealed class FeedSubscriptionDeletedRabbitMqWorker(
    IConsumer<FeedSubscriptionDeletedV1> consumer,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<SubscriptionRabbitMqWorker<FeedSubscriptionDeletedV1>> logger) :
    SubscriptionRabbitMqWorker<FeedSubscriptionDeletedV1>(consumer, serviceScopeFactory, logger)
{
    protected override async Task HandleAsync(FeedSubscriptionDeletedV1 message, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var command = new DecreaseSubscribersCountCommand(message.SubscriptionId);
        var result = await serviceProvider.GetRequiredService<ISender>().Send(command, cancellationToken);

        if (result.IsFailed)
        {
            throw new InvalidOperationException(result.Errors[0].Message);
        }
    }
}

internal static partial class SubscriptionRabbitMqWorkerLogMessages
{
    [LoggerMessage(Level = LogLevel.Error, Message = "RabbitMQ consumer for {MessageType} stopped. Retrying.")]
    public static partial void ConsumerStopped(ILogger logger, Exception exception, string messageType);
}