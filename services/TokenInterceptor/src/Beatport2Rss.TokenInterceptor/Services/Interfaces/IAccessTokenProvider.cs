namespace Beatport2Rss.TokenInterceptor.Services.Interfaces;

internal interface IAccessTokenProvider
{
    Task<string> ProvideAsync(CancellationToken cancellationToken = default);
}