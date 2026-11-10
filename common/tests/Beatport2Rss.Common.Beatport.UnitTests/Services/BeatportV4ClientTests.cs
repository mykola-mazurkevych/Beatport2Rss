using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Models;
using Beatport2Rss.Common.FluentResults.Errors;

using FluentResults;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace Beatport2Rss.Common.Beatport.UnitTests.Services;

public sealed class BeatportV4ClientTests
{
    [Fact]
    public async Task GetAsync_WhenTokenProvided_ShouldAuthenticateEachRequest()
    {
        using var cancellationSource = new CancellationTokenSource();
        var cancellationToken = cancellationSource.Token;
        var receivedTokens = new List<CancellationToken>();
        var requestCalls = 0;
        var label = new BeatportLabelDto(42, "label", "Label", new BeatportImage(new Uri("https://example.com/image.jpg")));
        var tokenProvider = new StubTokenProvider(token =>
        {
            receivedTokens.Add(token);
            return Task.FromResult(Result.Ok($"token-{receivedTokens.Count}"));
        });
        using var handler = new StubHttpMessageHandler((request, _) =>
        {
            requestCalls++;
            Assert.Equal("Bearer", request.Headers.Authorization?.Scheme);
            Assert.Equal($"token-{requestCalls}", request.Headers.Authorization?.Parameter);
            Assert.Equal(new Uri("https://example.com/v4/labels/42/"), request.RequestUri);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(label) });
        });
        using var provider = CreateProvider(tokenProvider, handler);
        var client = provider.GetRequiredService<IBeatportClient>();

        var firstResult = await client.GetAsync<BeatportLabelDto>(42, cancellationToken);
        var secondResult = await client.GetAsync<BeatportLabelDto>(42, cancellationToken);

        Assert.True(firstResult.IsSuccess);
        Assert.Equal(label, firstResult.Value);
        Assert.True(secondResult.IsSuccess);
        Assert.Equal(label, secondResult.Value);
        Assert.Equal(new[] { cancellationToken, cancellationToken }, receivedTokens);
        Assert.Equal(2, requestCalls);
    }

    [Fact]
    public async Task GetAsync_WhenTokenFails_ShouldPreserveErrorsWithoutSendingRequest()
    {
        var errors = new IError[]
        {
            new TransientError("Token service unavailable"),
            new TransientHttpError("Token service error", HttpStatusCode.ServiceUnavailable),
            new RequestTimeoutError("Token service timed out"),
            new TooManyRequestsError("Token service rate limited"),
            new Error("Token service failed")
        };
        var tokenProvider = new StubTokenProvider(_ => Task.FromResult(Result.Fail<string>(errors)));
        using var handler = new StubHttpMessageHandler((_, _) =>
            throw new InvalidOperationException("No HTTP request should be sent when token acquisition fails."));
        using var provider = CreateProvider(tokenProvider, handler);
        var client = provider.GetRequiredService<IBeatportClient>();

        var result = await client.GetAsync<BeatportLabelDto>(42, TestContext.Current.CancellationToken);

        Assert.True(result.IsFailed);
        Assert.Equal(errors.Length, result.Errors.Count);
        for (var index = 0; index < errors.Length; index++)
        {
            Assert.Same(errors[index], result.Errors[index]);
        }
    }

    [Fact]
    public async Task GetAsync_WhenTokenAcquisitionCanceled_ShouldPropagateCancellation()
    {
        using var cancellationSource = new CancellationTokenSource();
        await cancellationSource.CancelAsync();
        var cancellationToken = cancellationSource.Token;
        var tokenProvider = new StubTokenProvider(token =>
        {
            Assert.Equal(cancellationToken, token);
            return Task.FromCanceled<Result<string>>(token);
        });
        using var handler = new StubHttpMessageHandler((_, _) =>
            throw new InvalidOperationException("No HTTP request should be sent after cancellation."));
        using var provider = CreateProvider(tokenProvider, handler);
        var client = provider.GetRequiredService<IBeatportClient>();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.GetAsync<BeatportLabelDto>(42, cancellationToken));
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, typeof(UnauthorizedError))]
    [InlineData(HttpStatusCode.Forbidden, typeof(ForbiddenError))]
    [InlineData(HttpStatusCode.NotFound, typeof(NotFoundError))]
    [InlineData(HttpStatusCode.RequestTimeout, typeof(RequestTimeoutError))]
    [InlineData(HttpStatusCode.TooManyRequests, typeof(TooManyRequestsError))]
    [InlineData(HttpStatusCode.ServiceUnavailable, typeof(TransientHttpError))]
    public async Task GetAsync_WhenHttpRequestFails_ShouldPreserveHttpErrorMapping(HttpStatusCode statusCode, Type errorType)
    {
        var tokenProvider = new StubTokenProvider(_ => Task.FromResult(Result.Ok("token")));
        using var handler = new StubHttpMessageHandler((_, _) => Task.FromResult(new HttpResponseMessage(statusCode)
        {
            Content = JsonContent.Create(new { Detail = "Request failed" })
        }));
        using var provider = CreateProvider(tokenProvider, handler);
        var client = provider.GetRequiredService<IBeatportClient>();

        var result = await client.GetAsync<BeatportLabelDto>(42, TestContext.Current.CancellationToken);

        Assert.True(result.IsFailed);
        var error = Assert.Single(result.Errors);
        Assert.IsType(errorType, error);
        Assert.Contains("Request failed", error.Message, StringComparison.Ordinal);
    }

    private static ServiceProvider CreateProvider(IBeatportTokenProvider tokenProvider, HttpMessageHandler handler)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["BeatportOptions:ApiV4BaseUri"] = "https://example.com/v4/",
            ["BeatportOptions:WebBaseUri"] = "https://example.com/"
        }).Build();
        var services = new ServiceCollection();
        services.Configure<JsonSerializerOptions>(options => options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
        services.AddSingleton(tokenProvider);
        services.AddBeatportClient(configuration);
        services.AddHttpClient(nameof(IBeatportClient)).ConfigurePrimaryHttpMessageHandler(() => handler);
        return services.BuildServiceProvider();
    }

    private sealed class StubTokenProvider(Func<CancellationToken, Task<Result<string>>> provide) : IBeatportTokenProvider
    {
        public Task<Result<string>> ProvideAsync(CancellationToken cancellationToken = default) =>
            provide(cancellationToken);
    }

    private sealed class StubHttpMessageHandler(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            send(request, cancellationToken);
    }
}
