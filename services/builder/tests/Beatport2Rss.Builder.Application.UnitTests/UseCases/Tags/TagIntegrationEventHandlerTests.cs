using Beatport2Rss.Builder.Application.UseCases.Tags.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Tags;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Tags;

public sealed class TagIntegrationEventHandlerTests
{
    [Fact]
    public void TagCreatedHandler_ShouldMapEventToCreateCommand()
    {
        var occurredAt = DateTimeOffset.UtcNow;
        var tagId = Guid.NewGuid();
        var command = TagCreatedHandler.Handle(
            new TagCreated(Guid.NewGuid(), occurredAt, tagId, Guid.NewGuid(), "Techno"));

        Assert.Equal(tagId, command.TagId);
        Assert.Equal(occurredAt, command.CreatedAt);
        Assert.Equal("Techno", command.Name);
    }

    [Fact]
    public void TagUpdatedHandler_ShouldMapEventToUpdateCommand()
    {
        var tagId = Guid.NewGuid();
        var command = TagUpdatedHandler.Handle(
            new TagUpdated(Guid.NewGuid(), DateTimeOffset.UtcNow, tagId, "House"));

        Assert.Equal(tagId, command.TagId);
        Assert.Equal("House", command.Name);
    }

    [Fact]
    public void TagDeletedHandler_ShouldMapEventToDeleteCommand()
    {
        var tagId = Guid.NewGuid();
        var command = TagDeletedHandler.Handle(
            new TagDeleted(Guid.NewGuid(), DateTimeOffset.UtcNow, tagId));

        Assert.Equal(tagId, command.TagId);
    }
}
