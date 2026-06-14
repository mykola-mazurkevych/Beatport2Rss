// ReSharper disable PropertyCanBeMadeInitOnly.Local
// ReSharper disable UnusedMember.Local

using Beatport2Rss.ReleaseCollector.Domain.Common.ValueObjects;
using Beatport2Rss.ReleaseCollector.Domain.Tracks;
using Beatport2Rss.SharedKernel.Interfaces;

namespace Beatport2Rss.ReleaseCollector.Domain.Releases;

public sealed class Release :
    IAggregateRoot<ReleaseId>
{
    private readonly HashSet<ReleaseSubscription> _subscriptions = [];
    private readonly HashSet<Track> _tracks = [];

    private Release()
    {
    }

    private Release(IEnumerable<ReleaseSubscription> subscriptions, IEnumerable<Track> tracks) =>
        (_subscriptions, _tracks) = ([.. subscriptions], [.. tracks]);

    public ReleaseId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public BeatportId BeatportId { get; private set; }
    public BeatportSlug BeatportSlug { get; private set; }

    public ReleaseName Name { get; private set; }
    public CatalogNumber CatalogNumber { get; private set; }

    public Uri ImageUri { get; private set; } = null!;

    public DateOnly ReleaseDate { get; private set; }

    public IReadOnlyCollection<ReleaseSubscription> Subscriptions => _subscriptions.AsReadOnly();
    public IReadOnlyCollection<Track> Tracks => _tracks.AsReadOnly();

    public static Release Create(
        ReleaseId id,
        DateTimeOffset createdAt,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        ReleaseName name,
        CatalogNumber catalogNumber,
        Uri imageUri,
        DateOnly releaseDate,
        IEnumerable<ReleaseSubscription> subscriptions,
        IEnumerable<Track> tracks) =>
        new(subscriptions, tracks)
        {
            Id = id,
            CreatedAt = createdAt,
            BeatportId = beatportId,
            BeatportSlug = beatportSlug,
            Name = name,
            CatalogNumber = catalogNumber,
            ImageUri = imageUri,
            ReleaseDate = releaseDate,
        };
}