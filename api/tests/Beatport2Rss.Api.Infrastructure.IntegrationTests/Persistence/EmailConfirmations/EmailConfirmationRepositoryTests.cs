using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailConfirmations;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Api.Infrastructure.Extensions.EmailConfirmations;
using Beatport2Rss.Api.Infrastructure.Persistence;
using Beatport2Rss.Api.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Api.Infrastructure.Persistence.Repositories.EmailConfirmations;
using Beatport2Rss.Api.Infrastructure.Services.EmailConfirmations;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents.Users;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using JasperFx;
using JasperFx.CodeGeneration.Model;

using Moq;

using Npgsql;

using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;

using Xunit;

namespace Beatport2Rss.Api.Infrastructure.IntegrationTests.Persistence.EmailConfirmations;

public sealed class EmailConfirmationRepositoryTests
{
    private const string ConnectionVariable = "EMAIL_CONFIRMATION_TEST_POSTGRES";
    private static readonly DateTimeOffset IssuedAt = new(2026, 11, 19, 12, 0, 0, TimeSpan.Zero);

    public static bool HasPostgres => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ConnectionVariable));

    [Fact(Skip = "Set EMAIL_CONFIRMATION_TEST_POSTGRES to run against PostgreSQL.", SkipUnless = nameof(HasPostgres))]
    public async Task UserCreated_CommitsConfirmationAndDeliversPrivateMessage()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var connection = new NpgsqlConnectionStringBuilder(Environment.GetEnvironmentVariable(ConnectionVariable))
        {
            Database = $"email_confirmation_test_{Guid.NewGuid():N}",
            Pooling = false,
        };
        var dbOptions = new DbContextOptionsBuilder<ApiDbContext>().UseNpgsql(connection.ConnectionString).Options;
        await using var db = new ApiDbContext(dbOptions);
        await db.Database.EnsureCreatedAsync(cancellationToken);

        try
        {
            var user = User.Create(UserId.Create(Guid.NewGuid()), IssuedAt, EmailAddress.Create("user@example.test"),
                PasswordHash.Create("test-hash"), null, null, UserStatus.Pending, null);

            var received = new TaskCompletionSource<EmailConfirmationId>(TaskCreationOptions.RunContinuationsAsynchronously);
            var query = new Mock<IEmailConfirmationQueryRepository>(MockBehavior.Strict);
            query.Setup(value => value.FindAsync(It.IsAny<EmailConfirmationId>(), It.IsAny<CancellationToken>()))
                .Callback<EmailConfirmationId, CancellationToken>((id, _) => received.TrySetResult(id))
                .ReturnsAsync((EmailConfirmation?)null);
            using var host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning))
                .ConfigureServices(services =>
                {
                    services.AddDbContext<ApiDbContext>(options => options.UseNpgsql(connection.ConnectionString));
                    services.AddTransient(provider => provider.GetRequiredService<ApiDbContext>().Users);
                    services.AddTransient<IUserCommandRepository, UserCommandRepository>();
                    services.AddTransient<IEmailConfirmationCommandRepository, EmailConfirmationCommandRepository>();
                    services.AddSingleton<IClock>(new FixedClock(IssuedAt));
                    services.Configure<EmailConfirmationOptions>(options => options.Enabled = true);
                    services.AddSingleton(query.Object);
                    services.AddScoped<IUnitOfWork>(provider =>
                        new TestWolverineUnitOfWork(provider.GetRequiredService<IDbContextOutbox<ApiDbContext>>()));
                    services.AddConfirmationEmailDelivery();
                    services.AddWolverine(options =>
                    {
                        options.ServiceLocationPolicy = ServiceLocationPolicy.AllowedButWarn;
                        options.PersistMessagesWithPostgresql(connection.ConnectionString, "api_messaging")
                            .OverrideAutoCreateResources(AutoCreate.All);
                        options.UseEntityFrameworkCoreTransactions();
                        options.Policies.AutoApplyTransactions();
                        options.ConfigureConfirmationEmailDelivery();
                    });
                })
                .Build();
            await host.StartAsync(cancellationToken);

            var message = new UserCreated(Guid.NewGuid(), IssuedAt, user.Id.Value);
            await using (var scope = host.Services.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
                var outbox = scope.ServiceProvider.GetRequiredService<IDbContextOutbox<ApiDbContext>>();
                await context.Users.AddAsync(user, cancellationToken);
                await outbox.PublishAsync(message);
                await outbox.SaveChangesAndFlushMessagesAsync(cancellationToken);
            }

            var deliveredId = await received.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
            db.ChangeTracker.Clear();
            var confirmation = await db.EmailConfirmations.SingleAsync(cancellationToken);
            Assert.Equal(confirmation.Id, deliveredId);
            Assert.NotEqual(message.Id, confirmation.Id.Value);

            await using (var scope = host.Services.CreateAsyncScope())
            {
                await HandleUserCreatedAsync(message, scope.ServiceProvider, cancellationToken);
            }
            Assert.Equal(2, await db.EmailConfirmations.CountAsync(cancellationToken));
            await host.StopAsync(cancellationToken);
        }
        finally
        {
            await db.Database.EnsureDeletedAsync(CancellationToken.None);
        }
    }

    [Fact]
    public async Task UserCreated_ReplayedEventCreatesSeparateConfirmations()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var dbOptions = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new ApiDbContext(dbOptions);
        var user = User.Create(UserId.Create(Guid.NewGuid()), IssuedAt, EmailAddress.Create("user@example.test"),
            PasswordHash.Create("test-hash"), null, null, UserStatus.Pending, null);
        await db.Users.AddAsync(user, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var identifiers = new List<EmailConfirmationId>();
        var delivery = new Mock<IConfirmationEmailOutbox>(MockBehavior.Strict);
        delivery.Setup(value => value.EnqueueAsync(It.IsAny<EmailConfirmationId>()))
            .Callback<EmailConfirmationId>(identifiers.Add)
            .Returns(ValueTask.CompletedTask);
        var unit = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unit.Setup(value => value.SaveChangesAsync(cancellationToken))
            .Returns((Func<CancellationToken, Task>)db.SaveChangesAsync);
        await using var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton(db)
            .AddTransient(provider => provider.GetRequiredService<ApiDbContext>().Users)
            .AddTransient<IUserCommandRepository, UserCommandRepository>()
            .AddTransient<IEmailConfirmationCommandRepository, EmailConfirmationCommandRepository>()
            .AddSingleton<IClock>(new FixedClock(IssuedAt))
            .AddSingleton<IOptions<EmailConfirmationOptions>>(new OptionsWrapper<EmailConfirmationOptions>(new EmailConfirmationOptions { Enabled = true }))
            .AddSingleton(delivery.Object)
            .AddSingleton(unit.Object)
            .BuildServiceProvider();
        var message = new UserCreated(Guid.NewGuid(), IssuedAt, user.Id.Value);

        await HandleUserCreatedAsync(message, services, cancellationToken);
        await HandleUserCreatedAsync(message, services, cancellationToken);

        var confirmations = await db.EmailConfirmations.ToListAsync(cancellationToken);
        Assert.Equal(2, confirmations.Count);
        Assert.Equal(2, identifiers.Distinct().Count());
        Assert.All(confirmations, confirmation =>
        {
            Assert.Contains(confirmation.Id, identifiers);
            Assert.NotEqual(message.Id, confirmation.Id.Value);
            Assert.Equal(user.Id, confirmation.UserId);
            Assert.Equal(user.EmailAddress, confirmation.EmailAddress);
            Assert.Equal(IssuedAt.AddHours(24), confirmation.ExpiresAt);
        });
        delivery.Verify(value => value.EnqueueAsync(It.IsAny<EmailConfirmationId>()), Times.Exactly(2));
        unit.Verify(value => value.SaveChangesAsync(cancellationToken), Times.Exactly(2));
    }

    [Theory]
    [InlineData("disabled")]
    [InlineData("missing")]
    [InlineData("expired")]
    public async Task UserCreated_DoesNotStageUnsendableConfirmation(string scenario)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var dbOptions = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new ApiDbContext(dbOptions);
        var user = User.Create(UserId.Create(Guid.NewGuid()), IssuedAt, EmailAddress.Create("user@example.test"),
            PasswordHash.Create("test-hash"), null, null, UserStatus.Pending, null);
        if (scenario != "missing")
        {
            await db.Users.AddAsync(user, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        var delivery = new Mock<IConfirmationEmailOutbox>(MockBehavior.Strict);
        var unit = new Mock<IUnitOfWork>(MockBehavior.Strict);
        await using var services = new ServiceCollection()
            .AddLogging()
            .AddSingleton(db)
            .AddTransient(provider => provider.GetRequiredService<ApiDbContext>().Users)
            .AddTransient<IUserCommandRepository, UserCommandRepository>()
            .AddTransient<IEmailConfirmationCommandRepository, EmailConfirmationCommandRepository>()
            .AddSingleton<IClock>(new FixedClock(scenario == "expired" ? IssuedAt.AddHours(24) : IssuedAt))
            .AddSingleton<IOptions<EmailConfirmationOptions>>(new OptionsWrapper<EmailConfirmationOptions>(new EmailConfirmationOptions { Enabled = scenario != "disabled" }))
            .AddSingleton(delivery.Object)
            .AddSingleton(unit.Object)
            .BuildServiceProvider();

        await HandleUserCreatedAsync(new UserCreated(Guid.NewGuid(), IssuedAt, user.Id.Value), services, cancellationToken);

        Assert.Empty(await db.EmailConfirmations.ToListAsync(cancellationToken));
        delivery.Verify(value => value.EnqueueAsync(It.IsAny<EmailConfirmationId>()), Times.Never);
        unit.Verify(value => value.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(Skip = "Set EMAIL_CONFIRMATION_TEST_POSTGRES to run against PostgreSQL.", SkipUnless = nameof(HasPostgres))]
    public async Task ConfirmAsync_ConcurrentCallsAndReplay_AreIdempotent()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var connection = new NpgsqlConnectionStringBuilder(Environment.GetEnvironmentVariable(ConnectionVariable))
        {
            Database = $"email_confirmation_test_{Guid.NewGuid():N}",
            Pooling = false,
        };
        var options = new DbContextOptionsBuilder<ApiDbContext>().UseNpgsql(connection.ConnectionString).Options;
        await using var db = new ApiDbContext(options);
        await db.Database.EnsureCreatedAsync(cancellationToken);

        try
        {
            var user = User.Create(UserId.Create(Guid.NewGuid()), IssuedAt, EmailAddress.Create("user@example.test"),
                PasswordHash.Create("test-hash"), null, null, UserStatus.Inactive, null);
            var id = EmailConfirmationIdentifier.Generate();
            var confirmation = EmailConfirmation.Create(
                id, user.CreatedAt, user.Id, user.EmailAddress, IssuedAt.AddHours(24));
            await db.Users.AddAsync(user, cancellationToken);
            await db.EmailConfirmations.AddAsync(confirmation, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();

            var commandRepository = new EmailConfirmationCommandRepository(db);
            var queryRepository = new EmailConfirmationQueryRepository(db);
            Assert.Null((await queryRepository.FindAsync(id, cancellationToken))!.ConfirmedAt);
            Assert.False(await commandRepository.ConfirmAsync(id, new FixedClock(confirmation.ExpiresAt), cancellationToken));
            db.ChangeTracker.Clear();

            var firstTime = new FixedClock(IssuedAt.AddHours(1));
            var secondTime = new FixedClock(IssuedAt.AddHours(2));
            await using var parallelDb = new ApiDbContext(options);
            var parallelRepository = new EmailConfirmationCommandRepository(parallelDb);
            var outcomes = await Task.WhenAll(commandRepository.ConfirmAsync(id, firstTime, cancellationToken),
                parallelRepository.ConfirmAsync(id, secondTime, cancellationToken));
            Assert.All(outcomes, Assert.True);

            db.ChangeTracker.Clear();
            var persisted = await db.Users.SingleAsync(cancellationToken);
            Assert.Equal(UserStatus.Active, persisted.Status);
            var confirmedAt = (await queryRepository.FindAsync(id, cancellationToken))!.ConfirmedAt;
            Assert.Contains(confirmedAt, new DateTimeOffset?[] { firstTime.UtcNow, secondTime.UtcNow });

            persisted.UpdateStatus(isActive: false);
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();
            Assert.True(await commandRepository.ConfirmAsync(id, new FixedClock(confirmation.ExpiresAt.AddHours(1)), cancellationToken));
            Assert.Equal(confirmedAt, (await queryRepository.FindAsync(id, cancellationToken))!.ConfirmedAt);
            Assert.Equal(UserStatus.Inactive, (await db.Users.SingleAsync(cancellationToken)).Status);

            var changedUser = await db.Users.SingleAsync(cancellationToken);
            changedUser.UpdateEmailAddress(EmailAddress.Create("new@example.test"));
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();
            Assert.Null(await queryRepository.FindAsync(id, cancellationToken));
            Assert.False(await commandRepository.ConfirmAsync(id, firstTime, cancellationToken));
        }
        finally
        {
            await db.Database.EnsureDeletedAsync(CancellationToken.None);
        }
    }

    private sealed class FixedClock(DateTimeOffset utcNow) : IClock
    {
        public DateTimeOffset UtcNow { get; } = utcNow;
        public DateOnly UtcToday => DateOnly.FromDateTime(UtcNow.UtcDateTime);
    }

    private static Task HandleUserCreatedAsync(UserCreated message, IServiceProvider services, CancellationToken cancellationToken) =>
        ActivatorUtilities.CreateInstance<UserCreatedConfirmationHandler>(services).HandleAsync(message, cancellationToken);

    internal sealed class TestWolverineUnitOfWork(IDbContextOutbox<ApiDbContext> outbox) : IUnitOfWork
    {
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await outbox.SaveChangesAndFlushMessagesAsync(cancellationToken);
    }
}
