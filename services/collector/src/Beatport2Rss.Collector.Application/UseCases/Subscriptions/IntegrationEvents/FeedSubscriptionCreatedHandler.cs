using Beatport2Rss.Collector.Application.Extensions;
using Beatport2Rss.Collector.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Collector.Application.UseCases.Subscriptions.IntegrationEvents;

public static class FeedSubscriptionCreatedHandler
{
    public static IncrementSubscribersCountCommand Handle(FeedSubscriptionCreated feedSubscriptionCreated) =>
        feedSubscriptionCreated.ToCommand();
}