using Beatport2Rss.ReleaseCollector.Domain.Common.ValueObjects;
using Beatport2Rss.ReleaseCollector.Domain.Labels;
using Beatport2Rss.ReleaseCollector.Domain.Tracks;
using Beatport2Rss.SharedKernel.Interfaces;

namespace Beatport2Rss.ReleaseCollector.Domain.Releases;

public sealed class Release :
    IAggregateRoot<ReleaseId>
{
    private readonly HashSet<ReleaseArtist> _artists = [];
    private readonly HashSet<Track> _tracks = [];

    private Release()
    {
    }

    private Release(
        IEnumerable<ReleaseArtist> artists,
        IEnumerable<Track> tracks)
    {
        _artists = [.. artists];
        _tracks = [.. tracks];
    }

    public ReleaseId Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public BeatportId BeatportId { get; private set; }
    public BeatportSlug BeatportSlug { get; private set; }

    public ReleaseName Name { get; private set; }
    public DateOnly ReleaseDate { get; private set; }

    public LabelId LabelId { get; private set; }
    public CatalogNumber CatalogNumber { get; private set; }

    public Uri ImageUri { get; private set; } = null!;

    public IReadOnlyCollection<ReleaseArtist> Artists => _artists.AsReadOnly();
    public IReadOnlyCollection<Track> Tracks => _tracks.AsReadOnly();

    public static Release Create(
        ReleaseId id,
        DateTimeOffset createdAt,
        BeatportId beatportId,
        BeatportSlug beatportSlug,
        ReleaseName name,
        DateOnly releaseDate,
        LabelId labelId,
        CatalogNumber catalogNumber,
        Uri imageUri,
        IEnumerable<ReleaseArtist> artists,
        IEnumerable<Track> tracks) =>
        new(artists, tracks)
        {
            Id = id,
            CreatedAt = createdAt,
            BeatportId = beatportId,
            BeatportSlug = beatportSlug,
            Name = name,
            ReleaseDate = releaseDate,
            LabelId = labelId,
            CatalogNumber = catalogNumber,
            ImageUri = imageUri,
        };
}