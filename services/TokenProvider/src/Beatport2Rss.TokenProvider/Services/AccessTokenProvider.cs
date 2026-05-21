using Beatport2Rss.TokenProvider.Services.Interfaces;

using Microsoft.Extensions.Caching.Memory;

namespace Beatport2Rss.TokenProvider.Services;

internal sealed class AccessTokenProvider(
    IMemoryCache cache,
    IBeatportAccessTokenInterceptor interceptor) :
    IAccessTokenProvider, IDisposable, IAsyncDisposable
{
    private const string CacheKey = "AccessToken";

    private readonly SemaphoreSlim _lock = new(1, 1);
    private bool _disposed;

    public async Task<string> ProvideAsync(CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(CacheKey, out var value) &&
            value is string cachedAccessToken)
        {
            return cachedAccessToken;
        }

        await _lock.WaitAsync(cancellationToken);

        try
        {
            (string? accessToken, int expiresIn) = await interceptor.InterceptAsync(headless: true, cancellationToken);

            if (accessToken is null || expiresIn == 0)
            {
                throw new InvalidOperationException("Failed to acquire access token.");
            }

            cache.Set(CacheKey, accessToken, TimeSpan.FromSeconds(expiresIn));

            return accessToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    public ValueTask DisposeAsync()
    {
        Dispose(true);
        return ValueTask.CompletedTask;
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _lock.Dispose();
        }

        _disposed = true;
    }
}