namespace Beatport2Rss.Common.IntegrationEvents.Subscriptions;

[IntegrationEventTopic("subscription.tag.deleted")]
public sealed record SubscriptionTagDeleted(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid SubscriptionId,
    Guid TagId) :
    IIntegrationEvent;