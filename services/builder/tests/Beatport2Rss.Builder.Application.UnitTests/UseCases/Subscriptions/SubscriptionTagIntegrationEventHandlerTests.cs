using Beatport2Rss.Builder.Application.UseCases.Subscriptions.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Subscriptions;

public sealed class SubscriptionTagIntegrationEventHandlerTests
{
    [Fact]
    public void SubscriptionTagCreatedHandler_ShouldMapEventToCreateCommand()
    {
        var subscriptionId = Guid.NewGuid();
        var tagId = Guid.NewGuid();
        var command = SubscriptionTagCreatedHandler.Handle(
            new SubscriptionTagCreated(Guid.NewGuid(), DateTimeOffset.UtcNow, subscriptionId, tagId));

        Assert.Equal(subscriptionId, command.SubscriptionId);
        Assert.Equal(tagId, command.TagId);
    }

    [Fact]
    public void SubscriptionTagDeletedHandler_ShouldMapEventToDeleteCommand()
    {
        var subscriptionId = Guid.NewGuid();
        var tagId = Guid.NewGuid();
        var command = SubscriptionTagDeletedHandler.Handle(
            new SubscriptionTagDeleted(Guid.NewGuid(), DateTimeOffset.UtcNow, subscriptionId, tagId));

        Assert.Equal(subscriptionId, command.SubscriptionId);
        Assert.Equal(tagId, command.TagId);
    }
}
