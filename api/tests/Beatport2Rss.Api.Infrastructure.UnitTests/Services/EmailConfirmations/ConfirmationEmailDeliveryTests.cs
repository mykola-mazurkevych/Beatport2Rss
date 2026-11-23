using System.Net;
using System.Text;
using System.Text.Json;

using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailConfirmations;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Api.Infrastructure.Extensions.EmailConfirmations;
using Beatport2Rss.Api.Infrastructure.Services.EmailConfirmations;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.IntegrationEvents.Users;
using Beatport2Rss.Common.Miscellaneous.Exceptions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;
using Beatport2Rss.Common.Wolverine.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using Resend;

using Wolverine;
using Wolverine.RabbitMQ;

using Xunit;

using DeliveryConfirmationOptions = Beatport2Rss.Api.Infrastructure.Options.EmailConfirmationOptions;

namespace Beatport2Rss.Api.Infrastructure.UnitTests.Services.EmailConfirmations;

public sealed class ConfirmationEmailDeliveryTests
{
    [Fact]
    public async Task UserCreatedIsPublishedToBothLocalQueueAndRabbitMq()
    {
        using var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddOptions<EmailConfirmationOptions>();
                services.AddWolverine(options =>
                {
                    options.UseRabbitMq();
                    options.Publishes<UserCreated>();
                    options.ConfigureConfirmationEmailDelivery();
                    options.StubAllExternalTransports();
                });
            })
            .Build();
        var cancellationToken = TestContext.Current.CancellationToken;
        await host.StartAsync(cancellationToken);

        var bus = host.Services.GetRequiredService<IMessageBus>();
        var message = new UserCreated(Guid.NewGuid(), DateTimeOffset.UtcNow, Guid.NewGuid());
        var destinations = bus.PreviewSubscriptions(message)
            .Select(subscription => subscription.Destination?.AbsoluteUri)
            .ToArray();
        Assert.Equal(2, destinations.Length);
        Assert.Contains("local://user-created-confirmation/", destinations);
        Assert.Contains(destinations, destination => destination is not null && destination.StartsWith("rabbitmq:", StringComparison.Ordinal));

        await host.StopAsync(cancellationToken);
    }

    [Fact]
    public async Task WolverineRoutesConfirmationToTheApiLocalQueueAndResolvesItsInternalHandler()
    {
        using var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddOptions<EmailConfirmationOptions>();
                services.AddConfirmationEmailDelivery();
                services.AddWolverine(options => options.ConfigureConfirmationEmailDelivery());
            })
            .Build();
        var cancellationToken = TestContext.Current.CancellationToken;
        await host.StartAsync(cancellationToken);

        var bus = host.Services.GetRequiredService<IMessageBus>();
        var message = new SendConfirmationEmail(EmailConfirmationIdentifier.Generate().ToString());
        var destination = Assert.Single(bus.PreviewSubscriptions(message)).Destination;
        Assert.Equal("local://confirmation-email-delivery/", destination?.AbsoluteUri);
        await bus.InvokeAsync(message, cancellationToken);

        await host.StopAsync(cancellationToken);
    }

    [Fact]
    public void DurableMessageRoundTripsWithoutExposingIdentifierInToString()
    {
        var identifier = EmailConfirmationIdentifier.Generate().ToString();
        var message = new SendConfirmationEmail(identifier);
        var payload = JsonSerializer.SerializeToUtf8Bytes(message);
        var restored = JsonSerializer.Deserialize<SendConfirmationEmail>(payload);

        Assert.NotNull(restored);
        Assert.Equal(identifier, restored.Identifier);
        Assert.DoesNotContain(identifier, message.ToString(), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("disabled")]
    [InlineData("invalid")]
    [InlineData("missing")]
    [InlineData("expired")]
    [InlineData("confirmed")]
    public async Task UndeliverableMessageDoesNotResolveProvider(string scenario)
    {
        var identifier = EmailConfirmationIdentifier.Generate();
        var clock = new FixedClock();
        var (user, confirmation) = CreateConfirmation(identifier, clock, expired: scenario == "expired");
        if (scenario == "confirmed")
        {
            Assert.True(confirmation.TryConfirm(user, clock.UtcNow));
        }

        await using var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IClock>(clock)
            .AddSingleton<IOptions<EmailConfirmationOptions>>(
                new OptionsWrapper<EmailConfirmationOptions>(new EmailConfirmationOptions { Enabled = scenario != "disabled" }))
            .AddSingleton<IEmailConfirmationQueryRepository>(new ConfirmationRepository(scenario == "missing" ? null : confirmation))
            .BuildServiceProvider();

        var message = new SendConfirmationEmail(scenario == "invalid" ? "invalid" : identifier.ToString());
        await SendConfirmationEmailHandler.DeliverAsync(message, services, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task SdkRequestHasStableIdempotencyKeyAndBothMessageFormats()
    {
        var identifier = EmailConfirmationIdentifier.Generate();
        var (_, confirmation) = CreateConfirmation(identifier, new FixedClock());
        using var transport = new FakeResendTransport(HttpStatusCode.OK, """{"id":"c3dcb630-9e41-4f90-bccc-87267425c12e"}""");
        using var client = new HttpClient(transport);
        var sender = CreateSender(client);

        await sender.SendAsync(confirmation, TestContext.Current.CancellationToken);
        await sender.SendAsync(confirmation, TestContext.Current.CancellationToken);

        var expectedIdempotencyKey = $"email-confirmation/{confirmation.Id.Value:D}";
        Assert.Equal([expectedIdempotencyKey, expectedIdempotencyKey], transport.Keys);
        Assert.NotNull(transport.Body);
        using var payload = JsonDocument.Parse(transport.Body);
        var expectedLink = $"https://example.test/base/confirm-email?token={identifier}";
        Assert.Equal(confirmation.EmailAddress.Value, payload.RootElement.GetProperty("to")[0].GetString());
        Assert.Equal("Beatport2Rss <noreply@example.test>", payload.RootElement.GetProperty("from").GetString());
        var html = payload.RootElement.GetProperty("html").GetString();
        var text = payload.RootElement.GetProperty("text").GetString();
        Assert.Contains($"href=\"{expectedLink}\"", html, StringComparison.Ordinal);
        Assert.Contains(expectedLink, text, StringComparison.Ordinal);
        Assert.Contains("This link expires at 2026-11-19 12:01:00 UTC.", html, StringComparison.Ordinal);
        Assert.Contains("This link expires at 2026-11-19 12:01:00 UTC.", text, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(429, "rate_limit_exceeded", true)]
    [InlineData(503, "internal_server_error", true)]
    [InlineData(409, "concurrent_idempotent_requests", true)]
    [InlineData(409, "invalid_idempotent_request", false)]
    [InlineData(422, "validation_error", false)]
    public async Task ProviderFailuresAreClassifiedAndDoNotExposeResponseDetails(int status, string errorName, bool retry)
    {
        var identifier = EmailConfirmationIdentifier.Generate();
        var (_, confirmation) = CreateConfirmation(identifier, new FixedClock());
        using var transport = new FakeResendTransport(
            (HttpStatusCode)status, JsonSerializer.Serialize(new { statusCode = status, name = errorName, message = identifier.ToString() }));
        using var client = new HttpClient(transport);
        var sender = CreateSender(client);
        var cancellationToken = TestContext.Current.CancellationToken;

        var expectedException = retry ? typeof(RetryableException) : typeof(PermanentConfirmationEmailException);
        var failure = await Assert.ThrowsAsync(expectedException, () => sender.SendAsync(confirmation, cancellationToken));
        Assert.Null(failure.InnerException);
        Assert.DoesNotContain(identifier.ToString(), failure.ToString(), StringComparison.Ordinal);
        Assert.Single(transport.Keys);
    }

    private static ResendConfirmationEmail CreateSender(HttpClient client) =>
        new(
            new ResendClient(new ResendOptionsSnapshot(), client),
            new OptionsWrapper<DeliveryConfirmationOptions>(new DeliveryConfirmationOptions
            {
                PublicWebBaseUrl = new Uri("https://example.test/base"),
                SenderName = "Beatport2Rss",
                SenderAddress = "noreply@example.test",
            }));

    private static (User, EmailConfirmation) CreateConfirmation(EmailConfirmationId identifier, FixedClock clock, bool expired = false)
    {
        var user = User.Create(
            UserId.Create(Guid.NewGuid()),
            clock.UtcNow.AddMinutes(-2),
            Common.Domain.Common.EmailAddress.Create("user@example.test"),
            PasswordHash.Create("unused-hash"),
            null,
            null,
            UserStatus.Pending,
            null);
        var expiresAt = clock.UtcNow.AddMinutes(expired ? -1 : 1);
        return (user, EmailConfirmation.Create(
            identifier,
            user.CreatedAt,
            user.Id,
            user.EmailAddress,
            expiresAt));
    }

    private sealed class FixedClock : IClock
    {
        public DateTimeOffset UtcNow { get; } = new(2026, 11, 19, 12, 0, 0, TimeSpan.Zero);
        public DateOnly UtcToday => DateOnly.FromDateTime(UtcNow.UtcDateTime);
    }

    private sealed class ConfirmationRepository(EmailConfirmation? confirmation) : IEmailConfirmationQueryRepository
    {
        public ValueTask<EmailConfirmation?> FindAsync(EmailConfirmationId id, CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(confirmation?.Id == id ? confirmation : null);
    }

    private sealed class ResendOptionsSnapshot : IOptionsSnapshot<ResendClientOptions>
    {
        public ResendClientOptions Value { get; } = new() { ApiToken = "test-key-not-sent" };

        public ResendClientOptions Get(string? name) => Value;
    }

    private sealed class FakeResendTransport(HttpStatusCode status, string response) : HttpMessageHandler
    {
        public List<string> Keys { get; } = [];
        public string? Body { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Keys.Add(Assert.Single(request.Headers.GetValues("Idempotency-Key")));
            Body = await request.Content!.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(status) { Content = new StringContent(response, Encoding.UTF8, "application/json") };
        }
    }
}
