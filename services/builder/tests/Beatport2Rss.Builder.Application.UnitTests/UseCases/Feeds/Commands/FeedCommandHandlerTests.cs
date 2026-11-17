using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Builder.Domain.Feeds;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Feeds.Commands;

public sealed class FeedCommandHandlerTests
{
    [Theory]
    [InlineData("Author")]
    [InlineData(null)]
    public async Task CreateFeedCommandHandler_ShouldAddAndSaveFeed(string? authorName)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var feedId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var repository = new Mock<IFeedCommandRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        Feed? addedFeed = null;
        repository.Setup(value => value.ExistsAsync(FeedId.Create(feedId), cancellationToken))
            .ReturnsAsync(false);
        repository.Setup(value => value.AddAsync(It.IsAny<Feed>(), cancellationToken))
            .Callback<Feed, CancellationToken>((feed, _) => addedFeed = feed)
            .ReturnsAsync((Feed feed, CancellationToken _) => feed);
        unitOfWork.Setup(value => value.SaveChangesAsync(cancellationToken)).Returns(Task.CompletedTask);
        var handler = new CreateFeedCommandHandler(
            repository.Object,
            unitOfWork.Object,
            NullLogger<CreateFeedCommand>.Instance);

        await handler.Handle(
            new CreateFeedCommand(feedId, userId, createdAt, "Feed", "feed", authorName, FeedStatus.Active),
            cancellationToken);

        Assert.NotNull(addedFeed);
        Assert.Equal(FeedId.Create(feedId), addedFeed.Id);
        Assert.Equal(UserId.Create(userId), addedFeed.UserId);
        Assert.Equal(createdAt, addedFeed.CreatedAt);
        Assert.Equal(FeedName.Create("Feed"), addedFeed.Name);
        Assert.Equal(Slug.Create("feed"), addedFeed.Slug);
        if (authorName is null)
        {
            Assert.Null(addedFeed.AuthorName);
        }
        else
        {
            Assert.Equal(AuthorName.Create(authorName), addedFeed.AuthorName);
        }
        Assert.Equal(FeedStatus.Active, addedFeed.Status);
        unitOfWork.Verify(value => value.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateFeedCommandHandler_WhenFeedAlreadyExists_ShouldNotAddOrSave()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IFeedCommandRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        repository.Setup(value => value.ExistsAsync(It.IsAny<FeedId>(), cancellationToken))
            .ReturnsAsync(true);
        var handler = new CreateFeedCommandHandler(
            repository.Object,
            unitOfWork.Object,
            NullLogger<CreateFeedCommand>.Instance);

        await handler.Handle(
            new CreateFeedCommand(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow, "Feed", "feed", null, FeedStatus.Active),
            cancellationToken);

        repository.Verify(value => value.AddAsync(It.IsAny<Feed>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteFeedCommandHandler_ShouldDeleteAndSaveFeed()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var feed = CreateFeed();
        var repository = new Mock<IFeedCommandRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        repository.Setup(value => value.FindAsync(feed.Id, cancellationToken)).ReturnsAsync(feed);
        repository.Setup(value => value.Delete(feed));
        unitOfWork.Setup(value => value.SaveChangesAsync(cancellationToken)).Returns(Task.CompletedTask);
        var handler = new DeleteFeedCommandHandler(repository.Object, unitOfWork.Object);

        await handler.Handle(new DeleteFeedCommand(feed.Id.Value), cancellationToken);

        repository.Verify(value => value.Delete(feed), Times.Once);
        unitOfWork.Verify(value => value.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteFeedCommandHandler_WhenFeedIsMissing_ShouldNotSave()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IFeedCommandRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        repository.Setup(value => value.FindAsync(It.IsAny<FeedId>(), cancellationToken))
            .ReturnsAsync((Feed?)null);
        var handler = new DeleteFeedCommandHandler(repository.Object, unitOfWork.Object);

        await handler.Handle(new DeleteFeedCommand(Guid.NewGuid()), cancellationToken);

        repository.Verify(value => value.Delete(It.IsAny<Feed>()), Times.Never);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateFeedCommandHandler_ShouldUpdateAndSaveFeed()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var feed = CreateFeed();
        var repository = new Mock<IFeedCommandRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        repository.Setup(value => value.FindAsync(feed.Id, cancellationToken)).ReturnsAsync(feed);
        unitOfWork.Setup(value => value.SaveChangesAsync(cancellationToken)).Returns(Task.CompletedTask);
        var handler = new UpdateFeedCommandHandler(repository.Object, unitOfWork.Object);

        await handler.Handle(
            new UpdateFeedCommand(feed.Id.Value, "Updated", "updated", null, FeedStatus.Inactive),
            cancellationToken);

        Assert.Equal(FeedName.Create("Updated"), feed.Name);
        Assert.Equal(Slug.Create("updated"), feed.Slug);
        Assert.Null(feed.AuthorName);
        Assert.Equal(FeedStatus.Inactive, feed.Status);
        unitOfWork.Verify(value => value.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateFeedCommandHandler_ShouldSetAuthorName()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var feed = CreateFeed();
        var repository = new Mock<IFeedCommandRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        repository.Setup(value => value.FindAsync(feed.Id, cancellationToken)).ReturnsAsync(feed);
        unitOfWork.Setup(value => value.SaveChangesAsync(cancellationToken)).Returns(Task.CompletedTask);
        var handler = new UpdateFeedCommandHandler(repository.Object, unitOfWork.Object);

        await handler.Handle(
            new UpdateFeedCommand(feed.Id.Value, "Updated", "updated", "New Author", FeedStatus.Active),
            cancellationToken);

        Assert.Equal(AuthorName.Create("New Author"), feed.AuthorName);
        unitOfWork.Verify(value => value.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateFeedCommandHandler_WhenFeedIsMissing_ShouldNotSave()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<IFeedCommandRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        repository.Setup(value => value.FindAsync(It.IsAny<FeedId>(), cancellationToken))
            .ReturnsAsync((Feed?)null);
        var handler = new UpdateFeedCommandHandler(repository.Object, unitOfWork.Object);

        await handler.Handle(
            new UpdateFeedCommand(Guid.NewGuid(), "Updated", "updated", "Author", FeedStatus.Inactive),
            cancellationToken);

        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Feed CreateFeed() =>
        Feed.Create(
            FeedId.Create(Guid.NewGuid()),
            DateTimeOffset.UtcNow,
            UserId.Create(Guid.NewGuid()),
            FeedName.Create("Feed"),
            Slug.Create("feed"),
            AuthorName.Create("Original Author"),
            FeedStatus.Active);
}