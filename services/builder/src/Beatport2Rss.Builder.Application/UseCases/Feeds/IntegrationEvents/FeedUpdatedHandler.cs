using Beatport2Rss.Builder.Application.Extensions;
using Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

namespace Beatport2Rss.Builder.Application.UseCases.Feeds.IntegrationEvents;

public static class FeedUpdatedHandler
{
    public static UpdateFeedCommand Handle(FeedUpdated feedUpdated) =>
        feedUpdated.ToCommand();
}