using Beatport2Rss.Builder.Application.UseCases.Tags.Commands;
using Beatport2Rss.Common.IntegrationEvents.Tags;

namespace Beatport2Rss.Builder.Application.UseCases.Tags.IntegrationEvents;

public static class TagUpdatedHandler
{
    public static UpdateTagCommand Handle(TagUpdated tagUpdated) =>
        new(tagUpdated.TagId, tagUpdated.Name);
}
