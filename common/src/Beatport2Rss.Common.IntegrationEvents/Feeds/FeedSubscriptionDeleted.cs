namespace Beatport2Rss.Common.IntegrationEvents.Feeds;

[IntegrationEventTopic("feed.subscription.deleted")]
public sealed record FeedSubscriptionDeleted(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid FeedId,
    Guid SubscriptionId) :
    IIntegrationEvent;