using Beatport2Rss.Builder.Domain.Artists;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Builder.Domain.Tracks;

public sealed record TrackArtist :
    IValueObject
{
    private TrackArtist()
    {
    }

    public TrackId TrackId { get; private set; }
    public ArtistId ArtistId { get; private set; }
    public TrackArtistType Type { get; private set; }

    public static TrackArtist Create(
        TrackId trackId,
        ArtistId artistId,
        TrackArtistType type) =>
        new()
        {
            TrackId = trackId,
            ArtistId = artistId,
            Type = type,
        };
}