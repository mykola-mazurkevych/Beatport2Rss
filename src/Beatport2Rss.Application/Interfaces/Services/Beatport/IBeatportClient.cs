using Beatport2Rss.Application.Dtos.Beatport;
using Beatport2Rss.Domain.Common.ValueObjects;

using FluentResults;

namespace Beatport2Rss.Application.Interfaces.Services.Beatport;

public interface IBeatportClient
{
    Task<Result<TBeatportDto?>> GetAsync<TBeatportDto>(
        BeatportId beatportId,
        string accessToken,
        CancellationToken cancellationToken = default)
        where TBeatportDto : BeatportDto;
}