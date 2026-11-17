using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Builder.Application.UseCases.Tags.Commands;
using Beatport2Rss.Builder.Application.UseCases.Users.Commands;
using Beatport2Rss.Builder.Application.UseCases.Users.IntegrationEvents;
using Beatport2Rss.Builder.Domain.Users;
using Beatport2Rss.Common.Domain.Feeds;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents.Users;
using Beatport2Rss.Common.Miscellaneous.Exceptions;

using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Users.Commands;

public sealed class UserLifecycleCommandHandlerTests
{
    [Fact]
    public async Task RepeatedLifecycleEvents_ShouldSaveCreationAndDeletionOnlyOnce()
    {
        var token = TestContext.Current.CancellationToken;
        var userId = UserId.Create(Guid.NewGuid());
        var createdAt = DateTimeOffset.UtcNow;
        User? stored = null;
        var users = new Mock<IUserCommandRepository>(MockBehavior.Strict);
        users.Setup(value => value.FindAsync(userId, token)).ReturnsAsync(() => stored);
        users.Setup(value => value.AddAsync(It.IsAny<User>(), token))
            .Callback<User, CancellationToken>((user, _) => stored = user)
            .ReturnsAsync((User user, CancellationToken _) => user);
        users.Setup(value => value.Delete(It.IsAny<User>())).Callback(() => stored = null);
        var unitOfWork = new Mock<IUnitOfWork>();
        var create = new CreateUserCommandHandler(users.Object, unitOfWork.Object);
        var delete = new DeleteUserCommandHandler(users.Object, unitOfWork.Object);
        var created = UserCreatedHandler.Handle(new UserCreated(Guid.NewGuid(), createdAt, userId.Value));

        await create.HandleAsync(created, token);
        await create.HandleAsync(created, token);
        Assert.NotNull(stored);
        Assert.Equal(userId, stored.Id);
        Assert.Equal(createdAt, stored.CreatedAt);
        var deleted = UserDeletedHandler.Handle(new UserDeleted(Guid.NewGuid(), DateTimeOffset.UtcNow, userId.Value));
        await delete.HandleAsync(deleted, token);
        await delete.HandleAsync(deleted, token);

        Assert.Null(stored);
        unitOfWork.Verify(value => value.SaveChangesAsync(token), Times.Exactly(2));
    }

    [Fact]
    public async Task CreatingFeedOrTagBeforeUser_ShouldRetryWithoutSaving()
    {
        var token = TestContext.Current.CancellationToken;
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var users = Mock.Of<IUserCommandRepository>();
        var createFeed = new CreateFeedCommandHandler(
            Mock.Of<IFeedCommandRepository>(), users, unitOfWork.Object, NullLogger<CreateFeedCommand>.Instance);
        var createTag = new CreateTagCommandHandler(
            Mock.Of<ITagCommandRepository>(), users, unitOfWork.Object);

        await Assert.ThrowsAsync<RetryableException>(() => createFeed.Handle(
            new CreateFeedCommand(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow, "Feed", "feed", null, FeedStatus.Active), token));
        await Assert.ThrowsAsync<RetryableException>(() => createTag.HandleAsync(
            new CreateTagCommand(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow, "Tag"), token));

        Assert.Empty(unitOfWork.Invocations);
    }
}