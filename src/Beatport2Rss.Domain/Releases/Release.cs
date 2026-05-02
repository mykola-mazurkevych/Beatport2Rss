// ReSharper disable PropertyCanBeMadeInitOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tracks;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Domain.Releases;

public sealed class Release :
    IAggregateRoot<ReleaseId>
{
    private readonly HashSet<ReleaseSubscription> _subscriptions = [];
    private readonly HashSet<Track> _tracks = [];

    private Release()
    {
    }

    public ReleaseId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public BeatportId BeatportId { get; private set; }
    public BeatportSlug BeatportSlug { get; private set; }

    public ReleaseName Name { get; private set; }
    public CatelogNumber CatalogNumber { get; private set; }

    public Uri ImageUri { get; private set; } = null!;

    public DateOnly ReleaseDate { get; private set; }

    public int TracksCount { get; private set; }

    public ReleaseStatus Status { get; private set; }

    public IReadOnlyCollection<ReleaseSubscription> Subscriptions => _subscriptions.AsReadOnly();
    public IReadOnlyCollection<Track> Tracks => _tracks.AsReadOnly();

    public static Release Create(
        DateTimeOffset createdAt,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        ReleaseName name,
        CatelogNumber catalogNumber,
        Uri imageUri,
        DateOnly releaseDate,
        int tracksCount,
        ReleaseStatus status) =>
        new()
        {
            CreatedAt = createdAt,
            BeatportId = beatportId,
            BeatportSlug = beatportSlug,
            Name = name,
            CatalogNumber = catalogNumber,
            ImageUri = imageUri,
            ReleaseDate = releaseDate,
            TracksCount = tracksCount,
            Status = status,
        };
}