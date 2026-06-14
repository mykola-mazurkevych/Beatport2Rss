// ReSharper disable PropertyCanBeMadeInitOnly.Local
// ReSharper disable UnusedMember.Local

using Beatport2Rss.ReleaseCollector.Domain.Common;
using Beatport2Rss.ReleaseCollector.Domain.Releases;
using Beatport2Rss.SharedKernel.Interfaces;

namespace Beatport2Rss.ReleaseCollector.Domain.Tracks;

public sealed class Track :
    IEntity<TrackId>
{
    private readonly HashSet<TrackSubscription> _subscriptions = [];

    private Track()
    {
    }

    private Track(IEnumerable<TrackSubscription> subscriptions) =>
        _subscriptions = [.. subscriptions];

    public TrackId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public BeatportId BeatportId { get; private set; }
    public BeatportSlug BeatportSlug { get; private set; }

    public int Number { get; private set; }

    public TrackName Name { get; private set; }
    public MixName MixName { get; private set; }

    public TimeSpan Length { get; private set; }

    public Uri SampleUri { get; private set; } = null!;

    public ReleaseId ReleaseId { get; private set; }

    public IReadOnlyCollection<TrackSubscription> Subscriptions => _subscriptions.AsReadOnly();

    public static Track Create(
        TrackId id,
        DateTimeOffset createdAt,
        ReleaseId releaseId,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        int number,
        TrackName name,
        MixName mixName,
        TimeSpan length,
        Uri sampleUri,
        IEnumerable<TrackSubscription> subscriptions) =>
        new(subscriptions)
        {
            Id = id,
            CreatedAt = createdAt,
            ReleaseId = releaseId,
            BeatportId = beatportId,
            BeatportSlug = beatportSlug,
            Number = number,
            Name = name,
            MixName = mixName,
            Length = length,
            SampleUri = sampleUri,
        };
}
