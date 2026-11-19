using Beatport2Rss.Builder.Domain.Tracks;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Genres;

namespace Beatport2Rss.Builder.Application.Dtos;

public sealed record TrackDto(
    BeatportId BeatportId,
    int Number,
    TrackName Name,
    MixName MixName,
    IReadOnlyCollection<BeatportId> ArtistBeatportIds,
    IReadOnlyCollection<BeatportId> RemixerBeatportIds,
    GenreId GenreId,
    TimeSpan Length,
    Uri SampleUri);