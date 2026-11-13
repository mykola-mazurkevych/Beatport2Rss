namespace Beatport2Rss.Common.IntegrationEvents.Releases;

[IntegrationEventTopic("release.collected")]
public sealed record ReleaseCollected(
    Guid Id,
    DateTimeOffset OccurredAt,
    Guid ReleaseId,
    int BeatportId,
    string BeatportSlug,
    IReadOnlyCollection<int> ArtistBeatportIds,
    IReadOnlyCollection<int> RemixerBeatportIds,
    int LabelBeatportId,
    string CatalogNumber,
    string Name,
    string? Description,
    DateOnly ReleaseDate,
    Uri ImageUri,
    int TracksCount,
    IReadOnlyCollection<ReleaseCollectedTrack> Tracks) :
    IIntegrationEvent;

public sealed record ReleaseCollectedTrack(
    int BeatportId,
    int Number,
    string Name,
    string MixName,
    IReadOnlyCollection<int> ArtistBeatportIds,
    IReadOnlyCollection<int> RemixerBeatportIds,
    TimeSpan Length,
    Uri SampleUri);