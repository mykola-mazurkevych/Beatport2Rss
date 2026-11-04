using Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Builder.Application.Extensions;

internal static class FeedCreatedExtensions
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
}