namespace Beatport2Rss.Common.IntegrationEvents.V1.Feeds;

public sealed record FeedDeletedV1(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid FeedId) :
    IIntegrationEvent;