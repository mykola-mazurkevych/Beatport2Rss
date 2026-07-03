using System.Collections.Concurrent;
using System.Text.Json;

using Beatport2Rss.Common.Messaging.Interfaces;
using Beatport2Rss.Common.Messaging.Options;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace Beatport2Rss.Common.Messaging.Services;

internal sealed class RabbitMqPublisher(
    IConnectionFactory connectionFactory,
    IOptions<QueueOptions> queueOptions,
    IOptions<JsonSerializerOptions> jsonSerializerOptions) :
    IPublisher, IDisposable, IAsyncDisposable
{
    private readonly Lazy<IConnection> _connection = new(
        connectionFactory.CreateConnection,
        LazyThreadSafetyMode.ExecutionAndPublication);

    private readonly QueueOptions _queueOptions = queueOptions.Value;
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions.Value;

    private readonly ConcurrentDictionary<string, byte> _declaredQueues = [];
    private bool _disposed;

    public Task PublishAsync<TMessage>(
        TMessage message,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        cancellationToken.ThrowIfCancellationRequested();

        var messageTypeName = typeof(TMessage).Name;

        if (!_queueOptions.Queues.TryGetValue(messageTypeName, out var queueName))
        {
            throw new InvalidOperationException($"No queue configured for message type '{messageTypeName}'.");
        }

        using var model = _connection.Value.CreateModel();

        DeclareQueues(model, queueName);

        var properties = model.CreateBasicProperties();
        properties.Persistent = true;

        var body = JsonSerializer.SerializeToUtf8Bytes(message, _jsonSerializerOptions);
        model.BasicPublish(string.Empty, queueName, mandatory: false, properties, body);

        return Task.CompletedTask;
    }

    public void Dispose() =>
        Dispose(true);

    public ValueTask DisposeAsync()
    {
        Dispose(true);
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
            if (_connection.IsValueCreated)
            {
                _connection.Value.Dispose();
            }
        }

        _disposed = true;
    }

    private void DeclareQueues(IModel model, string queueName)
    {
        if (!_declaredQueues.TryAdd(queueName, 0))
        {
            return;
        }

        try
        {
            var deadLetterQueueName = $"{queueName}-{_queueOptions.DeadLetterSuffix}";
            model.QueueDeclare(
                deadLetterQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var arguments = new Dictionary<string, object>
            {
                ["x-dead-letter-exchange"] = string.Empty,
                ["x-dead-letter-routing-key"] = deadLetterQueueName,
            };

            model.QueueDeclare(
                queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments);
        }
        catch
        {
            _declaredQueues.TryRemove(queueName, out _);
            throw;
        }
    }
}