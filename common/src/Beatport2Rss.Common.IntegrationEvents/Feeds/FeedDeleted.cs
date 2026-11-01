namespace Beatport2Rss.Common.IntegrationEvents.Feeds;

[IntegrationEventTopic("feed.deleted")]
public sealed record FeedDeleted(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid FeedId) :
    IIntegrationEvent;