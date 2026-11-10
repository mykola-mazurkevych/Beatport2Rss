using FluentResults;

namespace Beatport2Rss.Common.Beatport.Interfaces;

public interface IBeatportTokenProvider
{
    Task<Result<string>> ProvideAsync(CancellationToken cancellationToken = default);
}