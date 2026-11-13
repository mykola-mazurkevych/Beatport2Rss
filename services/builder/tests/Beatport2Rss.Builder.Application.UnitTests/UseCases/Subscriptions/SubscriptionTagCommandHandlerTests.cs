using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Tags;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;

using Microsoft.Extensions.Logging;

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

        var logger = new RecordingLogger<CreateSubscriptionTagCommandHandler>();
        var handler = new CreateSubscriptionTagCommandHandler(repository.Object, unitOfWork.Object, logger);

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

        var logger = new RecordingLogger<CreateSubscriptionTagCommandHandler>();
        var handler = new CreateSubscriptionTagCommandHandler(repository.Object, unitOfWork.Object, logger);

        await handler.HandleAsync(
            new CreateSubscriptionTagCommand(subscriptionId.Value, tagId.Value),
            CancellationToken.None);

        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        Assert.Contains(LogLevel.Warning, logger.Levels);
    }

    [Fact]
    public async Task CreateSubscriptionTagCommandHandler_WhenSubscriptionIsMissing_ShouldLogWarning()
    {
        var subscriptionId = Guid.NewGuid();
        var repository = new Mock<ISubscriptionCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var logger = new RecordingLogger<CreateSubscriptionTagCommandHandler>();
        var handler = new CreateSubscriptionTagCommandHandler(repository.Object, unitOfWork.Object, logger);

        await handler.HandleAsync(
            new CreateSubscriptionTagCommand(subscriptionId, Guid.NewGuid()),
            CancellationToken.None);

        Assert.Contains(LogLevel.Warning, logger.Levels);
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

        var logger = new RecordingLogger<DeleteSubscriptionTagCommandHandler>();
        var handler = new DeleteSubscriptionTagCommandHandler(repository.Object, unitOfWork.Object, logger);

        await handler.HandleAsync(
            new DeleteSubscriptionTagCommand(subscriptionId.Value, tagId.Value),
            CancellationToken.None);

        Assert.False(subscription.ContainsTag(tagId));
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteSubscriptionTagCommandHandler_WhenTagIsMissing_ShouldLogWarning()
    {
        var subscriptionId = SubscriptionId.Create(Guid.NewGuid());
        var subscription = Subscription.Create(
            subscriptionId,
            DateTimeOffset.UtcNow,
            SubscriptionType.Label,
            SubscriptionName.Create("Label"),
            BeatportId.Create(123));
        var tagId = Guid.NewGuid();

        var repository = new Mock<ISubscriptionCommandRepository>();
        repository
            .Setup(value => value.FindAsync(subscriptionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        var unitOfWork = new Mock<IUnitOfWork>();
        var logger = new RecordingLogger<DeleteSubscriptionTagCommandHandler>();
        var handler = new DeleteSubscriptionTagCommandHandler(repository.Object, unitOfWork.Object, logger);

        await handler.HandleAsync(
            new DeleteSubscriptionTagCommand(subscriptionId.Value, tagId),
            CancellationToken.None);

        Assert.Contains(LogLevel.Warning, logger.Levels);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteSubscriptionTagCommandHandler_WhenSubscriptionIsMissing_ShouldLogWarning()
    {
        var repository = new Mock<ISubscriptionCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var logger = new RecordingLogger<DeleteSubscriptionTagCommandHandler>();
        var handler = new DeleteSubscriptionTagCommandHandler(repository.Object, unitOfWork.Object, logger);

        await handler.HandleAsync(
            new DeleteSubscriptionTagCommand(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        Assert.Contains(LogLevel.Warning, logger.Levels);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}

internal sealed class RecordingLogger<T> :
    ILogger<T>
{
    public List<LogLevel> Levels { get; } = [];

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull =>
        null;

    public bool IsEnabled(LogLevel logLevel) =>
        true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter) =>
        Levels.Add(logLevel);
}