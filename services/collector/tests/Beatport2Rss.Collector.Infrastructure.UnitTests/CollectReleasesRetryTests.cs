using System.Collections.Concurrent;

using Beatport2Rss.Collector.Application;
using Beatport2Rss.Collector.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Collector.Application.UseCases.Releases.Commands;
using Beatport2Rss.Collector.Domain.Subscriptions;
using Beatport2Rss.Collector.Infrastructure.Persistence;
using Beatport2Rss.Collector.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Collector.Jobs;
using Beatport2Rss.Collector.Jobs.Jobs;
using Beatport2Rss.Common.Beatport.Interfaces;
using Beatport2Rss.Common.Beatport.Models;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.FluentResults.Extensions;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Releases;
using Beatport2Rss.Common.IntegrationEvents.Subscriptions;
using Beatport2Rss.Common.Miscellaneous.Exceptions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using JasperFx.CodeGeneration.Model;
using JasperFx.Resources;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using Npgsql;

using Quartz;

using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Postgresql;

using Xunit;

namespace Beatport2Rss.Collector.Infrastructure.UnitTests;

public sealed class CollectReleasesRetryTests
{
    [Fact]
    public async Task SubscriptionRequestedCreatesSubscriptionThroughWolverine()
    {
        var client = new Mock<IBeatportClient>();
        client.Setup(value => value.GetAsync<BeatportArtistDto>(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok<BeatportArtistDto?>(new BeatportArtistDto(
                42, "artist", "Artist", new BeatportImageDto(new Uri("https://example.com/image")), null, null)));

        using var host = await CreateHostAsync(client.Object);
        var cancellationToken = TestContext.Current.CancellationToken;
        await host.Services.GetRequiredService<IMessageBus>().SendAsync(new SubscriptionRequested(
            Guid.NewGuid(), DateTimeOffset.UtcNow, SubscriptionType.Artist, 42));
        var committedEvents = host.Services.GetRequiredService<CommittedEvents>();
        await committedEvents.Completed.Task.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken);

        await using var scope = host.Services.CreateAsyncScope();
        var subscription = await scope.ServiceProvider.GetRequiredService<CollectorDbContext>()
            .Subscriptions.SingleAsync(cancellationToken);
        var subscriptionCreated = Assert.Single(committedEvents.Events.OfType<SubscriptionCreated>());
        Assert.Equal(42, subscription.BeatportId.Value);
        Assert.Equal(subscription.Id.Value, subscriptionCreated.SubscriptionId);
    }

