using Beatport2Rss.Common.Domain.Feeds;

namespace Beatport2Rss.Common.IntegrationEvents.Feeds;

[IntegrationEventTopic("feed.updated")]
public sealed record FeedUpdated(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid FeedId,
    string Name,
    string Slug,
    string? AuthorName,
    FeedStatus Status) :
    IIntegrationEvent;