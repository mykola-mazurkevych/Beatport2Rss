namespace Beatport2Rss.Common.IntegrationEvents.Genres;

[IntegrationEventTopic("genre.created")]
public sealed record GenreCreated(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid GenreId,
    int BeatportId,
    string Name) :
    IIntegrationEvent;
