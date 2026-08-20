namespace Beatport2Rss.Common.IntegrationEvents.V1;

public sealed record FeedDeletedV1(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid FeedId) :
    IIntegrationEvent;