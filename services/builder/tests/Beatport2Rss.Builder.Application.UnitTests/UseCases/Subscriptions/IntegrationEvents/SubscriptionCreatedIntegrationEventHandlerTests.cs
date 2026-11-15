using Beatport2Rss.Builder.Application.UseCases.Subscriptions.IntegrationEvents;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Subscriptions.IntegrationEvents;

public sealed class SubscriptionCreatedIntegrationEventHandlerTests
{
    [Fact]
    public void SubscriptionCreatedHandler_ShouldMapEventToCreateCommand()
    {
        var occurredAt = DateTimeOffset.UtcNow;
        var subscriptionId = Guid.NewGuid();
        var type = SubscriptionType.Label;
        const string name = "Label";
        const int beatportId = 42;
        var integrationEvent = new SubscriptionCreated(
            Guid.NewGuid(),
            occurredAt,
            subscriptionId,
            type,
            name,
            beatportId,
            "label",
            new Uri("https://example.com/image"),
            new Uri("https://example.com"),
            "Biography");

        var command = SubscriptionCreatedHandler.Handle(integrationEvent);

        Assert.Equal(subscriptionId, command.SubscriptionId);
        Assert.Equal(occurredAt, command.CreatedAt);
        Assert.Equal(name, command.Name);
        Assert.Equal(type, command.Type);
        Assert.Equal(beatportId, command.BeatportId);
    }
}