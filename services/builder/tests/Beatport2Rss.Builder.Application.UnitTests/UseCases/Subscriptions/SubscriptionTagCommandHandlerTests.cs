using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Moq;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Subscriptions;

public sealed class SubscriptionTagCommandHandlerTests
{
    [Fact]
    public async Task CreateSubscriptionTagCommandHandler_ShouldAddAndSaveTag()
    {
        var subscriptionId = SubscriptionId.Create(Guid.NewGuid());
        var tagId = TagId.Create(Guid.NewGuid());
        var subscription = Subscription.Create(
            subscriptionId,
            DateTimeOffset.UtcNow,
            SubscriptionType.Label,
            SubscriptionName.Create("Label"),
            BeatportId.Create(123));
        var repository = new Mock<ISubscriptionCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        repository
            .Setup(value => value.FindAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        var handler = new CreateSubscriptionTagCommandHandler(repository.Object, unitOfWork.Object);

        await handler.HandleAsync(
            new CreateSubscriptionTagCommand(subscriptionId.Value, tagId.Value),
            CancellationToken.None);

        Assert.True(subscription.ContainsTag(tagId));
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateSubscriptionTagCommandHandler_WhenTagAlreadyExists_ShouldNotSave()
    {
        var subscriptionId = SubscriptionId.Create(Guid.NewGuid());
        var tagId = TagId.Create(Guid.NewGuid());
        var subscription = Subscription.Create(
            subscriptionId,
            DateTimeOffset.UtcNow,
            SubscriptionType.Label,
            SubscriptionName.Create("Label"),
            BeatportId.Create(123));
        subscription.AddTag(tagId);

        var repository = new Mock<ISubscriptionCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        repository
            .Setup(value => value.FindAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        var handler = new CreateSubscriptionTagCommandHandler(repository.Object, unitOfWork.Object);

        await handler.HandleAsync(
            new CreateSubscriptionTagCommand(subscriptionId.Value, tagId.Value),
            CancellationToken.None);

        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteSubscriptionTagCommandHandler_ShouldRemoveAndSaveTag()
    {
        var subscriptionId = SubscriptionId.Create(Guid.NewGuid());
        var tagId = TagId.Create(Guid.NewGuid());
        var subscription = Subscription.Create(
            subscriptionId,
            DateTimeOffset.UtcNow,
            SubscriptionType.Label,
            SubscriptionName.Create("Label"),
            BeatportId.Create(123));
        subscription.AddTag(tagId);

        var repository = new Mock<ISubscriptionCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        repository
            .Setup(value => value.FindAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        var handler = new DeleteSubscriptionTagCommandHandler(repository.Object, unitOfWork.Object);

        await handler.HandleAsync(
            new DeleteSubscriptionTagCommand(subscriptionId.Value, tagId.Value),
            CancellationToken.None);

        Assert.False(subscription.ContainsTag(tagId));
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}