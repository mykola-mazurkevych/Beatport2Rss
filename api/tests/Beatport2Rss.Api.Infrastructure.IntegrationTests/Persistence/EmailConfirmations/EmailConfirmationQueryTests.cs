using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Api.Infrastructure.Persistence;
using Beatport2Rss.Api.Infrastructure.Persistence.Migrations;
using Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;
using Beatport2Rss.Api.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

using Xunit;

namespace Beatport2Rss.Api.Infrastructure.IntegrationTests.Persistence.EmailConfirmations;

public sealed class EmailConfirmationQueryTests
{
    [Fact]
    public async Task FindAsync_ShouldReturnOnlyRequestedConfirmation()
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new ApiDbContext(options);
        var id = EmailConfirmationId.Create(Guid.NewGuid());
        var expiresAt = new DateTimeOffset(2026, 11, 24, 12, 0, 0, TimeSpan.Zero);
        var confirmedAt = expiresAt.AddHours(-1);
        context.EmailConfirmationQueryModels.AddRange(
            new EmailConfirmationQueryModel(id, expiresAt, confirmedAt),
            new EmailConfirmationQueryModel(EmailConfirmationId.Create(Guid.NewGuid()), expiresAt.AddDays(1), null));
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        var repository = new EmailConfirmationQueryRepository(context.EmailConfirmationQueryModels.AsNoTracking());

        var result = await repository.FindAsync(id, TestContext.Current.CancellationToken);
        var missing = await repository.FindAsync(
            EmailConfirmationId.Create(Guid.NewGuid()),
            TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(expiresAt, result.ExpiresAt);
        Assert.Equal(confirmedAt, result.ConfirmedAt);
        Assert.Null(missing);
    }

    [Fact]
    public void Query_ShouldTranslateIdentifierFilterAgainstView()
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseNpgsql("Host=localhost;Database=unused;Username=unused;Password=unused")
            .Options;
        using var context = new ApiDbContext(options);
        var id = EmailConfirmationId.Create(Guid.NewGuid());

        var sql = context.EmailConfirmationQueryModels
            .Where(confirmation => confirmation.Id == id)
            .Select(confirmation => new { confirmation.Id, confirmation.ExpiresAt, confirmation.ConfirmedAt })
            .ToQueryString();

        Assert.Contains("FROM api.\"vwEmailConfirmations\"", sql);
        Assert.Contains("WHERE", sql);
        Assert.Contains("\"Id\" =", sql);
    }

    [Fact]
    public void Migration_ShouldUseEmbeddedViewScripts()
    {
        var migration = new AddedEmailConfirmationsView();
        var up = Assert.IsType<SqlOperation>(Assert.Single(migration.UpOperations)).Sql;
        var down = Assert.IsType<SqlOperation>(Assert.Single(migration.DownOperations)).Sql;

        Assert.Contains("CREATE VIEW \"api\".\"vwEmailConfirmations\"", up);
        Assert.Contains("C.\"Id\"", up);
        Assert.Contains("C.\"ExpiresAt\"", up);
        Assert.Contains("C.\"ConfirmedAt\"", up);
        Assert.Contains("FROM \"api\".\"EmailConfirmations\"", up);
        Assert.Contains("DROP VIEW IF EXISTS \"api\".\"vwEmailConfirmations\"", down);
    }
}
