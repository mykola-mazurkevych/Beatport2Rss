using Beatport2Rss.Builder.Application.UseCases.Feeds.IntegrationEvents;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.IntegrationEvents.Feeds;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Feeds.IntegrationEvents;

public sealed class FeedLifecycleIntegrationEventHandlerTests
{
    [Fact]
    public void FeedCreatedHandler_ShouldMapEventToCreateCommand()
    {
        var occurredAt = DateTimeOffset.UtcNow;
        var feedId = Guid.NewGuid();
        const string name = "Feed";
        const string slug = "feed";
        const string authorName = "Author";
        var status = FeedStatus.Active;
        var integrationEvent = new FeedCreated(
            Guid.NewGuid(),
            occurredAt,
            feedId,
            Guid.NewGuid(),
            name,
            slug,
            authorName,
            status);

        var command = FeedCreatedHandler.Handle(integrationEvent);

        Assert.Equal(feedId, command.FeedId);
        Assert.Equal(occurredAt, command.CreatedAt);
        Assert.Equal(name, command.Name);
        Assert.Equal(slug, command.Slug);
        Assert.Equal(authorName, command.AuthorName);
        Assert.Equal(status, command.Status);
    }

    [Fact]
    public void FeedUpdatedHandler_ShouldMapEventToUpdateCommand()
    {
        var feedId = Guid.NewGuid();
        const string name = "Updated";
        const string slug = "updated";
        const string authorName = "Updated Author";
        var status = FeedStatus.Inactive;
        var integrationEvent = new FeedUpdated(
            Guid.NewGuid(),
            DateTimeOffset.UtcNow,
            feedId,
            name,
            slug,
            authorName,
            status);

        var command = FeedUpdatedHandler.Handle(integrationEvent);

        Assert.Equal(feedId, command.FeedId);
        Assert.Equal(name, command.Name);
        Assert.Equal(slug, command.Slug);
        Assert.Equal(authorName, command.AuthorName);
        Assert.Equal(status, command.Status);
    }

    [Fact]
    public void FeedDeletedHandler_ShouldMapEventToDeleteCommand()
    {
        var feedId = Guid.NewGuid();

        var command = FeedDeletedHandler.Handle(
            new FeedDeleted(Guid.NewGuid(), DateTimeOffset.UtcNow, feedId));

        Assert.Equal(feedId, command.FeedId);
    }
}