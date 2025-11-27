using Beatport2Rss.SharedKernel;
using Beatport2Rss.Domain.Tracks;

namespace Beatport2Rss.Domain.Releases;

public sealed class Release : IAggregateRoot<ReleaseId>
{
    private readonly HashSet<Track> _tracks = [];

    private Release()
    {
    }

    public ReleaseId Id { get; private set; }

    public long BeatportId { get; private set; }
    public string BeatportSlug { get; private set; } = null!;

    public string Artist { get; private set; } = null!;
    public string Name { get; private set; } = null!;

    public string Label { get; private set; } = null!;
    public string CatalogNumber { get; private set; } = null!;

    public Uri ImageUri { get; private set; } = null!;

    public DateOnly ReleaseDate { get; private set; }

    public int TracksCount { get; private set; }

    public DateTimeOffset CreatedDate { get; private set; }

    public ReleaseStatus Status { get; private set; }

    public IReadOnlySet<Track> Tracks => _tracks.AsReadOnly();

    public static Release Create(
        ReleaseId id,
        long beatportId,
        string beatportSlug,
        string artist,
        string name,
        string label,
        string catalogNumber,
        Uri imageUri,
        DateOnly releaseDate,
        int tracksCount,
        DateTimeOffset createdDate,
        ReleaseStatus status) =>
        new()
        {
            Id = id,
            BeatportId = beatportId,
            BeatportSlug = beatportSlug,
            Artist = artist,
            Name = name,
            Label = label,
            CatalogNumber = catalogNumber,
            ImageUri = imageUri,
            ReleaseDate = releaseDate,
            TracksCount = tracksCount,
            CreatedDate = createdDate,
            Status = status,
        };

    public void UpdateStatus(ReleaseStatus status) => Status = status;

    public void AddTrack(Track track) => _tracks.Add(track);
    public void RemoveTrack(Track track) => _tracks.Remove(track);
    public Track? GetTrack(TrackId trackId) => _tracks.FirstOrDefault(t => t.Id.Equals(trackId));
}