namespace Beatport2Rss.Common.IntegrationEvents.V1.Feeds;

public sealed record FeedCreatedV1(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid FeedId,
    Guid UserId,
    string Name,
    string Slug,
    string? AuthorName,
    string Status) :
    IIntegrationEvent;