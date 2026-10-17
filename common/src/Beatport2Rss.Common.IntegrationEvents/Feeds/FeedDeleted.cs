namespace Beatport2Rss.Common.IntegrationEvents.Feeds;

public sealed record FeedDeleted(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid FeedId) :
    IIntegrationEvent;