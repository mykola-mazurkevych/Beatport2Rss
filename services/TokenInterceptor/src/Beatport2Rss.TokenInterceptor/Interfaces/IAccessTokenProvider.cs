namespace Beatport2Rss.TokenInterceptor.Interfaces;

internal interface IAccessTokenProvider
{
    Task<string> ProvideAsync(CancellationToken cancellationToken = default);
}