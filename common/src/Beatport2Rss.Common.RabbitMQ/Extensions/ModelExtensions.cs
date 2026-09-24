using Beatport2Rss.Common.RabbitMQ.Options;

using RabbitMQ.Client;

namespace Beatport2Rss.Common.RabbitMQ.Extensions;

internal static class ModelExtensions
{
    extension(IModel model)
    {
        public void DeclareExchange(string exchangeName) =>
            model.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);

        public void DeclareQueueWithBinding<TMessage>(QueueOptions queueOptions)
        {
            var messageTypeName = typeof(TMessage).Name;
            var queue = queueOptions.Queues[messageTypeName];

            var deadLetterQueue = $"{queue}-{queueOptions.DeadLetterSuffix}";
            model.QueueDeclare(deadLetterQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var arguments = new Dictionary<string, object>
            {
                ["x-dead-letter-exchange"] = string.Empty,
                ["x-dead-letter-routing-key"] = deadLetterQueue,
            };

            model.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments);
            model.QueueBind(queue, queueOptions.ExchangeName, queueOptions.RoutingKeys[messageTypeName], arguments: null);
        }
    }
}