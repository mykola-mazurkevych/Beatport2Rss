namespace Beatport2Rss.TokenProvider.Services.Interfaces;

internal interface IBeatportAccessTokenInterceptor
{
    Task<(string AccessToken, int ExpiresIn)> InterceptAsync(
        bool headless,
        CancellationToken cancellationToken = default);
}