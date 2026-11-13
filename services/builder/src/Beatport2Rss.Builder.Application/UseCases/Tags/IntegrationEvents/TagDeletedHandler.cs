using Beatport2Rss.Builder.Application.UseCases.Tags.Commands;
using Beatport2Rss.Common.IntegrationEvents.Tags;

namespace Beatport2Rss.Builder.Application.UseCases.Tags.IntegrationEvents;

public static class TagDeletedHandler
{
    public static DeleteTagCommand Handle(TagDeleted tagDeleted) =>
        new(tagDeleted.TagId);
}