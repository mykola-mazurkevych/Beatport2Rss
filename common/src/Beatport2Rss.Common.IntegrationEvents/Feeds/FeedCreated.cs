namespace Beatport2Rss.Common.IntegrationEvents.Feeds;

public sealed record FeedCreated(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid FeedId,
    Guid UserId,
    string Name,
    string Slug,
    string? AuthorName,
    string Status) :
    IIntegrationEvent;