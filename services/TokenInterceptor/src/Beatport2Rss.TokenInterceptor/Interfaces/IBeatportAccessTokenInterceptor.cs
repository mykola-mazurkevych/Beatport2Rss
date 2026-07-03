namespace Beatport2Rss.TokenInterceptor.Interfaces;

internal interface IBeatportAccessTokenInterceptor
{
    Task<(string AccessToken, int ExpiresIn)> InterceptAsync(
        bool headless,
        CancellationToken cancellationToken = default);
}