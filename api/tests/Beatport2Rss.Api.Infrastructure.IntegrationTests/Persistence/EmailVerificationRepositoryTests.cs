using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Api.Infrastructure.Persistence;
using Beatport2Rss.Api.Infrastructure.Persistence.Repositories;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.EntityFrameworkCore;

using Npgsql;

using Xunit;

namespace Beatport2Rss.Api.Infrastructure.IntegrationTests.Persistence;

public sealed class EmailVerificationRepositoryTests
{
    private const string ConnectionVariable = "EMAIL_CONFIRMATION_TEST_POSTGRES";
    private static readonly DateTimeOffset IssuedAt = new(2026, 11, 19, 12, 0, 0, TimeSpan.Zero);

    public static bool HasPostgres => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ConnectionVariable));

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
            var hash = EmailConfirmationIdentifier.Hash(EmailConfirmationIdentifier.Generate())!;
            var verification = EmailVerification.Create(user, hash, IssuedAt.AddHours(24));
            await db.Users.AddAsync(user, cancellationToken);
            await db.EmailVerifications.AddAsync(verification, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();

            var repository = new EmailVerificationRepository(db);
            Assert.Null((await repository.FindAsync(hash, cancellationToken))!.ConfirmedAt);
            Assert.False(await repository.ConfirmAsync(hash, new FixedClock(verification.ExpiresAt), cancellationToken));
            db.ChangeTracker.Clear();

            var firstTime = new FixedClock(IssuedAt.AddHours(1));
            var secondTime = new FixedClock(IssuedAt.AddHours(2));
            await using var parallelDb = new ApiDbContext(options);
            var parallelRepository = new EmailVerificationRepository(parallelDb);
            var outcomes = await Task.WhenAll(repository.ConfirmAsync(hash, firstTime, cancellationToken).AsTask(),
                parallelRepository.ConfirmAsync(hash, secondTime, cancellationToken).AsTask());
            Assert.All(outcomes, outcome => Assert.True(outcome));

            db.ChangeTracker.Clear();
            var persisted = await db.Users.SingleAsync(cancellationToken);
            Assert.Equal(UserStatus.Active, persisted.Status);
            var confirmedAt = (await repository.FindAsync(hash, cancellationToken))!.ConfirmedAt;
            Assert.Contains(confirmedAt, new DateTimeOffset?[] { firstTime.UtcNow, secondTime.UtcNow });

            persisted.UpdateStatus(isActive: false);
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();
            Assert.True(await repository.ConfirmAsync(hash, new FixedClock(verification.ExpiresAt.AddHours(1)), cancellationToken));
            Assert.Equal(confirmedAt, (await repository.FindAsync(hash, cancellationToken))!.ConfirmedAt);
            Assert.Equal(UserStatus.Inactive, (await db.Users.SingleAsync(cancellationToken)).Status);

            var changedUser = await db.Users.SingleAsync(cancellationToken);
            changedUser.UpdateEmailAddress(EmailAddress.Create("new@example.test"));
            await db.SaveChangesAsync(cancellationToken);
            db.ChangeTracker.Clear();
            Assert.Null(await repository.FindAsync(hash, cancellationToken));
            Assert.False(await repository.ConfirmAsync(hash, firstTime, cancellationToken));
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
}
