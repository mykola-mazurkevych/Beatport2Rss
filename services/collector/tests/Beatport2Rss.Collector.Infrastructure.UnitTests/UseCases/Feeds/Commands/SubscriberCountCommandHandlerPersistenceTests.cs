using Beatport2Rss.Collector.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Collector.Domain.Subscriptions;
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

namespace Beatport2Rss.Collector.Infrastructure.UnitTests.UseCases.Feeds.Commands;

public sealed class SubscriberCountCommandHandlerPersistenceTests
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
            .Returns<CancellationToken>(handlerContext.SaveChangesAsync);
        if (increment)
        {
            var handler = new IncrementSubscribersCountCommandHandler(
                repository,
                unitOfWork.Object,
                NullLogger<IncrementSubscribersCountCommand>.Instance);
            await handler.HandleAsync(new IncrementSubscribersCountCommand(subscriptionId.Value), cancellationToken);
        }
        else
        {
            var handler = new DecrementSubscribersCountCommandHandler(
                repository,
                unitOfWork.Object,
                NullLogger<DecrementSubscribersCountCommand>.Instance);
            await handler.HandleAsync(new DecrementSubscribersCountCommand(subscriptionId.Value), cancellationToken);
        }

        await using var verificationContext = new CollectorDbContext(options);
        var savedSubscription = await verificationContext.Subscriptions.SingleAsync(cancellationToken);
        Assert.Equal(expectedCount, savedSubscription.SubscribersCount);
        Assert.Equal(refreshedAt, savedSubscription.RefreshedAt);
    }

    private sealed class TestClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

        public DateOnly UtcToday => DateOnly.FromDateTime(UtcNow.UtcDateTime);
    }
}