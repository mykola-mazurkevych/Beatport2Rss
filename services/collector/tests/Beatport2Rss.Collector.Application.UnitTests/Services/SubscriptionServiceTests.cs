using System.Net;

using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Application.Services;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Models;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.FluentResults.Errors;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using Moq;

using Xunit;

namespace Beatport2Rss.Collector.Application.UnitTests.Services;

public sealed class SubscriptionServiceTests
{
    [Theory]
    [InlineData(SubscriptionType.Artist)]
    [InlineData(SubscriptionType.Label)]
    public async Task EnsureSubscriptionExistsAsync_WhenCalledTwice_ShouldCreateOneSubscriptionAndMetadataEvent(SubscriptionType type)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var beatportId = BeatportId.Create(42);
        var now = new DateTimeOffset(2026, 11, 11, 7, 0, 0, TimeSpan.Zero);
        var image = new BeatportImageDto(new Uri("https://example.com/image.jpg"));
        var website = new Uri("https://example.com/");
        var clock = new Mock<IClock>();
        clock.SetupGet(value => value.UtcNow).Returns(now);
        var client = new Mock<IBeatportClient>(MockBehavior.Strict);
        if (type == SubscriptionType.Artist)
        {
            client.Setup(value => value.GetAsync<BeatportArtistDto>(42, cancellationToken))
                .ReturnsAsync(Result.Ok<BeatportArtistDto?>(new BeatportArtistDto(42, "slug", "Name", image, "Bio", website)));
        }
        else
        {
            client.Setup(value => value.GetAsync<BeatportLabelDto>(42, cancellationToken))
                .ReturnsAsync(Result.Ok<BeatportLabelDto?>(new BeatportLabelDto(42, "slug", "Name", image)));
        }

        var subscriptions = new List<Subscription>();
        var repository = new Mock<ISubscriptionCommandRepository>(MockBehavior.Strict);
        repository.Setup(value => value.ExistsAsync(type, beatportId, cancellationToken))
            .ReturnsAsync(() => subscriptions.Count != 0);
        repository.Setup(value => value.AddAsync(It.IsAny<Subscription>(), cancellationToken))
            .Callback<Subscription, CancellationToken>((subscription, _) => subscriptions.Add(subscription))
            .ReturnsAsync((Subscription subscription, CancellationToken _) => subscription);
        var events = new List<SubscriptionCreated>();
        var outbox = new Mock<IIntegrationEventOutbox>(MockBehavior.Strict);
        outbox.Setup(value => value.EnqueueAsync(It.IsAny<SubscriptionCreated>(), cancellationToken))
            .Callback<SubscriptionCreated, CancellationToken>((message, _) => events.Add(message))
            .Returns(Task.CompletedTask);
        var service = new SubscriptionService(clock.Object, client.Object, repository.Object, outbox.Object);

        var result = await service.EnsureSubscriptionExistsAsync(type, beatportId, cancellationToken);
        var repeatedResult = await service.EnsureSubscriptionExistsAsync(type, beatportId, cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.True(repeatedResult.IsSuccess);
        var subscription = Assert.Single(subscriptions);
        Assert.Equal(type, subscription.Type);
        Assert.Equal(beatportId, subscription.BeatportId);
        Assert.Equal(now, subscription.CreatedAt);
        Assert.Equal(0, subscription.SubscribersCount);
        Assert.Null(subscription.RefreshedAt);
        var message = Assert.Single(events);
        Assert.NotEqual(Guid.Empty, message.Id);
        Assert.NotEqual(Guid.Empty, subscription.Id.Value);
        Assert.Equal(subscription.Id.Value, message.SubscriptionId);
        Assert.Equal(type, message.Type);
        Assert.Equal(42, message.BeatportId);
        Assert.Equal("slug", message.BeatportSlug);
        Assert.Equal("Name", message.Name);
        Assert.Equal(image.Uri, message.ImageUri);
        Assert.Equal(now, message.OccurredAt);
        Assert.Equal(type == SubscriptionType.Artist ? website : null, message.Website);
        Assert.Equal(type == SubscriptionType.Artist ? "Bio" : null, message.Bio);
        client.VerifyAll();
        Assert.Single(client.Invocations);
    }

