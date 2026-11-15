using Beatport2Rss.Builder.Application.UseCases.Feeds.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Feeds;

public sealed class FeedSubscriptionIntegrationEventHandlerTests
{
    [Fact]
    public void FeedSubscriptionCreatedHandler_ShouldMapEventToCreateCommand()
    {
        var feedId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();

        var command = FeedSubscriptionCreatedHandler.Handle(
            new FeedSubscriptionCreated(Guid.NewGuid(), DateTimeOffset.UtcNow, feedId, subscriptionId));

        Assert.Equal(feedId, command.FeedId);
        Assert.Equal(subscriptionId, command.SubscriptionId);
    }

    [Fact]
    public void FeedSubscriptionDeletedHandler_ShouldMapEventToDeleteCommand()
    {
        var feedId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();

        var command = FeedSubscriptionDeletedHandler.Handle(
            new FeedSubscriptionDeleted(Guid.NewGuid(), DateTimeOffset.UtcNow, feedId, subscriptionId));

        Assert.Equal(feedId, command.FeedId);
        Assert.Equal(subscriptionId, command.SubscriptionId);
    }
}