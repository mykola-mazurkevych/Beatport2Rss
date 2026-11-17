using Beatport2Rss.Collector.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Collector.Application.UseCases.Feeds.IntegrationEvents;

public static class FeedCreatedHandler
{
    public static CreateFeedCommand Handle(FeedCreated feedCreated) =>
        new(feedCreated.FeedId,
            feedCreated.OccurredAt,
            feedCreated.UserId);
}