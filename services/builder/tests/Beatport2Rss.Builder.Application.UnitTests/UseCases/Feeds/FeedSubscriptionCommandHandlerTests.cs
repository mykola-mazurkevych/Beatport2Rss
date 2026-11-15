using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Builder.Domain.Feeds;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Miscellaneous.Exceptions;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Feeds;

public sealed class FeedSubscriptionCommandHandlerTests
{
    [Fact]
    public async Task CreateFeedSubscriptionCommandHandler_ShouldAddAndSaveSubscription()
    {
        var feedId = FeedId.Create(Guid.NewGuid());
        var subscriptionId = SubscriptionId.Create(Guid.NewGuid());
        var feed = CreateFeed(feedId);
        var feedRepository = new Mock<IFeedCommandRepository>();
        var subscriptionRepository = new Mock<ISubscriptionCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        feedRepository
            .Setup(value => value.FindAsync(feedId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feed);
        subscriptionRepository
            .Setup(value => value.ExistsAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateFeedSubscriptionCommandHandler(
            feedRepository.Object,
            subscriptionRepository.Object,
            unitOfWork.Object,
            Mock.Of<ILogger<CreateFeedSubscriptionCommandHandler>>());

        await handler.HandleAsync(
            new CreateFeedSubscriptionCommand(feedId.Value, subscriptionId.Value),
            CancellationToken.None);

        Assert.True(feed.HasSubscription(subscriptionId));
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateFeedSubscriptionCommandHandler_WhenFeedIsMissing_ShouldThrowRetryableException()
    {
        var feedRepository = new Mock<IFeedCommandRepository>();
        var subscriptionRepository = new Mock<ISubscriptionCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new CreateFeedSubscriptionCommandHandler(
            feedRepository.Object,
            subscriptionRepository.Object,
            unitOfWork.Object,
            Mock.Of<ILogger<CreateFeedSubscriptionCommandHandler>>());

        await Assert.ThrowsAsync<RetryableException>(() => handler.HandleAsync(
            new CreateFeedSubscriptionCommand(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None));

        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateFeedSubscriptionCommandHandler_WhenSubscriptionIsMissing_ShouldThrowRetryableException()
    {
        var feedId = FeedId.Create(Guid.NewGuid());
        var feed = CreateFeed(feedId);
        var feedRepository = new Mock<IFeedCommandRepository>();
        var subscriptionRepository = new Mock<ISubscriptionCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        feedRepository
            .Setup(value => value.FindAsync(feedId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feed);

        var handler = new CreateFeedSubscriptionCommandHandler(
            feedRepository.Object,
            subscriptionRepository.Object,
            unitOfWork.Object,
            Mock.Of<ILogger<CreateFeedSubscriptionCommandHandler>>());

        await Assert.ThrowsAsync<RetryableException>(() => handler.HandleAsync(
            new CreateFeedSubscriptionCommand(feedId.Value, Guid.NewGuid()),
            CancellationToken.None));

        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteFeedSubscriptionCommandHandler_ShouldRemoveAndSaveSubscription()
    {
        var feedId = FeedId.Create(Guid.NewGuid());
        var subscriptionId = SubscriptionId.Create(Guid.NewGuid());
        var feed = CreateFeed(feedId);
        feed.AddSubscription(subscriptionId);
        var feedRepository = new Mock<IFeedCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        feedRepository
            .Setup(value => value.FindAsync(feedId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feed);

        await DeleteFeedSubscriptionCommandHandler.HandleAsync(
            new DeleteFeedSubscriptionCommand(feedId.Value, subscriptionId.Value),
            feedRepository.Object,
            unitOfWork.Object,
            CancellationToken.None);

        Assert.False(feed.HasSubscription(subscriptionId));
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteFeedSubscriptionCommandHandler_WhenSubscriptionIsNotLinked_ShouldNotSave()
    {
        var feedId = FeedId.Create(Guid.NewGuid());
        var feedRepository = new Mock<IFeedCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        feedRepository
            .Setup(value => value.FindAsync(feedId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateFeed(feedId));

        await DeleteFeedSubscriptionCommandHandler.HandleAsync(
            new DeleteFeedSubscriptionCommand(feedId.Value, Guid.NewGuid()),
            feedRepository.Object,
            unitOfWork.Object,
            CancellationToken.None);

        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Feed CreateFeed(FeedId feedId) =>
        Feed.Create(
            feedId,
            DateTimeOffset.UtcNow,
            FeedName.Create("Feed"),
            Slug.Create("feed"),
            null,
            FeedStatus.Active);
}