namespace Beatport2Rss.Interceptor.Interfaces;

internal interface IAccessTokenProvider
{
    Task<string> ProvideAsync(CancellationToken cancellationToken = default);
}