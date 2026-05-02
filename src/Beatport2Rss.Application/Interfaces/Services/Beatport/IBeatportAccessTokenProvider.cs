namespace Beatport2Rss.Application.Interfaces.Services.Beatport;

public interface IBeatportAccessTokenProvider
{
    Task<(string? AccessToken, int ExpiresIn)> ProvideAsync(
        bool headless,
        CancellationToken cancellationToken = default);
}