    [Fact]
    public async Task CollectionJobUsesUtcForItsSchedule()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CollectorJobOptions:CollectReleasesCronSchedule"] = "0 0 3 ? * FRI"
            })
            .Build();
        using var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services => services.AddCollectorJobs(configuration))
            .Build();
        var cancellationToken = TestContext.Current.CancellationToken;
        await host.StartAsync(cancellationToken);
        var scheduler = await host.Services.GetRequiredService<ISchedulerFactory>().GetScheduler(cancellationToken);
        var triggers = await scheduler.GetTriggersOfJob(new JobKey(nameof(CollectReleasesJob)), cancellationToken);

        var trigger = Assert.IsAssignableFrom<ICronTrigger>(Assert.Single(triggers));
        Assert.Equal(TimeZoneInfo.Utc, trigger.TimeZone);
        await host.StopAsync(cancellationToken);
    }

    [Fact]
    public async Task QueuedSubscriptionsArePersistedWhileProcessingIsBlocked()
    {
        var connectionString = Environment.GetEnvironmentVariable("COLLECTOR_TEST_POSTGRES");
        Assert.SkipWhen(string.IsNullOrEmpty(connectionString), "COLLECTOR_TEST_POSTGRES is not configured.");
        var firstStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var requests = 0;
        var client = new Mock<IBeatportClient>();
        client.Setup(value => value.SearchForIdsAsync<BeatportReleaseDto>(
                It.IsAny<IReadOnlyDictionary<string, string?>>(), It.IsAny<CancellationToken>()))
            .Returns(async (IReadOnlyDictionary<string, string?> _, CancellationToken token) =>
            {
                if (Interlocked.Increment(ref requests) == 1)
                {
                    firstStarted.TrySetResult();
                    await release.Task.WaitAsync(token);
                }
                else
                {
                    secondStarted.TrySetResult();
                }
                return Result.Ok<IReadOnlyCollection<int>>([]);
            });

        using var host = await CreateHostAsync(client.Object, connectionString);
        var cancellationToken = TestContext.Current.CancellationToken;
        await SeedAsync(host.Services, includeSecondSubscription: true, cancellationToken);

        try
        {
            await ExecuteJobAsync(host.Services, cancellationToken);
            await firstStarted.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            long count = 0;
            for (var i = 0; i < 50 && count < 2; i++)
            {
                await using var query = new NpgsqlCommand(
                    "SELECT count(*) FROM collector_test_messaging.wolverine_incoming_envelopes WHERE message_type LIKE '%CollectReleasesCommand%'",
                    connection);
                count = (long)(await query.ExecuteScalarAsync(cancellationToken))!;
                if (count < 2)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                }
            }

            Assert.Equal(2, count);
            Assert.Equal(1, requests);
        }
        finally
        {
            release.TrySetResult();
        }
        await secondStarted.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
        await host.StopAsync(cancellationToken);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task RetriesWholeSubscriptionWithoutRetainingPartialAttempt(int failureType)
    {
        var client = new Mock<IBeatportClient>();
        client.Setup(value => value.SearchForIdsAsync<BeatportReleaseDto>(
                It.IsAny<IReadOnlyDictionary<string, string?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok<IReadOnlyCollection<int>>([1, 2]));
        var secondReleaseRequests = 0;
        client.Setup(value => value.GetAsync<BeatportReleaseDto>(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) =>
            {
                if (id == 2 && Interlocked.Increment(ref secondReleaseRequests) == 1)
                {
                    if (failureType == 1)
                    {
                        throw new HttpRequestException("Network error");
                    }
                    if (failureType == 2)
                    {
                        throw new TaskCanceledException("Timeout");
                    }
                    return Result.Fail<BeatportReleaseDto?>(Result.TooManyRequests("Rate limited").Errors);
                }

                return Result.Ok<BeatportReleaseDto?>(CreateRelease(id));
            });

        using var host = await CreateHostAsync(client.Object);
        var cancellationToken = TestContext.Current.CancellationToken;
        await SeedAsync(host.Services, includeSecondSubscription: false, cancellationToken);

        await ExecuteJobAsync(host.Services, cancellationToken);
        await host.Services.GetRequiredService<CommittedEvents>().Completed.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);

        await using var scope = host.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<CollectorDbContext>();
        var subscription = await db.Subscriptions.SingleAsync(value => value.SubscribersCount > 0, cancellationToken);
        Assert.Equal(new DateOnly(2026, 11, 12), subscription.RefreshedAt);
        Assert.Equal(2, await db.Releases.CountAsync(cancellationToken));
        Assert.Equal(2, host.Services.GetRequiredService<CommittedEvents>().Events.OfType<ReleaseCollected>().Count());
        Assert.Equal(2, secondReleaseRequests);
        client.Verify(value => value.SearchForIdsAsync<BeatportReleaseDto>(
            It.IsAny<IReadOnlyDictionary<string, string?>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ExhaustedRetriesLeaveDateUnchangedAndNextSubscriptionIsProcessed()
    {
        var requests = new ConcurrentQueue<string>();
        var client = new Mock<IBeatportClient>();
        client.Setup(value => value.SearchForIdsAsync<BeatportReleaseDto>(
                It.IsAny<IReadOnlyDictionary<string, string?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyDictionary<string, string?> parameters, CancellationToken _) =>
            {
                var artistId = parameters["artist_id"]!;
                requests.Enqueue(artistId);
                return artistId == "42"
                    ? Result.Fail<IReadOnlyCollection<int>>(Result.TooManyRequests("Rate limited").Errors)
                    : Result.Ok<IReadOnlyCollection<int>>([]);
            });

        using var host = await CreateHostAsync(client.Object);
        var cancellationToken = TestContext.Current.CancellationToken;
        await SeedAsync(host.Services, includeSecondSubscription: true, cancellationToken);

        await ExecuteJobAsync(host.Services, cancellationToken);
        await host.Services.GetRequiredService<CommittedEvents>().Completed.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);

        await using var scope = host.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<CollectorDbContext>();
        var subscriptions = await db.Subscriptions.Where(value => value.SubscribersCount > 0).ToListAsync(cancellationToken);
        Assert.Equal(new DateOnly(2026, 11, 1), subscriptions.Single(value => value.BeatportId.Value == 42).RefreshedAt);
        Assert.Equal(new DateOnly(2026, 11, 12), subscriptions.Single(value => value.BeatportId.Value == 43).RefreshedAt);
        Assert.Equal(["42", "42", "42", "42", "42", "42", "43"], requests.ToArray());
    }

    private static async Task<IHost> CreateHostAsync(IBeatportClient client, string? connectionString = null)
    {
        var databaseName = Guid.NewGuid().ToString();
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddApplication();
                services.AddSingleton(client);
                services.AddSingleton<IClock, TestClock>();
                services.AddSingleton<CommittedEvents>();
                services.AddDbContext<CollectorDbContext>(options =>
                {
                    if (connectionString is null)
                    {
                        options.UseInMemoryDatabase(databaseName);
                    }
                    else
                    {
                        options.UseNpgsql(connectionString);
                    }
                });
                services.AddTransient(provider => provider.GetRequiredService<CollectorDbContext>().Subscriptions);
                services.AddTransient(provider => provider.GetRequiredService<CollectorDbContext>().Releases);
                services.AddTransient<ISubscriptionCommandRepository, SubscriptionCommandRepository>();
                services.AddTransient<IReleaseCommandRepository, ReleaseCommandRepository>();
                services.AddScoped<TestOutbox>();
                services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<TestOutbox>());
                services.AddScoped<IIntegrationEventOutbox>(provider => provider.GetRequiredService<TestOutbox>());
                services.AddWolverine(options =>
                {
                    options.ServiceLocationPolicy = ServiceLocationPolicy.AllowedButWarn;
                    options.Discovery.IncludeAssembly(typeof(CollectorApplication).Assembly);
                    var queue = options.LocalQueueFor<CollectReleasesCommand>().Sequential();
                    if (connectionString is not null)
                    {
                        options.PersistMessagesWithPostgresql(connectionString, "collector_test_messaging");
                        options.Policies.UseDurableLocalQueues();
                        queue.UseDurableInbox();
                    }
                    options.OnException<HttpRequestException>().RetryWithCooldown(
                        TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10),
                        TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10));
                    options.OnException<RetryableException>().RetryWithCooldown(
                        TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10),
                        TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10));
                });
                if (connectionString is not null)
                {
                    services.AddResourceSetupOnStartup();
                }
            })
            .Build();
        if (connectionString is not null)
        {
            await using var scope = host.Services.CreateAsyncScope();
            await scope.ServiceProvider.GetRequiredService<CollectorDbContext>().Database.EnsureCreatedAsync();
        }
        await host.StartAsync();
        return host;
    }

    private static async Task SeedAsync(IServiceProvider services, bool includeSecondSubscription, CancellationToken cancellationToken)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<CollectorDbContext>();
        var subscription = Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow,
            SubscriptionType.Artist, BeatportId.Create(42));
        subscription.IncrementSubscribersCount();
        subscription.MarkRefreshed(new DateOnly(2026, 11, 1));
        db.Subscriptions.Add(subscription);
        if (includeSecondSubscription)
        {
            var secondSubscription = Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow,
                SubscriptionType.Artist, BeatportId.Create(43));
            secondSubscription.IncrementSubscribersCount();
            secondSubscription.MarkRefreshed(new DateOnly(2026, 11, 2));
            db.Subscriptions.Add(secondSubscription);
        }
        db.Subscriptions.Add(Subscription.Create(SubscriptionId.Create(Guid.NewGuid()), DateTimeOffset.UtcNow,
            SubscriptionType.Label, BeatportId.Create(10)));
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task ExecuteJobAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        await using var scope = services.CreateAsyncScope();
        var job = ActivatorUtilities.CreateInstance<CollectReleasesJob>(scope.ServiceProvider);
        await job.Execute(Mock.Of<IJobExecutionContext>(), cancellationToken);
    }

    private static BeatportReleaseDto CreateRelease(int id) =>
        new(id, $"release-{id}", [], [], new BeatportReleaseLabelDto(10, "label"), "catalog",
            "release", null, new DateOnly(2026, 11, 12), new DateOnly(2026, 11, 12),
            new BeatportImageDto(new Uri("https://example.com/image")), 0, []);

    private sealed class TestClock : IClock
    {
        public DateTimeOffset UtcNow => new(2026, 11, 12, 3, 0, 0, TimeSpan.Zero);
        public DateOnly UtcToday => new(2026, 11, 12);
    }

    private sealed class CommittedEvents
    {
        public ConcurrentBag<IIntegrationEvent> Events { get; } = [];
        public TaskCompletionSource Completed { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private sealed class TestOutbox(CollectorDbContext db, CommittedEvents committedEvents) :
        IUnitOfWork, IIntegrationEventOutbox
    {
        private readonly List<IIntegrationEvent> _pendingEvents = [];

        public Task EnqueueAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
            where TEvent : IIntegrationEvent
        {
            _pendingEvents.Add(integrationEvent);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await db.SaveChangesAsync(cancellationToken);
            foreach (var integrationEvent in _pendingEvents)
            {
                committedEvents.Events.Add(integrationEvent);
            }
            _pendingEvents.Clear();
            committedEvents.Completed.TrySetResult();
        }
    }
}
