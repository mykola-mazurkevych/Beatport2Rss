using Beatport2Rss.Builder.Domain.Common;
using Beatport2Rss.Common.Domain.Interfaces;
using Beatport2Rss.Common.Domain.Releases;
using Beatport2Rss.Common.Domain.Subscriptions;

namespace Beatport2Rss.Builder.Domain.Releases;

public sealed record ReleaseArtist :
    IValueObject
{
    private ReleaseArtist()
    {
    }

    public ReleaseId ReleaseId { get; private set; }
    public SubscriptionId ArtistId { get; private set; }

    public ArtistType Type { get; private set; }

    public static ReleaseArtist Create(
        ReleaseId releaseId,
        SubscriptionId artistId,
        ArtistType type) =>
        new()
        {
            ReleaseId = releaseId,
            ArtistId = artistId,
            Type = type,
        };
}