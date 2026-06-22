using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.TokenInterceptor.Services.Interfaces;

using Microsoft.Extensions.Caching.Memory;

using Polly.Registry;

namespace Beatport2Rss.TokenInterceptor.Services;

internal sealed class AccessTokenProvider(
    IMemoryCache cache,
    ResiliencePipelineProvider<string> pipelineProvider,
    IBeatportAccessTokenInterceptor interceptor) :
    IAccessTokenProvider, IDisposable, IAsyncDisposable
{
    public const string ResiliencePipelineKey = nameof(AccessTokenProvider);

    private const string CacheKey = "AccessToken";

    private readonly SemaphoreSlim _lock = new(1, 1);
    private bool _disposed;

    public async Task<string> ProvideAsync(CancellationToken cancellationToken = default)
    {
        if (TryGetFromCache(out var accessToken))
        {
            return accessToken;
        }

        await _lock.WaitAsync(cancellationToken);

        try
        {
            if (TryGetFromCache(out accessToken))
            {
                return accessToken;
            }

            var pipeline = pipelineProvider.GetPipeline(ResiliencePipelineKey);
            (accessToken, int expiresIn) = await pipeline.ExecuteAsync(
                async ct => await interceptor.InterceptAsync(headless: true, ct),
                cancellationToken);

            cache.Set(CacheKey, accessToken, TimeSpan.FromSeconds(expiresIn - 5));

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
        // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    private bool TryGetFromCache([NotNullWhen(true)] out string? accessToken)
    {
        if (cache.TryGetValue(CacheKey, out var value) &&
            value is string stringValue)
        {
            accessToken = stringValue;
            return true;
        }

        accessToken = null;
        return false;
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

            _disposed = true;
        }
    }
}