using Beatport2Rss.Builder.Application.UseCases.Tags.Commands;
using Beatport2Rss.Common.IntegrationEvents.Tags;

namespace Beatport2Rss.Builder.Application.UseCases.Tags.IntegrationEvents;

public static class TagCreatedHandler
{
    public static CreateTagCommand Handle(TagCreated tagCreated) =>
        new(tagCreated.TagId, tagCreated.OccurredAt, tagCreated.Name);
}
