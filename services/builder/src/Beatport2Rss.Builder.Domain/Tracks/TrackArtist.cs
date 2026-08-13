using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Builder.Domain.Tracks;

public sealed record TrackArtist :
    IValueObject
{
    private TrackArtist()
    {
    }

    public TrackId TrackId { get; private set; }
    public SubscriptionId ArtistId { get; private set; }
    public TrackArtistType Type { get; private set; }

    public static TrackArtist Create(
        TrackId trackId,
        SubscriptionId artistId,
        TrackArtistType type) =>
        new()
        {
            TrackId = trackId,
            ArtistId = artistId,
            Type = type,
        };
}