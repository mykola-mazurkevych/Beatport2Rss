using Beatport2Rss.Builder.Domain.Artists;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Builder.Domain.Releases;

public sealed record ReleaseArtist :
    IValueObject
{
    private ReleaseArtist()
    {
    }

    public ReleaseId ReleaseId { get; private set; }
    public ArtistId ArtistId { get; private set; }

    public ReleaseArtistType Type { get; private set; }

    public static ReleaseArtist Create(
        ReleaseId releaseId,
        ArtistId artistId,
        ReleaseArtistType type) =>
        new()
        {
            ReleaseId = releaseId,
            ArtistId = artistId,
            Type = type,
        };
}