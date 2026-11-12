using System.Globalization;
using System.Net;

using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Application.Interfaces.Services;
using Beatport2Rss.Collector.Application.UseCases.Releases.Commands;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Models;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.FluentResults.Extensions;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.Miscellaneous.Exceptions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using Microsoft.Extensions.Logging.Abstractions;

using Moq;

using Xunit;

namespace Beatport2Rss.Collector.Application.UnitTests.UseCases;

public sealed class CollectReleasesCommandTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task HandleAsync_RetriesOnlyTransientBeatportErrors(int errorIndex)
    {
        var failures = new[]
        {
            Result.TooManyRequests("Rate limited"),
            Result.Transient("Unavailable", HttpStatusCode.ServiceUnavailable),
            Result.RequestTimeout("Timeout"),
            Result.Transient("Network error"),
            Result.NotFound("Missing")
        };
        var cancellationToken = TestContext.Current.CancellationToken;
        var subscription = CreateSubscription();
        subscription.MarkRefreshed(new DateOnly(2026, 11, 5));
        var client = new Mock<IBeatportClient>();
        client.Setup(value => value.SearchForIdsAsync<BeatportReleaseDto>(
                It.IsAny<IReadOnlyDictionary<string, string?>>(), cancellationToken))
            .ReturnsAsync(Result.Fail<IReadOnlyCollection<int>>(failures[errorIndex].Errors));
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = CreateHandler(subscription, client.Object, unitOfWork.Object);
        var command = new CollectReleasesCommand(subscription.Id.Value);

        if (errorIndex < 4)
        {
            var exception = await Assert.ThrowsAsync<RetryableException>(() => handler.HandleAsync(command, cancellationToken));
            Assert.Equal(failures[errorIndex].Errors[0].Message, exception.Message);
        }
        else
        {
            await handler.HandleAsync(command, cancellationToken);
        }

        Assert.Equal(new DateOnly(2026, 11, 5), subscription.RefreshedAt);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task HandleAsync_PropagatesNetworkErrorsAndRetriesTimeouts(bool timeout)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var subscription = CreateSubscription();
        var client = new Mock<IBeatportClient>();
        Exception failure = timeout ? new TaskCanceledException("Timeout") : new HttpRequestException("Network error");
        client.Setup(value => value.SearchForIdsAsync<BeatportReleaseDto>(
                It.IsAny<IReadOnlyDictionary<string, string?>>(), cancellationToken))
            .ThrowsAsync(failure);
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = CreateHandler(subscription, client.Object, unitOfWork.Object);
        var command = new CollectReleasesCommand(subscription.Id.Value);

        if (timeout)
        {
            var exception = await Assert.ThrowsAsync<RetryableException>(() => handler.HandleAsync(command, cancellationToken));
            Assert.Same(failure, exception.InnerException);
        }
        else
        {
            await Assert.ThrowsAsync<HttpRequestException>(() => handler.HandleAsync(command, cancellationToken));
        }
        Assert.Null(subscription.RefreshedAt);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_DoesNotRetryCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        var cancellationToken = cancellation.Token;
        var subscription = CreateSubscription();
        var client = new Mock<IBeatportClient>();
        client.Setup(value => value.SearchForIdsAsync<BeatportReleaseDto>(
                It.IsAny<IReadOnlyDictionary<string, string?>>(), cancellationToken))
            .ThrowsAsync(new OperationCanceledException(cancellationToken));
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = CreateHandler(subscription, client.Object, unitOfWork.Object);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            handler.HandleAsync(new CollectReleasesCommand(subscription.Id.Value), cancellationToken));

        Assert.Null(subscription.RefreshedAt);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("en-US", false, "2026-11-02:2026-11-12")]
    [InlineData("uk-UA", true, "2026-10-30:2026-11-12")]
    public async Task HandleAsync_FormatsPublishDatesIndependentlyOfCulture(
        string cultureName, bool previouslyRefreshed, string expectedPublishDate)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
            var cancellationToken = TestContext.Current.CancellationToken;
            var subscription = CreateSubscription();
            if (previouslyRefreshed)
            {
                subscription.MarkRefreshed(new DateOnly(2026, 10, 30));
            }

            IReadOnlyDictionary<string, string?>? queryParameters = null;
            var client = new Mock<IBeatportClient>();
            client.Setup(value => value.SearchForIdsAsync<BeatportReleaseDto>(
                    It.IsAny<IReadOnlyDictionary<string, string?>>(), cancellationToken))
                .Callback<IReadOnlyDictionary<string, string?>, CancellationToken>((parameters, _) => queryParameters = parameters)
                .ReturnsAsync(Result.Ok<IReadOnlyCollection<int>>([]));
            var unitOfWork = new Mock<IUnitOfWork>();
            var handler = CreateHandler(subscription, client.Object, unitOfWork.Object);

            await handler.HandleAsync(new CollectReleasesCommand(subscription.Id.Value), cancellationToken);

            Assert.NotNull(queryParameters);
            Assert.Equal(expectedPublishDate, queryParameters["publish_date"]);
            Assert.Equal(new DateOnly(2026, 11, 12), subscription.RefreshedAt);
            unitOfWork.Verify(value => value.SaveChangesAsync(cancellationToken), Times.Once);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Theory]
    [InlineData(true, "2026-11-05:2026-11-12")]
    [InlineData(false, "2026-11-02:2026-11-12")]
    public async Task HandleAsync_RecordsTheDateUsedForCollectionWhenUtcDayChanges(bool previouslyRefreshed, string expectedPublishDate)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var subscription = CreateSubscription();
        if (previouslyRefreshed)
        {
            subscription.MarkRefreshed(new DateOnly(2026, 11, 5));
        }
        var clock = new Mock<IClock>();
        clock.SetupSequence(value => value.UtcToday)
            .Returns(new DateOnly(2026, 11, 12))
            .Returns(new DateOnly(2026, 11, 13));
        IReadOnlyDictionary<string, string?>? queryParameters = null;
        var client = new Mock<IBeatportClient>();
        client.Setup(value => value.SearchForIdsAsync<BeatportReleaseDto>(
                It.IsAny<IReadOnlyDictionary<string, string?>>(), cancellationToken))
            .Callback<IReadOnlyDictionary<string, string?>, CancellationToken>((parameters, _) => queryParameters = parameters)
            .ReturnsAsync(Result.Ok<IReadOnlyCollection<int>>([]));
        var handler = CreateHandler(subscription, client.Object, Mock.Of<IUnitOfWork>(), clock.Object);

        await handler.HandleAsync(new CollectReleasesCommand(subscription.Id.Value), cancellationToken);

        Assert.NotNull(queryParameters);
        Assert.Equal(expectedPublishDate, queryParameters["publish_date"]);
        Assert.Equal(new DateOnly(2026, 11, 12), subscription.RefreshedAt);
    }

    [Fact]
    public async Task HandleAsync_SkipsUnsubscribedSubscription()
    {
        var subscription = CreateSubscription();
        subscription.DecrementSubscribersCount();
        var client = new Mock<IBeatportClient>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var handler = CreateHandler(subscription, client.Object, unitOfWork.Object);

        await handler.HandleAsync(new CollectReleasesCommand(subscription.Id.Value), TestContext.Current.CancellationToken);

        Assert.Empty(client.Invocations);
        Assert.Empty(unitOfWork.Invocations);
    }

    private static Subscription CreateSubscription()
    {
        var subscription = Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow,
            SubscriptionType.Artist, BeatportId.Create(42));
        subscription.IncrementSubscribersCount();
        return subscription;
    }

    private static CollectReleasesCommandHandler CreateHandler(
        Subscription subscription, IBeatportClient client, IUnitOfWork unitOfWork, IClock? clock = null)
    {
        clock ??= Mock.Of<IClock>(value => value.UtcToday == new DateOnly(2026, 11, 12));
        var subscriptions = new Mock<ISubscriptionCommandRepository>();
        subscriptions.Setup(value => value.FindAsync(subscription.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        var releases = new Mock<IReleaseCommandRepository>();
        releases.Setup(value => value.FindAllAsync(It.IsAny<IEnumerable<BeatportId>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        return new CollectReleasesCommandHandler(
            clock,
            client,
            releases.Object,
            subscriptions.Object,
            Mock.Of<IIntegrationEventOutbox>(),
            unitOfWork,
            new UnusedSubscriptionService(),
            NullLogger<CollectReleasesCommand>.Instance);
    }

    private sealed class UnusedSubscriptionService : ISubscriptionService
    {
        public Task<Result> EnsureSubscriptionExistsAsync(
            SubscriptionType subscriptionType,
            BeatportId beatportId,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("No subscriptions should be created when there are no releases.");
    }
}
