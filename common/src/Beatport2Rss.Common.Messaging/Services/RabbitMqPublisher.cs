using System.Collections.Concurrent;
using System.Text.Json;

using Beatport2Rss.Common.Messaging.Interfaces;
using Beatport2Rss.Common.Messaging.Options;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace Beatport2Rss.Common.Messaging.Services;

internal sealed class RabbitMqPublisher(
    IRabbitMqConnectionFactory connectionFactory,
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

    public Task PublishAsync<TMessage>(
        TMessage message,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        cancellationToken.ThrowIfCancellationRequested();

        var messageTypeName = typeof(TMessage).Name;

        if (!_queueOptions.RoutingKeys.TryGetValue(messageTypeName, out var routingKey))
        {
            throw new InvalidOperationException($"No routing key configured for message type '{messageTypeName}'.");
        }

        using var model = _connection.Value.CreateModel();

        DeclareExchange(model, _queueOptions.ExchangeName);

        var properties = model.CreateBasicProperties();
        properties.Persistent = true;
        properties.Type = messageTypeName;

        var body = JsonSerializer.SerializeToUtf8Bytes(message, _jsonSerializerOptions);
        model.BasicPublish(_queueOptions.ExchangeName, routingKey, mandatory: false, properties, body);

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

    private void DeclareExchange(IModel model, string exchangeName)
    {
        if (!_declaredExchanges.TryAdd(exchangeName, 0))
        {
            return;
        }

        try
        {
            RabbitMqTopology.DeclareExchange(model, exchangeName);
        }
        catch
        {
            _declaredExchanges.TryRemove(exchangeName, out _);
            throw;
        }
    }
}