using Beatport2Rss.Builder.Application.UseCases.Users.Commands;
using Beatport2Rss.Builder.Domain.Feeds;
using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Builder.Domain.Tags;
using Beatport2Rss.Builder.Domain.Users;
using Beatport2Rss.Builder.Infrastructure.Persistence;
using Beatport2Rss.Builder.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Users.Commands;

public sealed class UserDeletionPersistenceTests
{
    [Fact]
    public async Task DeletingUser_ShouldCascadeOwnedDataAndPreserveSharedSubscriptionAndOtherOwner()
    {
        var token = TestContext.Current.CancellationToken;
        var options = new DbContextOptionsBuilder<BuilderDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new BuilderDbContext(options);
        var now = DateTimeOffset.UtcNow;
        var user = User.Create(UserId.Create(Guid.NewGuid()), now);
        var otherUser = User.Create(UserId.Create(Guid.NewGuid()), now);
        var subscription = Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), now,
            SubscriptionType.Label, SubscriptionName.Create("Label"), BeatportId.Create(42));
        var feed = Feed.Create(FeedId.Create(Guid.NewGuid()), now, user.Id,
            FeedName.Create("Feed"), Slug.Create("feed"), null, FeedStatus.Active);
        var otherFeed = Feed.Create(FeedId.Create(Guid.NewGuid()), now, otherUser.Id,
            FeedName.Create("Other feed"), Slug.Create("other-feed"), null, FeedStatus.Active);
        var tag = Tag.Create(TagId.Create(Guid.NewGuid()), now, user.Id, TagName.Create("Tag"));
        var otherTag = Tag.Create(TagId.Create(Guid.NewGuid()), now, otherUser.Id, TagName.Create("Other tag"));
        feed.AddSubscription(subscription.Id);
        otherFeed.AddSubscription(subscription.Id);
        subscription.AddTag(tag.Id);
        subscription.AddTag(otherTag.Id);
        context.Users.AddRange(user, otherUser);
        context.Feeds.AddRange(feed, otherFeed);
        context.Tags.AddRange(tag, otherTag);
        context.Subscriptions.Add(subscription);
        context.RssFeedDocuments.AddRange(
            RssFeedDocument.Create(feed.Id, now, now, "<rss />"),
            RssFeedDocument.Create(otherFeed.Id, now, now, "<rss />"));
        await context.SaveChangesAsync(token);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(value => value.SaveChangesAsync(token)).Returns<CancellationToken>(context.SaveChangesAsync);
        var handler = new DeleteUserCommandHandler(new UserCommandRepository(context.Users), unitOfWork.Object);

        await handler.HandleAsync(new DeleteUserCommand(user.Id.Value), token);

        Assert.Equal(otherUser.Id, (await context.Users.SingleAsync(token)).Id);
        Assert.Equal(otherFeed.Id, (await context.Feeds.SingleAsync(token)).Id);
        Assert.Equal(otherFeed.Id, (await context.RssFeedDocuments.SingleAsync(token)).Id);
        Assert.Equal(otherTag.Id, (await context.Tags.SingleAsync(token)).Id);
        Assert.Equal(subscription.Id, (await context.Subscriptions.SingleAsync(token)).Id);
        Assert.Equal(otherTag.Id, await context.Subscriptions.SelectMany(value => value.Tags)
            .Select(link => link.TagId).SingleAsync(token));
        Assert.Equal(otherFeed.Id, await context.Feeds.SelectMany(value => value.Subscriptions)
            .Select(link => link.FeedId).SingleAsync(token));
    }
}