using Beatport2Rss.Collector.Application.UseCases.Releases.Commands;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Collector.Domain.Feeds;
using Beatport2Rss.Collector.Domain.Users;
using Beatport2Rss.Collector.Infrastructure.Persistence;
using Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Beatport2Rss.Collector.Infrastructure.UnitTests.UseCases.Releases.Commands;

public sealed class QueueReleaseCollectionCommandHandlerTests
{
    [Fact]
    public async Task QueuesEveryEligibleSubscription()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var options = new DbContextOptionsBuilder<CollectorDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new CollectorDbContext(options);
        var user = User.Create(UserId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow);
        var feed = Feed.Create(FeedId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow, user.Id);
        context.Users.Add(user);
        context.Feeds.Add(feed);
        for (var i = 1; i <= 1000; i++)
        {
            var subscription = Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow,
                SubscriptionType.Artist, BeatportId.Create(i));
            feed.AddSubscription(subscription.Id);
            context.Subscriptions.Add(subscription);
        }
        await context.SaveChangesAsync(cancellationToken);

        var repository = new SubscriptionCommandRepository(context.Feeds, context.Subscriptions, new TestClock());
        var commands = (await new QueueReleaseCollectionCommandHandler(repository)
            .HandleAsync(new QueueReleaseCollectionCommand(), cancellationToken)).Cast<CollectReleasesCommand>().ToArray();

        Assert.Equal(1000, commands.Length);
        Assert.Equal(1000, commands.Select(command => command.SubscriptionId).Distinct().Count());
    }

    private sealed class TestClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

        public DateOnly UtcToday => DateOnly.FromDateTime(UtcNow.UtcDateTime);
    }
}