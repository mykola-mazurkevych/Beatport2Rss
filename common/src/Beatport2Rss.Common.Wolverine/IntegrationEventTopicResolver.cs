using System.Reflection;

using Beatport2Rss.Common.IntegrationEvents;

namespace Beatport2Rss.Common.Wolverine;

internal static class IntegrationEventTopicResolver
{
    public static string GetTopicName<TIntegrationEvent>()
        where TIntegrationEvent : IIntegrationEvent =>
        typeof(TIntegrationEvent).GetCustomAttribute<IntegrationEventTopicAttribute>()?.TopicName ??
        throw new InvalidOperationException(
            $"Integration event '{typeof(TIntegrationEvent).FullName}' must have an {nameof(IntegrationEventTopicAttribute)}.");
}
