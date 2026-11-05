using Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.IntegrationEvents;

public static class FeedCreatedHandler
{
    public static CreateFeedCommand Handle(FeedCreated feedCreated) =>
        new(feedCreated.FeedId,
            feedCreated.OccurredAt,
            feedCreated.Name,
            feedCreated.Slug,
            feedCreated.AuthorName,
            feedCreated.Status);
}