using System.Collections.Concurrent;
using System.Text.Json;

using Beatport2Rss.Common.RabbitMQ.Extensions;
using Beatport2Rss.Common.RabbitMQ.Interfaces;
using Beatport2Rss.Common.RabbitMQ.Options;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace Beatport2Rss.Common.RabbitMQ.Services;

internal sealed class RabbitMQPublisher(
    IRabbitMQConnectionFactory connectionFactory,
    IOptions<QueueOptions> queueOptions,
    IOptions<JsonSerializerOptions> jsonSerializerOptions) :
    IPublisher, IDisposable, IAsyncDisposable
{
    private readonly Lazy<IConnection> _connection = new(
        connectionFactory.CreateConnection,
        LazyThreadSafetyMode.ExecutionAndPublication);

    private readonly QueueOptions _queueOptions = queueOptions.Value;
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions.Value;

    private readonly ConcurrentDictionary<string, byte> _declaredExchanges = [];

    private bool _disposed;

    public Task PublishAsync(
        object message,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        cancellationToken.ThrowIfCancellationRequested();

        var messageTypeName = message.GetType().Name;

        if (!_queueOptions.RoutingKeys.TryGetValue(messageTypeName, out var routingKey))
        {
            throw new InvalidOperationException($"No routing key configured for message type '{messageTypeName}'.");
        }

        using var model = _connection.Value.CreateModel();

        DeclareExchange(model, _queueOptions.ExchangeName);

        var basicProperties = model.CreateBasicProperties();
        basicProperties.Persistent = true;
        basicProperties.Type = messageTypeName;

        var body = JsonSerializer.SerializeToUtf8Bytes(message, _jsonSerializerOptions);
        model.BasicPublish(_queueOptions.ExchangeName, routingKey, mandatory: false, basicProperties, body);

        return Task.CompletedTask;
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
            if (_connection.IsValueCreated)
            {
                _connection.Value.Dispose();
            }
        }

        _disposed = true;
    }

    private void DeclareExchange(IModel model, string exchangeName)
    {
        if (!_declaredExchanges.TryAdd(exchangeName, 0))
        {
            return;
        }

        try
        {
            model.DeclareExchange(exchangeName);
        }
        catch
        {
            _declaredExchanges.TryRemove(exchangeName, out _);
            throw;
        }
    }
}