using Beatport2Rss.Builder.Application.UseCases.Releases.IntegrationEvents;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Genres;
using Beatport2Rss.Common.Domain.Releases;
using Beatport2Rss.Common.IntegrationEvents.Releases;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Releases.Commands;

public sealed class ReleaseCollectedHandlerTests
{
    [Fact]
    public void Handle_ShouldMapReleaseAndTrackFieldsToCommand()
    {
        var occurredAt = DateTimeOffset.UtcNow;
        var releaseId = Guid.NewGuid();
        var imageUri = new Uri("https://example.com/release.jpg");
        var sampleUri = new Uri("https://example.com/sample.mp3");
        var integrationEvent = new ReleaseCollected(
            Guid.NewGuid(),
            occurredAt,
            releaseId,
            100,
            "release-slug",
            [101, 102],
            [201],
            301,
            "CAT-1",
            "Release",
            "Description",
            new DateOnly(2026, 11, 15),
            imageUri,
            1,
            [
                new ReleaseCollectedTrack(
                    401,
                    2,
                    "Track",
                    "Club Mix",
                    [501, 502],
                    [601],
                    TimeSpan.FromMinutes(5),
                    sampleUri,
                    Guid.Parse("00000000-0000-0000-0000-000000000701"))
            ]);

        var command = ReleaseCollectedHandler.Handle(integrationEvent);

        Assert.Equal(ReleaseId.Create(releaseId), command.ReleaseId);
        Assert.Equal(occurredAt, command.CreatedAt);
        Assert.Equal(BeatportId.Create(100), command.BeatportId);
        Assert.Equal(BeatportSlug.Create("release-slug"), command.BeatportSlug);
        Assert.Equal([BeatportId.Create(101), BeatportId.Create(102)], command.ArtistBeatportIds);
        Assert.Equal([BeatportId.Create(201)], command.RemixerBeatportIds);
        Assert.Equal(BeatportId.Create(301), command.LabelBeatportId);
        Assert.Equal("Release", command.ReleaseName.Value);
        Assert.Equal("Description", command.Description);
        Assert.Equal(new DateOnly(2026, 11, 15), command.ReleaseDate);
        Assert.Equal("CAT-1", command.CatalogNumber.Value);
        Assert.Equal(imageUri, command.ImageUri);
        Assert.Equal(1, command.TracksCount);

        var track = Assert.Single(command.Tracks);
        Assert.Equal(BeatportId.Create(401), track.BeatportId);
        Assert.Equal(2, track.Number);
        Assert.Equal("Track", track.Name.Value);
        Assert.Equal("Club Mix", track.MixName.Value);
        Assert.Equal([BeatportId.Create(501), BeatportId.Create(502)], track.ArtistBeatportIds);
        Assert.Equal([BeatportId.Create(601)], track.RemixerBeatportIds);
        Assert.Equal(TimeSpan.FromMinutes(5), track.Length);
        Assert.Equal(sampleUri, track.SampleUri);
        Assert.Equal(GenreId.Create(Guid.Parse("00000000-0000-0000-0000-000000000701")), track.GenreId);
    }
}