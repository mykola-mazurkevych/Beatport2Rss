using RabbitMQ.Client;

namespace Beatport2Rss.Common.Messaging.Services;

public static class RabbitMqTopology
{
    public static void DeclareQueueWithDeadLetter(IModel model, string queueName, string deadLetterSuffix)
    {
        var deadLetterQueueName = $"{queueName}-{deadLetterSuffix}";
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
}
