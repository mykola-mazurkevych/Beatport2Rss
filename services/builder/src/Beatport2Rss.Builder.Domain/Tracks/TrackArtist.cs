using Beatport2Rss.Builder.Domain.Common;
using Beatport2Rss.Common.Domain.Interfaces;
using Beatport2Rss.Common.Domain.Subscriptions;

namespace Beatport2Rss.Builder.Domain.Tracks;

public sealed record TrackArtist :
    IValueObject
{
    private TrackArtist()
    {
    }

    public TrackId TrackId { get; private set; }
    public SubscriptionId ArtistId { get; private set; }
    public ArtistType Type { get; private set; }

    public static TrackArtist Create(
        TrackId trackId,
        SubscriptionId artistId,
        ArtistType type) =>
        new()
        {
            TrackId = trackId,
            ArtistId = artistId,
            Type = type,
        };
}