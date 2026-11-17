using Beatport2Rss.Collector.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Collector.Application.UseCases.Feeds.IntegrationEvents;

public static class FeedDeletedHandler
{
    public static DeleteFeedCommand Handle(FeedDeleted feedDeleted) =>
        new(feedDeleted.FeedId);
}