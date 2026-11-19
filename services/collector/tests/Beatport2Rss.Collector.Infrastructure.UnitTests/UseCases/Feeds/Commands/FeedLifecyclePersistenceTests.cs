using Beatport2Rss.Collector.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Collector.Application.UseCases.Feeds.IntegrationEvents;
using Beatport2Rss.Collector.Application.UseCases.Users.Commands;
using Beatport2Rss.Collector.Application.UseCases.Users.IntegrationEvents;
using Beatport2Rss.Collector.Domain.Feeds;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Collector.Domain.Users;
using Beatport2Rss.Collector.Infrastructure.Persistence;
using Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents.Feeds;
using Beatport2Rss.Common.IntegrationEvents.Users;
using Beatport2Rss.Common.Miscellaneous.Exceptions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

namespace Beatport2Rss.Collector.Infrastructure.UnitTests.UseCases.Feeds.Commands;

public sealed class FeedLifecyclePersistenceTests
{
    [Fact]
    public async Task CreatingFeedBeforeUser_ShouldRetryThenPersistLifecycleIdempotently()
    {
        var token = TestContext.Current.CancellationToken;
        var options = new DbContextOptionsBuilder<CollectorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new CollectorDbContext(options);
        var users = new UserCommandRepository(context.Users);
        var feeds = new FeedCommandRepository(context.Feeds);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(value => value.SaveChangesAsync(token)).Returns<CancellationToken>(context.SaveChangesAsync);
        var userId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var command = FeedCreatedHandler.Handle(new FeedCreated(
            Guid.NewGuid(), createdAt, Guid.NewGuid(), userId, "Feed", "feed", null,
            FeedStatus.Active));
        var createFeed = new CreateFeedCommandHandler(feeds, users, unitOfWork.Object);

        await Assert.ThrowsAsync<RetryableException>(() => createFeed.HandleAsync(command, token));
        Assert.Empty(await context.Feeds.ToListAsync(token));
        var createUser = new CreateUserCommandHandler(users, unitOfWork.Object);
        var userCommand = UserCreatedHandler.Handle(new UserCreated(Guid.NewGuid(), createdAt, userId));
        await createUser.HandleAsync(userCommand, token);
        await createUser.HandleAsync(userCommand, token);
        await createFeed.HandleAsync(command, token);
        await createFeed.HandleAsync(command, token);

        var savedFeed = await context.Feeds.SingleAsync(token);
        Assert.Equal(UserId.Create(userId), savedFeed.UserId);
        Assert.Equal(createdAt, savedFeed.CreatedAt);
        Assert.Equal(createdAt, (await context.Users.SingleAsync(token)).CreatedAt);
        var deleteFeed = new DeleteFeedCommandHandler(feeds, unitOfWork.Object);
        await deleteFeed.HandleAsync(FeedDeletedHandler.Handle(
            new FeedDeleted(Guid.NewGuid(), DateTimeOffset.UtcNow, command.FeedId)), token);
        var deleteUser = new DeleteUserCommandHandler(users, unitOfWork.Object);
        await deleteUser.HandleAsync(UserDeletedHandler.Handle(
            new UserDeleted(Guid.NewGuid(), DateTimeOffset.UtcNow, userId)), token);
        Assert.Empty(await context.Feeds.ToListAsync(token));
        Assert.Empty(await context.Users.ToListAsync(token));
        unitOfWork.Verify(value => value.SaveChangesAsync(token), Times.Exactly(4));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DeletingOwnerOrFeed_ShouldStopCollectionOnlyForOrphanedSubscriptions(bool deleteUser)
    {
        var token = TestContext.Current.CancellationToken;
        var options = new DbContextOptionsBuilder<CollectorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new CollectorDbContext(options);
        var user = User.Create(UserId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow);
        var otherUser = User.Create(UserId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow);
        var feed = Feed.Create(FeedId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow, user.Id);
        var otherFeed = Feed.Create(FeedId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow, otherUser.Id);
        var shared = Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow,
            SubscriptionType.Artist, BeatportId.Create(42));
        var orphaned = Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow,
            SubscriptionType.Artist, BeatportId.Create(43));
        feed.AddSubscription(shared.Id);
        feed.AddSubscription(orphaned.Id);
        otherFeed.AddSubscription(shared.Id);
        context.Users.AddRange(user, otherUser);
        context.Feeds.AddRange(feed, otherFeed);
        context.Subscriptions.AddRange(shared, orphaned);
        await context.SaveChangesAsync(token);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(value => value.SaveChangesAsync(token)).Returns<CancellationToken>(context.SaveChangesAsync);

        if (deleteUser)
        {
            var handler = new DeleteUserCommandHandler(new UserCommandRepository(context.Users), unitOfWork.Object);
            await handler.HandleAsync(new DeleteUserCommand(user.Id.Value), token);
            await handler.HandleAsync(new DeleteUserCommand(user.Id.Value), token);
        }
        else
        {
            var handler = new DeleteFeedCommandHandler(new FeedCommandRepository(context.Feeds), unitOfWork.Object);
            await handler.HandleAsync(new DeleteFeedCommand(feed.Id.Value), token);
            await handler.HandleAsync(new DeleteFeedCommand(feed.Id.Value), token);
        }

        var repository = new SubscriptionCommandRepository(context.Feeds, context.Subscriptions, new TestClock());
        Assert.Equal([shared.Id], await repository.FindAllToRefreshAsync(token));
        Assert.Equal(2, await context.Subscriptions.CountAsync(token));
        Assert.Equal(otherFeed.Id, (await context.Feeds.SingleAsync(token)).Id);
        Assert.Equal(shared.Id, await context.Feeds.SelectMany(value => value.Subscriptions)
            .Select(link => link.SubscriptionId).SingleAsync(token));
        unitOfWork.Verify(value => value.SaveChangesAsync(token), Times.Once);
    }

    private sealed class TestClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
        public DateOnly UtcToday => DateOnly.FromDateTime(UtcNow.UtcDateTime);
    }
}
