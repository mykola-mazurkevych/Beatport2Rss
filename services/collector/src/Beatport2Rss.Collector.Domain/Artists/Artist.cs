using Beatport2Rss.Collector.Domain.Common.Subscriptions;
using Beatport2Rss.Collector.Domain.Common.ValueObjects;

namespace Beatport2Rss.Collector.Domain.Artists;

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
        BeatportSlug beatportSlug,
        int subscribersCount,
        DateTimeOffset? refreshedAt) :
        base(id,
            createdAt,
            name,
            beatportId,
            beatportSlug,
            subscribersCount,
            refreshedAt)
    {
    }

    public static Artist Create(
        ArtistId id,
        DateTimeOffset createdAt,
        ArtistName name,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        int subscribersCount,
        DateTimeOffset? refreshedAt) =>
        new(id,
            createdAt,
            name,
            beatportId,
            beatportSlug,
            subscribersCount,
            refreshedAt);
}