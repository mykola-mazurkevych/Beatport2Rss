using FluentResults;

namespace Beatport2Rss.Common.BeatportTokenProvider.Services.Interfaces;

public interface IBeatportTokenProvider
{
    Task<Result<string>> ProvideAsync(CancellationToken cancellationToken = default);
}