    [Fact]
    public async Task EnsureSubscriptionExistsAsync_WhenExisting_ShouldSkipBeatportAndOutbox()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ISubscriptionCommandRepository>(MockBehavior.Strict);
        repository.Setup(value => value.ExistsAsync(SubscriptionType.Label, BeatportId.Create(42), cancellationToken)).ReturnsAsync(true);
        var client = new Mock<IBeatportClient>(MockBehavior.Strict);
        var outbox = new Mock<IIntegrationEventOutbox>(MockBehavior.Strict);
        var service = new SubscriptionService(Mock.Of<IClock>(), client.Object, repository.Object, outbox.Object);

        var result = await service.EnsureSubscriptionExistsAsync(SubscriptionType.Label, BeatportId.Create(42), cancellationToken);

        Assert.True(result.IsSuccess);
        repository.VerifyAll();
        repository.VerifyNoOtherCalls();
        Assert.Empty(client.Invocations);
        Assert.Empty(outbox.Invocations);
    }

    [Theory]
    [InlineData(SubscriptionType.Artist)]
    [InlineData(SubscriptionType.Label)]
    public async Task EnsureSubscriptionExistsAsync_WhenBeatportFails_ShouldPreserveErrorsWithoutCreatingSubscription(SubscriptionType type)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ISubscriptionCommandRepository>(MockBehavior.Strict);
        repository.Setup(value => value.ExistsAsync(type, BeatportId.Create(42), cancellationToken)).ReturnsAsync(false);
        var errors = new IError[]
        {
            new TransientError("Unavailable"),
            new TransientHttpError("Unavailable", HttpStatusCode.ServiceUnavailable),
            new RequestTimeoutError("Timeout"),
            new TooManyRequestsError("Rate limited"),
            new NotFoundError("Missing")
        };
        var client = new Mock<IBeatportClient>(MockBehavior.Strict);
        client.Setup(value => value.GetAsync<BeatportArtistDto>(42, cancellationToken)).ReturnsAsync(Result.Fail<BeatportArtistDto?>(errors));
        client.Setup(value => value.GetAsync<BeatportLabelDto>(42, cancellationToken)).ReturnsAsync(Result.Fail<BeatportLabelDto?>(errors));
        var outbox = new Mock<IIntegrationEventOutbox>(MockBehavior.Strict);
        var service = new SubscriptionService(Mock.Of<IClock>(), client.Object, repository.Object, outbox.Object);

        var result = await service.EnsureSubscriptionExistsAsync(type, BeatportId.Create(42), cancellationToken);

        Assert.True(result.IsFailed);
        Assert.Equal(errors.Length, result.Errors.Count);
        for (var index = 0; index < errors.Length; index++)
        {
            Assert.Same(errors[index], result.Errors[index]);
        }

        repository.Verify(value => value.AddAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Empty(outbox.Invocations);
    }

    [Theory]
    [InlineData(SubscriptionType.Artist)]
    [InlineData(SubscriptionType.Label)]
    public async Task EnsureSubscriptionExistsAsync_WhenBeatportReturnsNull_ShouldReturnNotFound(SubscriptionType type)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var repository = new Mock<ISubscriptionCommandRepository>(MockBehavior.Strict);
        repository.Setup(value => value.ExistsAsync(type, BeatportId.Create(42), cancellationToken)).ReturnsAsync(false);
        var client = new Mock<IBeatportClient>(MockBehavior.Strict);
        client.Setup(value => value.GetAsync<BeatportArtistDto>(42, cancellationToken)).ReturnsAsync(Result.Ok<BeatportArtistDto?>(null));
        client.Setup(value => value.GetAsync<BeatportLabelDto>(42, cancellationToken)).ReturnsAsync(Result.Ok<BeatportLabelDto?>(null));
        var outbox = new Mock<IIntegrationEventOutbox>(MockBehavior.Strict);
        var service = new SubscriptionService(Mock.Of<IClock>(), client.Object, repository.Object, outbox.Object);

        var result = await service.EnsureSubscriptionExistsAsync(type, BeatportId.Create(42), cancellationToken);

        Assert.True(result.IsFailed);
        Assert.IsType<NotFoundError>(Assert.Single(result.Errors));
        repository.Verify(value => value.AddAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Empty(outbox.Invocations);
    }
}