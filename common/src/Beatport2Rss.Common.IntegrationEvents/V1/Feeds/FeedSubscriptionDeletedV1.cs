namespace Beatport2Rss.Common.IntegrationEvents.V1.Feeds;

public sealed record FeedSubscriptionDeletedV1(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid FeedId,
    Guid SubscriptionId) :
    IIntegrationEvent;