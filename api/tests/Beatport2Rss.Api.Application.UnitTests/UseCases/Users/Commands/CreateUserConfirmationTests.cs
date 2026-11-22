using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailVerifications;
using Beatport2Rss.Api.Application.Interfaces.Services.Security;
using Beatport2Rss.Api.Application.UseCases.EmailVerifications;
using Beatport2Rss.Api.Application.UseCases.Users.Commands;
using Beatport2Rss.Api.Domain.EmailVerifications;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.Users;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using Microsoft.Extensions.Options;

using Moq;

using Xunit;

namespace Beatport2Rss.Api.Application.UnitTests.UseCases.Users.Commands;

public sealed class CreateUserConfirmationTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Register_StagesConfirmationAndExistingEventBeforeCommit(bool enabled)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var now = new DateTimeOffset(2026, 11, 19, 12, 0, 0, TimeSpan.Zero);
        var clock = new Mock<IClock>(MockBehavior.Strict);
        clock.SetupGet(value => value.UtcNow).Returns(now);
        var hasher = new Mock<IPasswordHasher>(MockBehavior.Strict);
        hasher.Setup(value => value.Hash(It.IsAny<Password>())).Returns(PasswordHash.Create("test-hash"));
        var users = new Mock<IUserCommandRepository>(MockBehavior.Strict);
        var events = new Mock<IIntegrationEventOutbox>(MockBehavior.Strict);
        var verifications = new Mock<IEmailVerificationCommandRepository>(MockBehavior.Strict);
        var delivery = new Mock<IConfirmationEmailOutbox>(MockBehavior.Strict);
        var unit = new Mock<IUnitOfWork>(MockBehavior.Strict);
        var options = Options.Create(new EmailConfirmationOptions { Enabled = enabled });

        User? user = null;
        EmailVerification? verification = null;
        string? identifier = null;
        UserCreated? userCreated = null;

        users.Setup(value => value.ExistsAsync(It.IsAny<EmailAddress>(), cancellationToken)).ReturnsAsync(false);
        users.Setup(value => value.AddAsync(It.IsAny<User>(), cancellationToken))
            .Callback<User, CancellationToken>((value, _) => user = value).Returns(Task.CompletedTask);
        if (enabled)
        {
            verifications.Setup(value => value.AddAsync(It.IsAny<EmailVerification>(), cancellationToken))
                .Callback<EmailVerification, CancellationToken>((value, _) => verification = value).Returns(Task.CompletedTask);
            delivery.Setup(value => value.EnqueueAsync(It.IsAny<string>()))
                .Callback<string>(value => identifier = value).Returns(ValueTask.CompletedTask);
        }

        events.Setup(value => value.EnqueueAsync(It.IsAny<UserCreated>(), cancellationToken))
            .Callback<UserCreated, CancellationToken>((value, _) => userCreated = value).Returns(Task.CompletedTask);
        unit.Setup(value => value.SaveChangesAsync(cancellationToken)).Callback(() =>
        {
            Assert.NotNull(user);
            Assert.Equal(UserStatus.Pending, user.Status);
            Assert.NotNull(userCreated);
            Assert.Equal(user.Id.Value, userCreated.UserId);
            if (enabled)
            {
                Assert.NotNull(verification);
                Assert.NotNull(identifier);
                Assert.Equal(user.Id, verification.UserId);
                Assert.Equal(user.EmailAddress, verification.EmailAddress);
                Assert.Equal(now.AddHours(24), verification.ExpiresAt);
                Assert.Equal(EmailConfirmationIdentifier.Hash(identifier)!.Value, verification.SecretHash);
                Assert.Null(verification.ConfirmedAt);
            }
        }).Returns(Task.CompletedTask);

        var handler = new CreateUserCommandHandler(clock.Object, hasher.Object, users.Object, events.Object,
            verifications.Object, delivery.Object, options, unit.Object);
        var result = await handler.Handle(new CreateUserCommand("new@example.test", "validpassword123", null, null, null),
            cancellationToken);

        Assert.True(result.IsSuccess);
        unit.Verify(value => value.SaveChangesAsync(cancellationToken), Times.Once);
    }
}
