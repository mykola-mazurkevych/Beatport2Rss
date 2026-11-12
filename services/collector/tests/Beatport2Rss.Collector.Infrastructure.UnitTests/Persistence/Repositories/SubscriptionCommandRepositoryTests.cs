using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Collector.Application.UseCases.Releases.Commands;
using Beatport2Rss.Collector.Infrastructure.Persistence;
using Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Beatport2Rss.Collector.Infrastructure.UnitTests.Persistence.Repositories;

public sealed class SubscriptionCommandRepositoryTests
{
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