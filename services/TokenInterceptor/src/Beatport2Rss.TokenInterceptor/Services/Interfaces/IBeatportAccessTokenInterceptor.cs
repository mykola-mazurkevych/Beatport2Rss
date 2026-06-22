namespace Beatport2Rss.TokenInterceptor.Services.Interfaces;

internal interface IBeatportAccessTokenInterceptor
{
    Task<(string AccessToken, int ExpiresIn)> InterceptAsync(
        bool headless,
        CancellationToken cancellationToken = default);
}