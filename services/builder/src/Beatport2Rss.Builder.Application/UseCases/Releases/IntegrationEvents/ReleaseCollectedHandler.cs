using Beatport2Rss.Builder.Application.Dtos;
using Beatport2Rss.Builder.Application.UseCases.Releases.Commands;
using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Builder.Domain.Tracks;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Releases;
using Beatport2Rss.Common.IntegrationEvents.Releases;

namespace Beatport2Rss.Builder.Application.UseCases.Releases.IntegrationEvents;

public static class ReleaseCollectedHandler
{
    public static CreateReleaseCommand Handle(ReleaseCollected releaseCollected) =>
        new(ReleaseId.Create(releaseCollected.ReleaseId),
            releaseCollected.OccurredAt,
            BeatportId.Create(releaseCollected.BeatportId),
            BeatportSlug.Create(releaseCollected.BeatportSlug),
            [.. releaseCollected.ArtistBeatportIds.Select(BeatportId.Create)],
            [.. releaseCollected.RemixerBeatportIds.Select(BeatportId.Create)],
            BeatportId.Create(releaseCollected.LabelBeatportId),
            ReleaseName.Create(releaseCollected.Name),
            releaseCollected.Description,
            releaseCollected.ReleaseDate,
            CatalogNumber.Create(releaseCollected.CatalogNumber),
            releaseCollected.ImageUri,
            releaseCollected.TracksCount,
            [
                .. releaseCollected.Tracks
                    .Select(track =>
                        new TrackDto(BeatportId.Create(track.BeatportId),
                            track.Number,
                            TrackName.Create(track.Name),
                            MixName.Create(track.MixName),
                            [.. track.ArtistBeatportIds.Select(BeatportId.Create)],
                            [.. track.RemixerBeatportIds.Select(BeatportId.Create)],
                            track.Length,
                            track.SampleUri))
            ]);
}