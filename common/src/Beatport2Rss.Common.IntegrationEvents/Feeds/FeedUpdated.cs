namespace Beatport2Rss.Common.IntegrationEvents.Feeds;

public sealed record FeedUpdated(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid FeedId,
    string Name,
    string Slug,
    string? AuthorName,
    string Status) :
    IIntegrationEvent;