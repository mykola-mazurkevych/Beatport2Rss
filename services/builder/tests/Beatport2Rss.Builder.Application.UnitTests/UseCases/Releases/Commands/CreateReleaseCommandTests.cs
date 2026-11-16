using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.UseCases.Releases.Commands;
using Beatport2Rss.Builder.Application.UseCases.Releases.IntegrationEvents;
using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Common.Beatport;
using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents.Releases;
using Beatport2Rss.Common.Miscellaneous.Exceptions;
using Beatport2Rss.Common.Miscellaneous;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using JasperFx.CodeGeneration.Model;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Moq;

using Wolverine;
using Wolverine.ErrorHandling;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests.UseCases.Releases.Commands;

public sealed class CreateReleaseCommandTests
{
    [Theory]
    [InlineData(10, SubscriptionType.Label)]
    [InlineData(11, SubscriptionType.Artist)]
    [InlineData(12, SubscriptionType.Artist)]
    [InlineData(13, SubscriptionType.Artist)]
    [InlineData(14, SubscriptionType.Artist)]
    public async Task MissingSubscriptionIsRetryable(int missingId, SubscriptionType missingType)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var subscriptions = new Mock<ISubscriptionCommandRepository>();
        subscriptions.Setup(value => value.FindAllAsync(It.IsAny<IEnumerable<BeatportId>>(), cancellationToken))
            .ReturnsAsync(AllSubscriptions().Where(subscription => subscription.BeatportId.Value != missingId));
        var releases = new Mock<IReleaseCommandRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var command = ReleaseCollectedHandler.Handle(CreateEvent(includeArtists: true));
        var handler = new CreateReleaseCommandHandler(
            Mock.Of<IClock>(),
            Mock.Of<IBeatportUriBuilder>(),
            releases.Object,
            subscriptions.Object,
            unitOfWork.Object);

        var exception = await Assert.ThrowsAsync<RetryableException>(() =>
            handler.HandleAsync(command, cancellationToken));

        Assert.Contains($"{missingType} {missingId}", exception.Message, StringComparison.Ordinal);
        releases.Verify(value => value.AddAsync(It.IsAny<Release>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExistingReleaseIsNotAddedAgain()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var command = ReleaseCollectedHandler.Handle(CreateEvent(includeArtists: false));
        var releases = new Mock<IReleaseCommandRepository>(MockBehavior.Strict);
        var subscriptions = new Mock<ISubscriptionCommandRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        releases.Setup(value => value.ExistsAsync(command.ReleaseId, cancellationToken)).ReturnsAsync(true);
        var handler = new CreateReleaseCommandHandler(
            Mock.Of<IClock>(),
            Mock.Of<IBeatportUriBuilder>(),
            releases.Object,
            subscriptions.Object,
            unitOfWork.Object);

        await handler.HandleAsync(command, cancellationToken);

        releases.Verify(value => value.AddAsync(It.IsAny<Release>(), It.IsAny<CancellationToken>()), Times.Never);
        subscriptions.Verify(
            value => value.FindAllAsync(It.IsAny<IEnumerable<BeatportId>>(), It.IsAny<CancellationToken>()),
            Times.Never);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task DeleteStaleReleasesCommand_InvalidRetentionInDays_Throws(int retentionInDays)
    {
        var handler = new DeleteStaleReleasesCommandHandler(
            Mock.Of<IClock>(),
            Mock.Of<IReleaseCommandRepository>());

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            handler.Handle(new DeleteStaleReleasesCommand(retentionInDays), CancellationToken.None));
    }

    [Theory]
    [InlineData(false, 3)]
    [InlineData(true, 6)]
    public async Task WolverineRetriesUntilSubscriptionArrivesOrAttemptsAreExhausted(bool alwaysMissing, int expectedAttempts)
    {
        var attempts = 0;
        var lastAttempt = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var saved = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var subscriptions = new Mock<ISubscriptionCommandRepository>();
        subscriptions.Setup(value => value.FindAllAsync(It.IsAny<IEnumerable<BeatportId>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var attempt = Interlocked.Increment(ref attempts);
                if (attempt == expectedAttempts)
                {
                    lastAttempt.TrySetResult();
                }
                return alwaysMissing || attempt < expectedAttempts ? [] : AllSubscriptions();
            });
        var releases = new Mock<IReleaseCommandRepository>();
        releases.Setup(value => value.AddAsync(It.IsAny<Release>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Release release, CancellationToken _) => release);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => saved.TrySetResult())
            .Returns(Task.CompletedTask);
        using var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(configuration => configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["BeatportOptions:ApiV4BaseUri"] = "https://api.beatport.com/v4/catalog",
                    ["BeatportOptions:WebBaseUri"] = "https://www.beatport.com"
                }))
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton(subscriptions.Object);
                services.AddSingleton(releases.Object);
                services.AddSingleton(unitOfWork.Object);
                services.AddBeatportClient(context.Configuration);
                services.AddMiscellaneous();
                services.AddWolverine(options =>
                {
                    options.ServiceLocationPolicy = ServiceLocationPolicy.AllowedButWarn;
                    options.Discovery.IncludeAssembly(typeof(CreateReleaseCommand).Assembly);
                    options.OnException<RetryableException>().RetryWithCooldown(
                        TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10),
                        TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10));
                });
            })
            .Build();
        var cancellationToken = TestContext.Current.CancellationToken;
        await host.StartAsync(cancellationToken);
        await host.Services.GetRequiredService<IMessageBus>().SendAsync(CreateEvent(includeArtists: false));
        await lastAttempt.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
        if (!alwaysMissing)
        {
            await saved.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
        }
        await host.StopAsync(cancellationToken);

        Assert.Equal(expectedAttempts, attempts);
        releases.Verify(value => value.AddAsync(It.IsAny<Release>(), It.IsAny<CancellationToken>()),
            alwaysMissing ? Times.Never : Times.Once);
        if (!alwaysMissing)
        {
            releases.Verify(value => value.AddAsync(
                It.Is<Release>(release => release.BeatportUri == new Uri("https://www.beatport.com/release/100")),
                It.IsAny<CancellationToken>()), Times.Once);
        }
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()),
            alwaysMissing ? Times.Never : Times.Once);
    }

    private static ReleaseCollected CreateEvent(bool includeArtists) =>
        new(Guid.NewGuid(), DateTimeOffset.UtcNow, Guid.NewGuid(), 100, "release",
            includeArtists ? [11] : [], includeArtists ? [12] : [], 10, "catalog", "Release", null,
            new DateOnly(2026, 11, 12), new Uri("https://example.com/image"), includeArtists ? 1 : 0,
            includeArtists
                ? [new ReleaseCollectedTrack(101, 1, "Track", "Original Mix", [13], [14],
                    TimeSpan.FromSeconds(30), new Uri("https://example.com/sample"))]
                : []);

    private static Subscription[] AllSubscriptions() =>
    [
        CreateSubscription(10, SubscriptionType.Label),
        CreateSubscription(11, SubscriptionType.Artist),
        CreateSubscription(12, SubscriptionType.Artist),
        CreateSubscription(13, SubscriptionType.Artist),
        CreateSubscription(14, SubscriptionType.Artist)
    ];

    private static Subscription CreateSubscription(int beatportId, SubscriptionType type) =>
        Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow, type,
            SubscriptionName.Create("Subscription"), BeatportId.Create(beatportId));
}
