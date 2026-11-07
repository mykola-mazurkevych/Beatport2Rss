using Beatport2Rss.Builder.Application.Dtos;
using Beatport2Rss.Builder.Application.UseCases.Releases.Commands;
using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Builder.Domain.Tracks;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Releases;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.IntegrationEvents.Releases;

namespace Beatport2Rss.Builder.Application.UseCases.Releases.IntegrationEvents;

public static class ReleaseCollectedHandler
{
    public static CreateReleaseCommand Handle(ReleaseCollected releaseCollected) =>
        new(ReleaseId.Create(releaseCollected.ReleaseId),
            releaseCollected.OccurredAt,
            BeatportId.Create(releaseCollected.BeatportId),
            BeatportSlug.Create(releaseCollected.BeatportSlug),
            [.. releaseCollected.Artists.Select(MapArtist)],
            [.. releaseCollected.Remixers.Select(MapArtist)],
            new LabelDto(
                BeatportId.Create(releaseCollected.Label.BeatportId),
                SubscriptionName.Create(releaseCollected.Label.Name)),
            ReleaseName.Create(releaseCollected.ReleaseName),
            releaseCollected.ReleaseDate,
            CatalogNumber.Create(releaseCollected.CatalogNumber),
            releaseCollected.ImageUri,
            releaseCollected.TracksCount,
            [.. releaseCollected.Tracks.Select(MapTrack)]);

    private static ArtistDto MapArtist(ReleaseCollectedArtist artist) =>
        new(BeatportId.Create(artist.BeatportId),
            SubscriptionName.Create(artist.Name));

    private static TrackDto MapTrack(ReleaseCollectedTrack track) =>
        new(BeatportId.Create(track.BeatportId),
            track.Number,
            TrackName.Create(track.Name),
            MixName.Create(track.MixName),
            [.. track.Artists.Select(MapArtist)],
            [.. track.Remixers.Select(MapArtist)],
            track.Length,
            track.SampleUri);
}