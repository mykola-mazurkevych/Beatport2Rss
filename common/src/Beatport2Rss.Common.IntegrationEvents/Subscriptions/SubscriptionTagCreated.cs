namespace Beatport2Rss.Common.IntegrationEvents.Subscriptions;

[IntegrationEventTopic("subscription.tag.created")]
public sealed record SubscriptionTagCreated(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid SubscriptionId,
    Guid TagId) :
    IIntegrationEvent;
