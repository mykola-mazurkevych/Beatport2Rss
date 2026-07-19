using Beatport2Rss.Builder.Domain.Common.ValueObjects;
using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Builder.Domain.Tracks;

public sealed class Track :
    IEntity<TrackId>
{
    private readonly HashSet<TrackArtist> _artists = [];

    private Track()
    {
    }

    private Track(IEnumerable<TrackArtist> subscriptions) =>
        _artists = [.. subscriptions];

    public TrackId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public BeatportId BeatportId { get; private set; }
    public Uri BeatportUri { get; private set; } = null!;

    public int Number { get; private set; }

    public TrackName Name { get; private set; }
    public MixName MixName { get; private set; }

    public TimeSpan Length { get; private set; }

    public Uri SampleUri { get; private set; } = null!;

    public ReleaseId ReleaseId { get; private set; }

    public IReadOnlyCollection<TrackArtist> Artists => _artists.AsReadOnly();

    public static Track Create(
        TrackId id,
        DateTimeOffset createdAt,
        ReleaseId releaseId,
        BeatportId beatportId,
        Uri beatportUri,
        int number,
        TrackName name,
        MixName mixName,
        TimeSpan length,
        Uri sampleUri,
        IEnumerable<TrackArtist> artists) =>
        new(artists)
        {
            Id = id,
            CreatedAt = createdAt,
            ReleaseId = releaseId,
            BeatportId = beatportId,
            BeatportUri = beatportUri,
            Number = number,
            Name = name,
            MixName = mixName,
            Length = length,
            SampleUri = sampleUri,
        };
}
