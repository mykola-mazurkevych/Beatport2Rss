using System.Net;

using Beatport2Rss.Collector.Application.Interfaces.Services;
using Beatport2Rss.Collector.Application.UseCases.Subscriptions.Commands;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.FluentResults.Errors;
using Beatport2Rss.Common.Miscellaneous.Exceptions;

using FluentResults;

using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using Xunit;

namespace Beatport2Rss.Collector.Application.UnitTests.UseCases.Subscriptions.Commands;

public sealed class CreateSubscriptionCommandTests
{
    [Fact]
    public async Task HandleAsync_WhenSuccessful_ShouldSaveChangesAfterEnsuringSubscription()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.Setup(value => value.SaveChangesAsync(cancellationToken)).Returns(Task.CompletedTask);
        var service = new StubSubscriptionService((_, _, _) =>
        {
            Assert.Empty(unitOfWork.Invocations);
            return Task.FromResult(Result.Ok());
        });
        var handler = new CreateSubscriptionCommandHandler(service, unitOfWork.Object, NullLogger<CreateSubscriptionCommand>.Instance);

        await handler.HandleAsync(new CreateSubscriptionCommand(SubscriptionType.Label, 42), cancellationToken);

        Assert.Equal(1, service.Calls);
        Assert.Equal(SubscriptionType.Label, service.ReceivedType);
        Assert.Equal(BeatportId.Create(42), service.ReceivedBeatportId);
        Assert.Equal(cancellationToken, service.ReceivedCancellationToken);
        unitOfWork.Verify(value => value.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task HandleAsync_WhenFailed_ShouldRetryOnlyTransientErrorsWithoutSaving(int errorIndex)
    {
        var errors = new IError[]
        {
            new TransientError("Unavailable"),
            new TransientHttpError("Unavailable", HttpStatusCode.ServiceUnavailable),
            new RequestTimeoutError("Timeout"),
            new TooManyRequestsError("Rate limited"),
            new NotFoundError("Missing")
        };
        var cancellationToken = TestContext.Current.CancellationToken;
        var service = new StubSubscriptionService((_, _, _) => Task.FromResult(Result.Fail(errors[errorIndex])));
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var handler = new CreateSubscriptionCommandHandler(service, unitOfWork.Object, NullLogger<CreateSubscriptionCommand>.Instance);
        var command = new CreateSubscriptionCommand(SubscriptionType.Artist, 42);

        if (errorIndex < 4)
        {
            var exception = await Assert.ThrowsAsync<RetryableException>(() => handler.HandleAsync(command, cancellationToken));
            Assert.Contains(errors[errorIndex].Message, exception.Message, StringComparison.Ordinal);
        }
        else
        {
            await handler.HandleAsync(command, cancellationToken);
        }

        Assert.Equal(1, service.Calls);
        Assert.Equal(SubscriptionType.Artist, service.ReceivedType);
        Assert.Equal(BeatportId.Create(42), service.ReceivedBeatportId);
        Assert.Equal(cancellationToken, service.ReceivedCancellationToken);
        Assert.Empty(unitOfWork.Invocations);
    }

    private sealed class StubSubscriptionService(
        Func<SubscriptionType, BeatportId, CancellationToken, Task<Result>> ensure) : ISubscriptionService
    {
        public int Calls { get; private set; }
        public SubscriptionType? ReceivedType { get; private set; }
        public BeatportId? ReceivedBeatportId { get; private set; }
        public CancellationToken ReceivedCancellationToken { get; private set; }

        public Task<Result> EnsureSubscriptionExistsAsync(
            SubscriptionType subscriptionType,
            BeatportId beatportId,
            CancellationToken cancellationToken = default)
        {
            Calls++;
            ReceivedType = subscriptionType;
            ReceivedBeatportId = beatportId;
            ReceivedCancellationToken = cancellationToken;
            return ensure(subscriptionType, beatportId, cancellationToken);
        }
    }
}