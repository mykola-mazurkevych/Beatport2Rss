using Beatport2Rss.Builder.Application.Dtos;
using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.UseCases.Feeds.Queries;
using Beatport2Rss.Common.Domain.Feeds;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using Wolverine;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Feeds.Queries;

public sealed class GetRssFeedDocumentQueryHandlerTests
{
    private static readonly DateTimeOffset GeneratedAt = new(2026, 11, 17, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task WithoutCachedVersion_ShouldReturnContent()
    {
        var token = TestContext.Current.CancellationToken;
        var feedId = FeedId.Create(Guid.NewGuid());
        var repository = new Mock<IRssFeedDocumentQueryRepository>(MockBehavior.Strict);
        repository.Setup(value => value.FindContentAsync(feedId, token)).ReturnsAsync((GeneratedAt, "<rss />"));

        var result = await new GetRssFeedDocumentQueryHandler(repository.Object)
            .Handle(new GetRssFeedDocumentQuery(feedId.Value, null), token);

        Assert.Equal(RssFeedDocumentDto.Found(GeneratedAt, "<rss />"), result);
    }

    [Fact]
    public async Task WithMatchingCachedVersion_ShouldReturnNotModifiedWithoutLoadingContent()
    {
        var token = TestContext.Current.CancellationToken;
        var feedId = FeedId.Create(Guid.NewGuid());
        var repository = new Mock<IRssFeedDocumentQueryRepository>(MockBehavior.Strict);
        repository.Setup(value => value.FindGeneratedAtAsync(feedId, token)).ReturnsAsync(GeneratedAt);

        var result = await new GetRssFeedDocumentQueryHandler(repository.Object)
            .Handle(new GetRssFeedDocumentQuery(feedId.Value, GeneratedAt), token);

        Assert.Equal(RssFeedDocumentDto.NotModified(GeneratedAt), result);
    }

    [Fact]
    public async Task WithStaleCachedVersion_ShouldReturnContent()
    {
        var token = TestContext.Current.CancellationToken;
        var feedId = FeedId.Create(Guid.NewGuid());
        var repository = new Mock<IRssFeedDocumentQueryRepository>(MockBehavior.Strict);
        repository.Setup(value => value.FindGeneratedAtAsync(feedId, token)).ReturnsAsync(GeneratedAt);
        repository.Setup(value => value.FindContentAsync(feedId, token)).ReturnsAsync((GeneratedAt, "<rss />"));

        var result = await new GetRssFeedDocumentQueryHandler(repository.Object)
            .Handle(new GetRssFeedDocumentQuery(feedId.Value, GeneratedAt.AddMinutes(-5)), token);

        Assert.Equal(RssFeedDocumentDto.Found(GeneratedAt, "<rss />"), result);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task MissingDocument_ShouldReturnNotFound(bool withCachedVersion)
    {
        var token = TestContext.Current.CancellationToken;
        var feedId = FeedId.Create(Guid.NewGuid());
        var repository = new Mock<IRssFeedDocumentQueryRepository>(MockBehavior.Strict);
        repository.Setup(value => value.FindGeneratedAtAsync(feedId, token)).ReturnsAsync((DateTimeOffset?)null);
        repository.Setup(value => value.FindContentAsync(feedId, token))
            .ReturnsAsync(((DateTimeOffset GeneratedAt, string Content)?)null);

        var result = await new GetRssFeedDocumentQueryHandler(repository.Object)
            .Handle(new GetRssFeedDocumentQuery(feedId.Value, withCachedVersion ? GeneratedAt : null), token);

        Assert.Equal(RssFeedDocumentDto.NotFound(), result);
    }

    [Fact]
    public async Task InvokedThroughWolverine_ShouldReturnHandlerResponse()
    {
        var token = TestContext.Current.CancellationToken;
        var feedId = FeedId.Create(Guid.NewGuid());
        var repository = new Mock<IRssFeedDocumentQueryRepository>(MockBehavior.Strict);
        repository.Setup(value => value.FindContentAsync(feedId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GeneratedAt, "<rss />"));
        using var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton(repository.Object);
                services.AddWolverine(options =>
                    options.Discovery.IncludeAssembly(typeof(GetRssFeedDocumentQuery).Assembly));
            })
            .Build();
        await host.StartAsync(token);

        var result = await host.Services.GetRequiredService<IMessageBus>()
            .InvokeAsync<RssFeedDocumentDto>(new GetRssFeedDocumentQuery(feedId.Value, null), token);
        await host.StopAsync(token);

        Assert.Equal(RssFeedDocumentDto.Found(GeneratedAt, "<rss />"), result);
    }
}
