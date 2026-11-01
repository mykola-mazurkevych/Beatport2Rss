using Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Builder.Application.Extensions;

internal static class FeedExtensions
{
    extension(FeedCreated feedCreated)
    {
        public CreateFeedCommand ToCommand() =>
            new(feedCreated.FeedId,
                feedCreated.OccurredAt,
                feedCreated.Name,
                feedCreated.Slug,
                feedCreated.AuthorName,
                feedCreated.Status);
    }

    extension(FeedUpdated feedUpdated)
    {
        public UpdateFeedCommand ToCommand() =>
            new(feedUpdated.FeedId,
                feedUpdated.Name,
                feedUpdated.Slug,
                feedUpdated.AuthorName,
                feedUpdated.Status);
    }

    extension(FeedDeleted feedDeleted)
    {
        public DeleteFeedCommand ToCommand() =>
            new(feedDeleted.FeedId);
    }
}