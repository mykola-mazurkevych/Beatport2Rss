namespace Beatport2Rss.Common.IntegrationEvents.V1.Feeds;

public sealed record FeedUpdatedV1(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid FeedId,
    string Name,
    string Slug,
    string? AuthorName,
    string Status) :
    IIntegrationEvent;