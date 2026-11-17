using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Collector.Domain.Feeds;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Miscellaneous.Exceptions;

using Moq;

using Xunit;

namespace Beatport2Rss.Collector.Application.UnitTests.UseCases.Feeds.Commands;

public sealed class FeedSubscriptionCommandHandlerTests
{
    [Fact]
    public async Task CreatingDuplicateLinkAndDeletingItTwice_ShouldSaveOnlyActualChanges()
    {
        var token = TestContext.Current.CancellationToken;
        var feed = Feed.Create(FeedId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow, UserId.Create(Guid.NewGuid()));
        var subscription = Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow,
            SubscriptionType.Artist, BeatportId.Create(42));
        var feeds = new Mock<IFeedCommandRepository>(MockBehavior.Strict);
        feeds.Setup(value => value.FindAsync(feed.Id, token)).ReturnsAsync(feed);
        var subscriptions = new Mock<ISubscriptionCommandRepository>(MockBehavior.Strict);
        subscriptions.Setup(value => value.ExistsAsync(subscription.Id, token)).ReturnsAsync(true);
        var unitOfWork = new Mock<IUnitOfWork>();
        var create = new CreateFeedSubscriptionCommandHandler(feeds.Object, subscriptions.Object, unitOfWork.Object);
        var delete = new DeleteFeedSubscriptionCommandHandler(feeds.Object, unitOfWork.Object);

        await create.HandleAsync(new CreateFeedSubscriptionCommand(feed.Id.Value, subscription.Id.Value), token);
        await create.HandleAsync(new CreateFeedSubscriptionCommand(feed.Id.Value, subscription.Id.Value), token);
        Assert.Single(feed.Subscriptions);
        await delete.HandleAsync(new DeleteFeedSubscriptionCommand(feed.Id.Value, subscription.Id.Value), token);
        await delete.HandleAsync(new DeleteFeedSubscriptionCommand(feed.Id.Value, subscription.Id.Value), token);

        Assert.Empty(feed.Subscriptions);
        unitOfWork.Verify(value => value.SaveChangesAsync(token), Times.Exactly(2));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CreatingLinkBeforeDependencies_ShouldRetryWithoutSaving(bool missingFeed)
    {
        var token = TestContext.Current.CancellationToken;
        var feed = Feed.Create(FeedId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow, UserId.Create(Guid.NewGuid()));
        var feeds = new Mock<IFeedCommandRepository>();
        feeds.Setup(value => value.FindAsync(feed.Id, token)).ReturnsAsync(missingFeed ? null : feed);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var handler = new CreateFeedSubscriptionCommandHandler(
            feeds.Object, Mock.Of<ISubscriptionCommandRepository>(), unitOfWork.Object);

        await Assert.ThrowsAsync<RetryableException>(() => handler.HandleAsync(
            new CreateFeedSubscriptionCommand(feed.Id.Value, Guid.NewGuid()), token));

        Assert.Empty(feed.Subscriptions);
        Assert.Empty(unitOfWork.Invocations);
    }
}
