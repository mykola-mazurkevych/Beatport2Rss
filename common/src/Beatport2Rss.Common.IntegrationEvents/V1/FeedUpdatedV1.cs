namespace Beatport2Rss.Common.IntegrationEvents.V1;

public sealed record FeedUpdatedV1(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid FeedId,
    string Name,
    string Slug,
    string? AuthorName,
    bool IsActive) :
    IIntegrationEvent;