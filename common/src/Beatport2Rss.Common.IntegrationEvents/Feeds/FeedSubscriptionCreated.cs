namespace Beatport2Rss.Common.IntegrationEvents.Feeds;

public sealed record FeedSubscriptionCreated(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid FeedId,
    Guid SubscriptionId) :
    IIntegrationEvent;