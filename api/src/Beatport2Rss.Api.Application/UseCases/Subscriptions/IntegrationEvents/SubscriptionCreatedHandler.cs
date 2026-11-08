using Beatport2Rss.Api.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;

namespace Beatport2Rss.Api.Application.UseCases.Subscriptions.IntegrationEvents;

internal static class SubscriptionCreatedHandler
{
    public static CreateSubscriptionCommand Handle(SubscriptionCreated subscriptionCreated) =>
        new(subscriptionCreated.SubscriptionId,
            subscriptionCreated.Type,
            subscriptionCreated.Name,
            subscriptionCreated.BeatportId,
            subscriptionCreated.BeatportSlug,
            subscriptionCreated.ImageUri,
            subscriptionCreated.Website,
            subscriptionCreated.Bio);
}