using Beatport2Rss.Collector.Domain.Subscriptions;
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