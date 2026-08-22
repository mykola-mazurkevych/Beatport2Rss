namespace Beatport2Rss.Common.IntegrationEvents.V1;

public sealed record FeedSubscriptionDeletedV1(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid FeedId,
    Guid SubscriptionId) :
    IIntegrationEvent;