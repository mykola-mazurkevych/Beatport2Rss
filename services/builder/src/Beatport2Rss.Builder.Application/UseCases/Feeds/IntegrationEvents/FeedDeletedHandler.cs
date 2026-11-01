using Beatport2Rss.Builder.Application.Extensions;
using Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.IntegrationEvents;

public static class FeedDeletedHandler
{
    public static DeleteFeedCommand Handle(FeedDeleted feedDeleted) =>
        feedDeleted.ToCommand();
}