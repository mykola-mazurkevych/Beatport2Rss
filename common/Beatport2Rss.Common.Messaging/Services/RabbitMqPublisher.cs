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
    IPublisher, IDisposable
{
    private readonly QueueOptions _queueOptions = queueOptions.Value;
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions.Value;
    private readonly Lazy<IConnection> _connection = new(connectionFactory.CreateConnection);

    public Task PublishAsync<TMessage>(
        TMessage message,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var messageTypeName = typeof(TMessage).Name;

        if (!_queueOptions.Queues.TryGetValue(messageTypeName, out var queueName))
        {
            throw new InvalidOperationException($"No queue configured for message type '{messageTypeName}'.");
        }

        var deadLetterQueueName = $"{queueName}-{_queueOptions.DeadLetterSuffix}";

        using var model = _connection.Value.CreateModel();

        model.QueueDeclare(
            deadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        model.QueueDeclare(
            queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object>
            {
                ["x-dead-letter-exchange"] = string.Empty,
                ["x-dead-letter-routing-key"] = deadLetterQueueName,
            });

        var properties = model.CreateBasicProperties();
        properties.Persistent = true;

        var body = JsonSerializer.SerializeToUtf8Bytes(message, _jsonSerializerOptions);
        model.BasicPublish(string.Empty, queueName, properties, body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_connection.IsValueCreated)
        {
            _connection.Value.Dispose();
        }
    }
}
