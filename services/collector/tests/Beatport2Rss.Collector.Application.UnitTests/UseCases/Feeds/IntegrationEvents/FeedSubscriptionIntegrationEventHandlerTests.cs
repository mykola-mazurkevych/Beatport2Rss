using Beatport2Rss.Collector.Application.UseCases.Feeds.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

using Xunit;

namespace Beatport2Rss.Collector.Application.UnitTests.UseCases.Feeds.IntegrationEvents;

public sealed class FeedSubscriptionIntegrationEventHandlerTests
{
    [Fact]
    public void FeedSubscriptionCreatedHandler_ShouldMapSubscriptionId()
    {
        var subscriptionId = Guid.NewGuid();
        var feedId = Guid.NewGuid();
        var integrationEvent = new FeedSubscriptionCreated(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            feedId,
            subscriptionId);

        var command = FeedSubscriptionCreatedHandler.Handle(integrationEvent);

        Assert.Equal(subscriptionId, command.SubscriptionId);
        Assert.Equal(feedId, command.FeedId);
    }

    [Fact]
    public void FeedSubscriptionDeletedHandler_ShouldMapSubscriptionId()
    {
        var subscriptionId = Guid.NewGuid();
        var feedId = Guid.NewGuid();
        var integrationEvent = new FeedSubscriptionDeleted(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            feedId,
            subscriptionId);

        var command = FeedSubscriptionDeletedHandler.Handle(integrationEvent);

        Assert.Equal(subscriptionId, command.SubscriptionId);
        Assert.Equal(feedId, command.FeedId);
    }
}