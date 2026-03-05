using Beatport2Rss.Application.Dtos.Beatport;
using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tokens;

using FluentResults;

namespace Beatport2Rss.Application.Interfaces.Services.Beatport;

public interface IBeatportClient
{
    Task<Result<TBeatportDto?>> GetAsync<TBeatportDto>(
        BeatportId beatportId,
        BeatportAccessToken beatportAccessToken,
        CancellationToken cancellationToken = default)
        where TBeatportDto : BeatportDto;
}