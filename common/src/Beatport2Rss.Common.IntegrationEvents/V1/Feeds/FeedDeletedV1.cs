namespace Beatport2Rss.Common.IntegrationEvents.V1.Feeds;

public sealed record FeedDeletedV1(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid FeedId) :
    IIntegrationEvent;