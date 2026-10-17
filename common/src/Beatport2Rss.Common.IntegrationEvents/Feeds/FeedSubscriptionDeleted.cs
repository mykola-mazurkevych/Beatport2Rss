namespace Beatport2Rss.Common.IntegrationEvents.Feeds;

public sealed record FeedSubscriptionDeleted(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid FeedId,
    Guid SubscriptionId) :
    IIntegrationEvent;