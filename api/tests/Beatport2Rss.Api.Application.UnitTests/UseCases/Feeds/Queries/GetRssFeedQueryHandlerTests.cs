using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Services.Feeds;
using Beatport2Rss.Api.Application.UseCases.Feeds.Queries;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.FluentResults.Constants;
using Beatport2Rss.Common.FluentResults.Errors;
using Beatport2Rss.Common.FluentResults.Extensions;

using Moq;

using Xunit;

namespace Beatport2Rss.Api.Application.UnitTests.UseCases.Feeds.Queries;

public sealed class GetRssFeedQueryHandlerTests
{
    private static readonly DateTimeOffset GeneratedAt = new(2026, 11, 17, 12, 0, 0, 123, TimeSpan.Zero);

    [Fact]
    public async Task Handle_WhenFeedDoesNotExist_ShouldNotCallBuilder()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var slug = Slug.Create("my-feed");
        var feedQueryRepository = new Mock<IFeedQueryRepository>(MockBehavior.Strict);
        feedQueryRepository.Setup(repository => repository.ExistsAsync(slug, cancellationToken))
            .ReturnsAsync(false);
        var rssFeedDocumentClient = new Mock<IRssFeedDocumentClient>(MockBehavior.Strict);

        var result = await new GetRssFeedQueryHandler(feedQueryRepository.Object, rssFeedDocumentClient.Object)
            .Handle(new GetRssFeedQuery(slug, null), cancellationToken);

        Assert.True(result.IsFailed);
        Assert.IsType<NotFoundError>(Assert.Single(result.Errors));
        rssFeedDocumentClient.Verify(
            client => client.GetGeneratedAtAsync(It.IsAny<Slug>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenIfModifiedSinceMatches_ShouldNotFetchContent()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var slug = Slug.Create("my-feed");
        var ifModifiedSince = DateTimeOffset.FromUnixTimeSeconds(GeneratedAt.ToUnixTimeSeconds());
        var feedQueryRepository = new Mock<IFeedQueryRepository>(MockBehavior.Strict);
        feedQueryRepository.Setup(repository => repository.ExistsAsync(slug, cancellationToken))
            .ReturnsAsync(true);
        var rssFeedDocumentClient = new Mock<IRssFeedDocumentClient>(MockBehavior.Strict);
        rssFeedDocumentClient.Setup(client => client.GetGeneratedAtAsync(slug, cancellationToken))
            .ReturnsAsync(GeneratedAt);

        var result = await new GetRssFeedQueryHandler(feedQueryRepository.Object, rssFeedDocumentClient.Object)
            .Handle(new GetRssFeedQuery(slug, ifModifiedSince), cancellationToken);

        Assert.True(result.IsNotModified);
        Assert.True(result.TryGetSuccessMetadata(MetadataKeys.LastModified, out var lastModifiedAsObject));
        Assert.Equal(ifModifiedSince, Assert.IsType<DateTimeOffset>(lastModifiedAsObject));
        rssFeedDocumentClient.Verify(
            client => client.GetContentAsync(It.IsAny<Slug>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenIfModifiedSinceIsOlder_ShouldNotBeNotModified()
    {
        const string content = "<rss />";
        var cancellationToken = TestContext.Current.CancellationToken;
        var slug = Slug.Create("my-feed");
        var ifModifiedSince = DateTimeOffset.FromUnixTimeSeconds(GeneratedAt.ToUnixTimeSeconds() - 1);
        var feedQueryRepository = new Mock<IFeedQueryRepository>(MockBehavior.Strict);
        feedQueryRepository.Setup(repository => repository.ExistsAsync(slug, cancellationToken))
            .ReturnsAsync(true);
        var rssFeedDocumentClient = new Mock<IRssFeedDocumentClient>(MockBehavior.Strict);
        rssFeedDocumentClient.Setup(client => client.GetGeneratedAtAsync(slug, cancellationToken))
            .ReturnsAsync(GeneratedAt);
        rssFeedDocumentClient.Setup(client => client.GetContentAsync(slug, cancellationToken))
            .ReturnsAsync(content);

        var result = await new GetRssFeedQueryHandler(feedQueryRepository.Object, rssFeedDocumentClient.Object)
            .Handle(new GetRssFeedQuery(slug, ifModifiedSince), cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsNotModified);
        Assert.Equal(content, result.Value.Content);
    }

    [Fact]
    public async Task Handle_WhenIfModifiedSinceIsMissing_ShouldNotBeNotModified()
    {
        const string content = "<rss />";
        var cancellationToken = TestContext.Current.CancellationToken;
        var slug = Slug.Create("my-feed");
        var feedQueryRepository = new Mock<IFeedQueryRepository>(MockBehavior.Strict);
        feedQueryRepository.Setup(repository => repository.ExistsAsync(slug, cancellationToken))
            .ReturnsAsync(true);
        var rssFeedDocumentClient = new Mock<IRssFeedDocumentClient>(MockBehavior.Strict);
        rssFeedDocumentClient.Setup(client => client.GetGeneratedAtAsync(slug, cancellationToken))
            .ReturnsAsync(GeneratedAt);
        rssFeedDocumentClient.Setup(client => client.GetContentAsync(slug, cancellationToken))
            .ReturnsAsync(content);

        var result = await new GetRssFeedQueryHandler(feedQueryRepository.Object, rssFeedDocumentClient.Object)
            .Handle(new GetRssFeedQuery(slug, null), cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsNotModified);
        Assert.Equal(content, result.Value.Content);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(GeneratedAt.ToUnixTimeSeconds()), result.Value.LastModified);
    }
}
