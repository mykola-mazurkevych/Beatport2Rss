using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Subscriptions.Commands;

public sealed class CreateSubscriptionCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldAddAndSaveSubscription()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var subscriptionId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var repository = new Mock<ISubscriptionCommandRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        Subscription? addedSubscription = null;
        repository.Setup(value => value.ExistsAsync(SubscriptionId.Create(subscriptionId), cancellationToken))
            .ReturnsAsync(false);
        repository.Setup(value => value.AddAsync(It.IsAny<Subscription>(), cancellationToken))
            .Callback<Subscription, CancellationToken>((subscription, _) => addedSubscription = subscription)
            .ReturnsAsync((Subscription subscription, CancellationToken _) => subscription);
        unitOfWork.Setup(value => value.SaveChangesAsync(cancellationToken)).Returns(Task.CompletedTask);
        var handler = new CreateSubscriptionCommandHandler(
            repository.Object,
            unitOfWork.Object,
            NullLogger<CreateSubscriptionCommand>.Instance);

        await handler.Handle(
            new CreateSubscriptionCommand(
                subscriptionId,
                createdAt,
                "Subscription",
                SubscriptionType.Artist,
                42),
            cancellationToken);

        Assert.NotNull(addedSubscription);
        Assert.Equal(SubscriptionId.Create(subscriptionId), addedSubscription.Id);
        Assert.Equal(createdAt, addedSubscription.CreatedAt);
        Assert.Equal(SubscriptionType.Artist, addedSubscription.Type);
        Assert.Equal(SubscriptionName.Create("Subscription"), addedSubscription.Name);
        Assert.Equal(BeatportId.Create(42), addedSubscription.BeatportId);
        unitOfWork.Verify(value => value.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSubscriptionAlreadyExists_ShouldNotAddOrSave()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ISubscriptionCommandRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        repository.Setup(value => value.ExistsAsync(It.IsAny<SubscriptionId>(), cancellationToken))
            .ReturnsAsync(true);
        var handler = new CreateSubscriptionCommandHandler(
            repository.Object,
            unitOfWork.Object,
            NullLogger<CreateSubscriptionCommand>.Instance);

        await handler.Handle(
            new CreateSubscriptionCommand(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                "Subscription",
                SubscriptionType.Artist,
                42),
            cancellationToken);

        repository.Verify(value => value.AddAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}