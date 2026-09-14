using System.Text.Json;

using Beatport2Rss.Common.Messaging.Interfaces;
using Beatport2Rss.Common.Messaging.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Beatport2Rss.Common.Messaging.Services;

internal sealed class RabbitMqConsumer<TMessage>(
    IRabbitMqConnectionFactory connectionFactory,
    IOptions<QueueOptions> queueOptions,
    ILogger<RabbitMqConsumer<TMessage>> logger) :
    IConsumer<TMessage>, IDisposable
    where TMessage : class
{
    private readonly QueueOptions _queueOptions = queueOptions.Value;
    private IConnection? _connection;
    private IModel? _channel;
    private bool _disposed;

    public async Task ConsumeAsync(
        Func<TMessage, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var messageTypeName = typeof(TMessage).Name;
        var queue = _queueOptions.Queues[messageTypeName];
        var routingKey = _queueOptions.RoutingKeys[messageTypeName];

        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        RabbitMqTopology.DeclareExchange(_channel, _queueOptions.ExchangeName);
        RabbitMqTopology.DeclareQueueWithBinding(_channel, _queueOptions.ExchangeName, queue, routingKey, _queueOptions.DeadLetterSuffix);

        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += (_, eventArgs) => ProcessMessageAsync(eventArgs, handler, queue, cancellationToken);

        _channel.BasicConsume(queue, autoAck: false, consumer);
        await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    private async Task ProcessMessageAsync(
        BasicDeliverEventArgs eventArgs,
        Func<TMessage, CancellationToken, Task> handler,
        string queue,
        CancellationToken cancellationToken)
    {
        var channel = _channel ?? throw new InvalidOperationException("RabbitMQ channel is not configured.");

        try
        {
            var body = eventArgs.Body.ToArray();
            var message = JsonSerializer.Deserialize<TMessage>(body, RabbitMqConsumerSerializerOptions.Value) ??
                          throw new InvalidOperationException($"Received a null message payload for '{typeof(TMessage).Name}'.");

            await handler(message, cancellationToken);

            channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            RabbitMqConsumerLogMessages.FailedToProcessMessage(logger, exception, queue);
            channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _channel?.Dispose();
        _connection?.Dispose();
        _disposed = true;
    }
}

internal static class RabbitMqConsumerSerializerOptions
{
    public static readonly JsonSerializerOptions Value = new() { PropertyNameCaseInsensitive = true, };
}

internal static partial class RabbitMqConsumerLogMessages
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to process RabbitMQ message from queue {QueueName}")]
    public static partial void FailedToProcessMessage(ILogger logger, Exception exception, string queueName);
}