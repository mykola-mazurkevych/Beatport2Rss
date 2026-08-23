namespace Beatport2Rss.Common.IntegrationEvents.V1.Feeds;

public sealed record FeedSubscriptionCreatedV1(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid FeedId,
    Guid SubscriptionId) :
    IIntegrationEvent;