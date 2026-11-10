using Beatport2Rss.Common.Beatport.Models;

using FluentResults;

namespace Beatport2Rss.Common.Beatport.Interfaces;

public interface IBeatportClient
{
    Task<Result<TBeatportDto?>> GetAsync<TBeatportDto>(
        int beatportId,
        CancellationToken cancellationToken = default)
        where TBeatportDto : BeatportDto;
}