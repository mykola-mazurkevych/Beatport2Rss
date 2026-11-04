using Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Builder.Application.Extensions;

internal static class FeedUpdatedExtensions
{
    extension(FeedUpdated feedUpdated)
    {
        public UpdateFeedCommand ToCommand() =>
            new(feedUpdated.FeedId,
                feedUpdated.Name,
                feedUpdated.Slug,
                feedUpdated.AuthorName,
                feedUpdated.Status);
    }
}