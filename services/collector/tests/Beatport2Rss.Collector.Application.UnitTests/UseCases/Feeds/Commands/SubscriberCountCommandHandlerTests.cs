using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Application.UseCases.Feeds.Commands;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using Xunit;

namespace Beatport2Rss.Collector.Application.UnitTests.UseCases.Feeds.Commands;

public sealed class SubscriberCountCommandHandlerTests
{
    [Fact]
    public async Task IncrementSubscribersCountCommandHandler_WhenSubscriptionIsMissing_ShouldNotSave()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ISubscriptionCommandRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        repository.Setup(value => value.FindAsync(It.IsAny<SubscriptionId>(), cancellationToken))
            .ReturnsAsync((Subscription?)null);
        var handler = new IncrementSubscribersCountCommandHandler(
            repository.Object,
            unitOfWork.Object,
            NullLogger<IncrementSubscribersCountCommand>.Instance);

        await handler.HandleAsync(
            new IncrementSubscribersCountCommand(Guid.NewGuid()),
            cancellationToken);

        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DecrementSubscribersCountCommandHandler_WhenSubscriptionIsMissing_ShouldNotSave()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ISubscriptionCommandRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        repository.Setup(value => value.FindAsync(It.IsAny<SubscriptionId>(), cancellationToken))
            .ReturnsAsync((Subscription?)null);
        var handler = new DecrementSubscribersCountCommandHandler(
            repository.Object,
            unitOfWork.Object,
            NullLogger<DecrementSubscribersCountCommand>.Instance);

        await handler.HandleAsync(
            new DecrementSubscribersCountCommand(Guid.NewGuid()),
            cancellationToken);

        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DecrementSubscribersCountCommandHandler_WhenCountIsAlreadyZero_ShouldNotSave()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var subscription = Subscription.Create(
            SubscriptionId.Create(Guid.NewGuid()),
            DateTimeOffset.UtcNow,
            SubscriptionType.Artist,
            BeatportId.Create(42));
        var repository = new Mock<ISubscriptionCommandRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        repository.Setup(value => value.FindAsync(subscription.Id, cancellationToken))
            .ReturnsAsync(subscription);
        var handler = new DecrementSubscribersCountCommandHandler(
            repository.Object,
            unitOfWork.Object,
            NullLogger<DecrementSubscribersCountCommand>.Instance);

        await handler.HandleAsync(
            new DecrementSubscribersCountCommand(subscription.Id.Value),
            cancellationToken);

        Assert.Equal(0, subscription.SubscribersCount);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}