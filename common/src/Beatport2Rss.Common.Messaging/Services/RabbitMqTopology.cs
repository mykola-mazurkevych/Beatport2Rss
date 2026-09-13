using RabbitMQ.Client;

namespace Beatport2Rss.Common.Messaging.Services;

internal static class RabbitMqTopology
{
    public static void DeclareExchange(IModel model, string exchange) =>
        model.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);

    public static void DeclareQueueWithBinding(IModel model, string exchangeName, string queue, string routingKey, string deadLetterSuffix)
    {
        var deadLetterQueue = $"{queue}-{deadLetterSuffix}";
        model.QueueDeclare(deadLetterQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var arguments = new Dictionary<string, object>
        {
            ["x-dead-letter-exchange"] = string.Empty,
            ["x-dead-letter-routing-key"] = deadLetterQueue,
        };

        model.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments);
        model.QueueBind(queue, exchangeName, routingKey, arguments: null);
    }
}
