using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Collector.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Collector.Application.UseCases.Releases.Commands;
using Beatport2Rss.Collector.Infrastructure.Persistence;
using Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using Xunit;

namespace Beatport2Rss.Collector.Infrastructure.UnitTests.Persistence.Repositories;

public sealed class SubscriptionCommandRepositoryTests
{
    [Theory]
    [InlineData(true, 2)]
    [InlineData(false, 0)]
    public async Task ChangingSubscribersCountPreservesConcurrentRefreshDate(bool increment, int expectedCount)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var options = new DbContextOptionsBuilder<CollectorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var subscriptionId = SubscriptionId.Create(Guid.NewGuid());
        await using var handlerContext = new CollectorDbContext(options);
        var subscription = Subscription.Create(subscriptionId, DateTimeOffset.UtcNow, SubscriptionType.Artist, BeatportId.Create(42));
        subscription.IncrementSubscribersCount();
        handlerContext.Subscriptions.Add(subscription);
        await handlerContext.SaveChangesAsync(cancellationToken);

        var refreshedAt = new DateOnly(2026, 11, 12);
        await using (var collectorContext = new CollectorDbContext(options))
        {
            var collectedSubscription = await collectorContext.Subscriptions.SingleAsync(cancellationToken);
            collectedSubscription.MarkRefreshed(refreshedAt);
            await collectorContext.SaveChangesAsync(cancellationToken);
        }

        var repository = new SubscriptionCommandRepository(handlerContext.Subscriptions, new TestClock());
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns((CancellationToken token) => handlerContext.SaveChangesAsync(token));
        if (increment)
        {
            await IncrementSubscribersCountCommandHandler.HandleAsync(
                new IncrementSubscribersCountCommand(subscriptionId.Value), repository, unitOfWork.Object,
                NullLogger<IncrementSubscribersCountCommand>.Instance, cancellationToken);
        }
        else
        {
            await DecrementSubscribersCountCommandHandler.HandleAsync(
                new DecrementSubscribersCountCommand(subscriptionId.Value), repository, unitOfWork.Object,
                NullLogger<DecrementSubscribersCountCommand>.Instance, cancellationToken);
        }

        await using var verificationContext = new CollectorDbContext(options);
        var savedSubscription = await verificationContext.Subscriptions.SingleAsync(cancellationToken);
        Assert.Equal(expectedCount, savedSubscription.SubscribersCount);
        Assert.Equal(refreshedAt, savedSubscription.RefreshedAt);
    }

    [Fact]
    public async Task QueueReleaseCollectionCommand_QueuesEveryEligibleSubscription()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var options = new DbContextOptionsBuilder<CollectorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new CollectorDbContext(options);
        for (var i = 1; i <= 1000; i++)
        {
            var subscription = Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow,
                SubscriptionType.Artist, BeatportId.Create(i));
            subscription.IncrementSubscribersCount();
            context.Subscriptions.Add(subscription);
        }
        await context.SaveChangesAsync(cancellationToken);

        var repository = new SubscriptionCommandRepository(context.Subscriptions, new TestClock());
        var commands = (await new QueueReleaseCollectionCommandHandler(repository)
            .HandleAsync(new QueueReleaseCollectionCommand(), cancellationToken)).Cast<CollectReleasesCommand>().ToArray();

        Assert.Equal(1000, commands.Length);
        Assert.Equal(1000, commands.Select(command => command.SubscriptionId).Distinct().Count());
    }

    [Fact]
    public async Task FindAllToRefreshAsync_ReturnsOnlySubscribedAndNotRefreshedToday()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var options = new DbContextOptionsBuilder<CollectorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new CollectorDbContext(options);
        var clock = new TestClock();
        var pending = Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow,
            SubscriptionType.Artist, BeatportId.Create(1));
        pending.IncrementSubscribersCount();
        var refreshed = Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow,
            SubscriptionType.Artist, BeatportId.Create(2));
        refreshed.IncrementSubscribersCount();
        refreshed.MarkRefreshed(clock.UtcToday);
        var unsubscribed = Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow,
            SubscriptionType.Artist, BeatportId.Create(3));
        context.Subscriptions.AddRange(pending, refreshed, unsubscribed);
        await context.SaveChangesAsync(cancellationToken);

        var result = await new SubscriptionCommandRepository(context.Subscriptions, clock).FindAllToRefreshAsync(cancellationToken);

        Assert.Equal([pending.Id], result);
    }

    [Fact]
    public async Task ExistsAsync_WhenAddedWithoutSaving_ShouldFindTrackedSubscription()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var options = new DbContextOptionsBuilder<CollectorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new CollectorDbContext(options);
        var repository = new SubscriptionCommandRepository(context.Subscriptions, new TestClock());
        var beatportId = BeatportId.Create(42);
        Assert.False(await repository.ExistsAsync(SubscriptionType.Artist, beatportId, cancellationToken));
        var subscription = Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow, SubscriptionType.Artist, beatportId);
        await repository.AddAsync(subscription, cancellationToken);

        Assert.True(await repository.ExistsAsync(SubscriptionType.Artist, beatportId, cancellationToken));
        Assert.True(await repository.ExistsAsync(SubscriptionType.Artist, beatportId, cancellationToken));

        Assert.False(await repository.ExistsAsync(SubscriptionType.Label, beatportId, cancellationToken));
        Assert.False(await repository.ExistsAsync(SubscriptionType.Artist, BeatportId.Create(43), cancellationToken));
        Assert.Single(context.Subscriptions.Local);
        Assert.Equal(EntityState.Added, context.Entry(subscription).State);
        Assert.Empty(await context.Subscriptions.ToListAsync(cancellationToken));
    }

    [Fact]
    public async Task ExistsAsync_WhenPersistedButNotTracked_ShouldQueryDatabase()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var options = new DbContextOptionsBuilder<CollectorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new CollectorDbContext(options);
        var beatportId = BeatportId.Create(42);
        context.Subscriptions.Add(Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow, SubscriptionType.Label, beatportId));
        await context.SaveChangesAsync(cancellationToken);
        context.ChangeTracker.Clear();
        var repository = new SubscriptionCommandRepository(context.Subscriptions, new TestClock());

        Assert.True(await repository.ExistsAsync(SubscriptionType.Label, beatportId, cancellationToken));
        Assert.False(await repository.ExistsAsync(SubscriptionType.Artist, beatportId, cancellationToken));
        Assert.False(await repository.ExistsAsync(SubscriptionType.Label, BeatportId.Create(43), cancellationToken));
        Assert.Empty(context.ChangeTracker.Entries());
    }

    private sealed class TestClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

        public DateOnly UtcToday => DateOnly.FromDateTime(UtcNow.UtcDateTime);
    }
}