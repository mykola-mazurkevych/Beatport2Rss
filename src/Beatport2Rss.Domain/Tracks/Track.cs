using Beatport2Rss.Domain.Common.Interfaces;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Releases;

namespace Beatport2Rss.Domain.Tracks;

public sealed class Track : IEntity<TrackId>
{
    private Track()
    {
    }

    public TrackId Id { get; private set; }

    public BeatportId BeatportId { get; private set; }
    public BeatportSlug BeatportSlug { get; private set; }

    public int Number { get; private set; }

    public string Artist { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string MixName { get; private set; } = null!;

    public TimeSpan Length { get; private set; }

    public Uri SampleUri { get; private set; } = null!;

    public DateTimeOffset CreatedDate { get; private set; }

    public ReleaseId ReleaseId { get; private set; }

    public static Track Create(
        TrackId id,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        int number,
        string artist,
        string name,
        string mixName,
        TimeSpan length,
        Uri sampleUri,
        DateTimeOffset createdDate,
        ReleaseId releaseId) =>
        new()
        {
            Id = id,
            BeatportId = beatportId,
            BeatportSlug = beatportSlug,
            Number = number,
            Artist = artist,
            Name = name,
            MixName = mixName,
            Length = length,
            SampleUri = sampleUri,
            CreatedDate = createdDate,
            ReleaseId = releaseId,
        };
}
