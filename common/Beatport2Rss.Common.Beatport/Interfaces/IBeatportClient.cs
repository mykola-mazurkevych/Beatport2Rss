using Beatport2Rss.Common.Beatport.Models;

using FluentResults;

namespace Beatport2Rss.Common.Beatport.Interfaces;

public interface IBeatportClient
{
    Task<Result<TBeatportDto?>> GetAsync<TBeatportDto>(
        int beatportId,
        string accessToken,
        CancellationToken cancellationToken = default)
        where TBeatportDto : BeatportDto;
}