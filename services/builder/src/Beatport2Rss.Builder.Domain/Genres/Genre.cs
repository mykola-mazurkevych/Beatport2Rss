using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Interfaces;

namespace Beatport2Rss.Builder.Domain.Genres;

public sealed class Genre :
    IAggregateRoot<GenreId>
{
    private Genre()
    {
    }

    public GenreId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public BeatportId BeatportId { get; private set; }

    public string Name { get; private set; } = null!;

    public static Genre Create(
        GenreId id,
        DateTimeOffset createdAt,
        BeatportId beatportId,
        string name) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            BeatportId = beatportId,
            Name = name,
        };

    public void UpdateName(string name) =>
        Name = name;
}
