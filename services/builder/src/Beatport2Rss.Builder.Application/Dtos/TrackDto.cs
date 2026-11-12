using Beatport2Rss.Builder.Domain.Tracks;
using Beatport2Rss.Common.Domain.Beatport;

namespace Beatport2Rss.Builder.Application.Dtos;

public sealed record TrackDto(
    BeatportId BeatportId,
    int Number,
    TrackName Name,
    MixName MixName,
    IReadOnlyCollection<BeatportId> ArtistBeatportIds,
    IReadOnlyCollection<BeatportId> RemixerBeatportIds,
    TimeSpan Length,
    Uri SampleUri);