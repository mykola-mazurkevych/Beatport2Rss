namespace Beatport2Rss.Common.IntegrationEvents.Releases;

[IntegrationEventTopic("release.collected")]
public sealed record ReleaseCollected(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid ReleaseId,
    int BeatportId,
    string BeatportSlug,
    IReadOnlyCollection<ReleaseCollectedArtist> Artists,
    IReadOnlyCollection<ReleaseCollectedArtist> Remixers,
    ReleaseCollectedLabel Label,
    string ReleaseName,
    DateOnly ReleaseDate,
    string CatalogNumber,
    Uri ImageUri,
    int TracksCount,
    IReadOnlyCollection<ReleaseCollectedTrack> Tracks) :
    IIntegrationEvent;

public sealed record ReleaseCollectedArtist(
    int BeatportId,
    string Name);

public sealed record ReleaseCollectedLabel(
    int BeatportId,
    string Name);

public sealed record ReleaseCollectedTrack(
    int BeatportId,
    int Number,
    string Name,
    string MixName,
    IReadOnlyCollection<ReleaseCollectedArtist> Artists,
    IReadOnlyCollection<ReleaseCollectedArtist> Remixers,
    TimeSpan Length,
    Uri SampleUri);