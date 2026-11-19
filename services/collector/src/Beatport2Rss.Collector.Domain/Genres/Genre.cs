using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Genres;
using Beatport2Rss.Common.Domain.Interfaces;

namespace Beatport2Rss.Collector.Domain.Genres;

public sealed class Genre :
    IAggregateRoot<GenreId>
{
    private Genre()
    {
    }

    public GenreId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public BeatportId BeatportId { get; private set; }

    public static Genre Create(
        GenreId id,
        DateTimeOffset createdAt,
        BeatportId beatportId) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            BeatportId = beatportId,
        };
}