namespace Beatport2Rss.TokenProvider.Services.Interfaces;

internal interface IAccessTokenProvider
{
    Task<string> ProvideAsync(CancellationToken cancellationToken = default);
}