using Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Builder.Application.Extensions;

internal static class FeedDeletedExtensions
{
    extension(FeedDeleted feedDeleted)
    {
        public DeleteFeedCommand ToCommand() =>
            new(feedDeleted.FeedId);
    }
}