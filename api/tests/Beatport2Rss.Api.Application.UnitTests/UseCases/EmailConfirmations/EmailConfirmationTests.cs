using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.Extensions.Options;

using Moq;

using Xunit;

namespace Beatport2Rss.Api.Application.UnitTests.UseCases.EmailConfirmations;

public sealed class EmailConfirmationTests
{
    private static readonly DateTimeOffset IssuedAt = new(2026, 11, 19, 12, 0, 0, TimeSpan.Zero);
    private readonly Mock<IEmailVerificationCommandRepository> _commandRepository = new(MockBehavior.Strict);
    private readonly Mock<IEmailVerificationQueryRepository> _queryRepository = new(MockBehavior.Strict);
    private readonly Mock<IClock> _clock = new(MockBehavior.Strict);
    private readonly IOptions<EmailConfirmationOptions> _options = Options.Create(new EmailConfirmationOptions { Enabled = true });

    [Theory]
    [InlineData(UserStatus.Pending)]
    [InlineData(UserStatus.Inactive)]
    [InlineData(UserStatus.Active)]
    public void TryConfirm_CompletesVerificationRegardlessOfInitialStatus(UserStatus status)
    {
        var (user, verification) = CreateVerification(status);
        Assert.Null(verification.ConfirmedAt);

        var now = IssuedAt.AddHours(1);
        Assert.True(verification.TryConfirm(user, now));
        Assert.Equal(now, verification.ConfirmedAt);
        Assert.Equal(UserStatus.Active, user.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(false)]
    public async Task Put_RequiresAnExplicitTrue(bool? confirmed)
    {
        var handler = new ConfirmEmailCommandHandler(_commandRepository.Object, _clock.Object, _options);
        var result = await handler.Handle(new ConfirmEmailCommand(CreateHash('A'), confirmed), TestContext.Current.CancellationToken);

        Assert.Equal(EmailConfirmationErrors.ConfirmationRequired, Assert.Single(result.Errors).Message);
        _commandRepository.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("missing", false)]
    [InlineData("unknown", false)]
    [InlineData("unused", false)]
    [InlineData("confirmed", true)]
    public async Task Get_ReadsOnlyMatchingResources_AndHonorsExpiry(string scenario, bool confirmed)
    {
        var (user, verification) = CreateVerification(UserStatus.Pending);
        if (confirmed)
        {
            Assert.True(verification.TryConfirm(user, IssuedAt.AddHours(1)));
        }

        var now = verification.ExpiresAt;
        var cancellationToken = TestContext.Current.CancellationToken;
        _clock.SetupGet(clock => clock.UtcNow).Returns(now);
        EmailVerificationSecretHash? hash = scenario switch
        {
            "missing" => null,
            "unknown" => CreateHash('B'),
            _ => verification.SecretHash,
        };
        if (scenario != "missing")
        {
            _queryRepository.Setup(repository => repository.FindAsync(hash!.Value, cancellationToken))
                .ReturnsAsync(scenario == "confirmed" ? verification : null);
        }

        var handler = new GetEmailConfirmationQueryHandler(_queryRepository.Object, _clock.Object, _options);
        var result = await handler.Handle(new GetEmailConfirmationQuery(hash), cancellationToken);

        if (confirmed)
        {
            Assert.Equal(new EmailConfirmationState(true, verification.ExpiresAt), result.Value);
        }
        else
        {
            Assert.Equal(EmailConfirmationErrors.InvalidLink, Assert.Single(result.Errors).Message);
            Assert.Equal(UserStatus.Pending, user.Status);
            Assert.Null(verification.ConfirmedAt);
        }
    }

    [Fact]
    public async Task Get_ValidUnusedVerification_DoesNotActivateTheUser()
    {
        var (user, verification) = CreateVerification(UserStatus.Pending);
        var cancellationToken = TestContext.Current.CancellationToken;
        _clock.SetupGet(clock => clock.UtcNow).Returns(IssuedAt);
        _queryRepository.Setup(repository => repository.FindAsync(verification.SecretHash, cancellationToken))
            .ReturnsAsync(verification);

        var handler = new GetEmailConfirmationQueryHandler(_queryRepository.Object, _clock.Object, _options);
        var result = await handler.Handle(new GetEmailConfirmationQuery(verification.SecretHash), cancellationToken);

        Assert.Equal(new EmailConfirmationState(false, verification.ExpiresAt), result.Value);
        Assert.Equal(UserStatus.Pending, user.Status);
        Assert.Null(verification.ConfirmedAt);
    }

    [Fact]
    public void Identifier_IsUrlSafeAndNotStoredDirectly()
    {
        var first = EmailConfirmationIdentifier.Generate();
        var second = EmailConfirmationIdentifier.Generate();
        Assert.NotEqual(first, second);
        Assert.Matches("^[A-Za-z0-9_-]{43}$", first);
        var hash = EmailConfirmationIdentifier.Hash(first);
        Assert.NotNull(hash);
        Assert.NotEqual(first, hash.Value.Value);
        Assert.Equal(hash, EmailConfirmationIdentifier.Hash(first));
        Assert.NotEqual(hash, EmailConfirmationIdentifier.Hash(second));
        Assert.Null(EmailConfirmationIdentifier.Hash("not a token"));
    }

    private static (User, EmailVerification) CreateVerification(UserStatus status)
    {
        var user = User.Create(UserId.Create(Guid.NewGuid()), IssuedAt, EmailAddress.Create("user@example.test"),
            PasswordHash.Create("test-hash"), null, null, status, null);
        return (user, EmailVerification.Create(
            EmailVerificationId.Create(Guid.NewGuid()), user, CreateHash('A'), IssuedAt.AddHours(24)));
    }

    private static EmailVerificationSecretHash CreateHash(char character) =>
        EmailVerificationSecretHash.Create(new string(character, EmailVerificationSecretHash.MaxLength));
}
