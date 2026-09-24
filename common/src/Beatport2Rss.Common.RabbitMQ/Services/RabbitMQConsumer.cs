using System.Text.Json;

using Beatport2Rss.Common.RabbitMQ.Extensions;
using Beatport2Rss.Common.RabbitMQ.Interfaces;
using Beatport2Rss.Common.RabbitMQ.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Beatport2Rss.Common.RabbitMQ.Services;

internal sealed class RabbitMQConsumer<TMessage>(
    IRabbitMQConnectionFactory connectionFactory,
    IOptions<QueueOptions> queueOptions,
    IOptions<JsonSerializerOptions> jsonSerializerOptions,
    ILogger<RabbitMQConsumer<TMessage>> logger) :
    IConsumer<TMessage>, IDisposable, IAsyncDisposable
    where TMessage : class
{
    private readonly QueueOptions _queueOptions = queueOptions.Value;
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions.Value;

    private IConnection? _connection;
    private IModel? _model;
    private bool _disposed;

    public Task ConsumeAsync(
        Func<TMessage, CancellationToken, Task> consume,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var messageTypeName = typeof(TMessage).Name;
        var queue = _queueOptions.Queues[messageTypeName];

        _connection = connectionFactory.CreateConnection();
        _model = _connection.CreateModel();

        _model.DeclareExchange(_queueOptions.ExchangeName);
        _model.DeclareQueueWithBinding<TMessage>(_queueOptions);

        _model.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(_model);
        consumer.Received += async (_, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var message =
                    JsonSerializer.Deserialize<TMessage>(body, _jsonSerializerOptions) ??
                    throw new InvalidOperationException($"Received a null message payload for '{typeof(TMessage).Name}'.");

                await consume(message, cancellationToken);

                _model.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                RabbitMQConsumerLogMessages.LogFailedToConsumeMessage(logger, exception, queue);
                _model.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
            }
        };

        _model.BasicConsume(queue, autoAck: false, consumer);

        return Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
    }

    public void Dispose() =>
        Dispose(disposing: true);

    public ValueTask DisposeAsync()
    {
        Dispose(disposing: true);
        return ValueTask.CompletedTask;
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _model?.Dispose();
            _connection?.Dispose();
        }

        _disposed = true;
    }
}

internal static partial class RabbitMQConsumerLogMessages
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to consume RabbitMQ message from queue {QueueName}")]
    public static partial void LogFailedToConsumeMessage(ILogger logger, Exception exception, string queueName);
}