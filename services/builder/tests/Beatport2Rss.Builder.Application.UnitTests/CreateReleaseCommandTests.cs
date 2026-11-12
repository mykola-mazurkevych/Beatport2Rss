using Beatport2Rss.Builder.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Builder.Application.UseCases.Releases.Commands;
using Beatport2Rss.Builder.Application.UseCases.Releases.IntegrationEvents;
using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Builder.Domain.Subscriptions;
using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents.Releases;
using Beatport2Rss.Common.Miscellaneous.Exceptions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using JasperFx.CodeGeneration.Model;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using Wolverine;
using Wolverine.ErrorHandling;

using Xunit;

namespace Beatport2Rss.Builder.Application.UnitTests;

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

        var exception = await Assert.ThrowsAsync<RetryableException>(() => CreateReleaseCommandHandler.HandleAsync(
            command, Mock.Of<IClock>(), Mock.Of<IBeatportUriBuilder>(), releases.Object, subscriptions.Object,
            unitOfWork.Object, cancellationToken));

        Assert.Contains($"{missingType} {missingId}", exception.Message, StringComparison.Ordinal);
        releases.Verify(value => value.AddAsync(It.IsAny<Release>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
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
        var uriBuilder = new Mock<IBeatportUriBuilder>();
        uriBuilder.Setup(value => value.Build(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(new Uri("https://example.com/release"));

        using var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton(subscriptions.Object);
                services.AddSingleton(releases.Object);
                services.AddSingleton(unitOfWork.Object);
                services.AddSingleton(uriBuilder.Object);
                services.AddSingleton(Mock.Of<IClock>());
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
