using Beatport2Rss.Builder.Domain.Common.Subscriptions;
using Beatport2Rss.Builder.Domain.Common.ValueObjects;

namespace Beatport2Rss.Builder.Domain.Artists;

public sealed class Artist :
    Subscription<ArtistId, ArtistName>
{
    private Artist()
    {
    }

    private Artist(
        ArtistId id,
        DateTimeOffset createdAt,
        ArtistName name,
        BeatportId beatportId,
        Uri beatportUri) :
        base(id,
            createdAt,
            name,
            beatportId,
            beatportUri)
    {
    }

    public static Artist Create(
        ArtistId id,
        DateTimeOffset createdAt,
        ArtistName name,
        BeatportId beatportId,
        Uri beatportUri) =>
        new(id,
            createdAt,
            name,
            beatportId,
            beatportUri);
}