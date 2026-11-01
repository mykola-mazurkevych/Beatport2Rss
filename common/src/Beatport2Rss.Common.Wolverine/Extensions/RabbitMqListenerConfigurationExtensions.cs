#pragma warning disable CA1034 // Nested types should not be visible

using Beatport2Rss.Common.IntegrationEvents;

using Wolverine.RabbitMQ;

namespace Beatport2Rss.Common.Wolverine.Extensions;

public static class RabbitMqListenerConfigurationExtensions
{
    extension(RabbitMqListenerConfiguration listener)
    {
        public RabbitMqListenerConfiguration ListensTo<TIntegrationEvent>()
            where TIntegrationEvent : IIntegrationEvent =>
            listener.ConfigureQueue(queue =>
                queue.BindTopic(IntegrationEventTopicResolver.GetTopicName<TIntegrationEvent>())
                    .ToExchange(RabbitMQTopology.IntegrationEventsExchange));
    }
}