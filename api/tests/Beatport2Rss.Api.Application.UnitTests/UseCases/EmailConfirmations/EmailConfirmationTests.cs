using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.ReadModels.EmailConfirmations;
using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Commands;
using Beatport2Rss.Api.Application.UseCases.EmailConfirmations.IntegrationEvents;
using Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Queries;
using Beatport2Rss.Api.Application.UseCases.Users.Commands;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents;
using Beatport2Rss.Common.IntegrationEvents.EmailConfirmations;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using Mediator;

using Microsoft.Extensions.Options;

using Moq;

using Xunit;

namespace Beatport2Rss.Api.Application.UnitTests.UseCases.EmailConfirmations;

public sealed class EmailConfirmationTests
{
    private static readonly DateTimeOffset IssuedAt = new(2026, 11, 19, 12, 0, 0, TimeSpan.Zero);
    private readonly Mock<IEmailConfirmationCommandRepository> _commandRepository = new(MockBehavior.Strict);
    private readonly Mock<IEmailConfirmationQueryRepository> _queryRepository = new(MockBehavior.Strict);
    private readonly Mock<IUserCommandRepository> _users = new(MockBehavior.Strict);
    private readonly Mock<IIntegrationEventOutbox> _outbox = new(MockBehavior.Strict);
    private readonly Mock<IUnitOfWork> _unitOfWork = new(MockBehavior.Strict);
    private readonly Mock<IClock> _clock = new(MockBehavior.Strict);
    private readonly IOptions<EmailConfirmationOptions> _options = Options.Create(new EmailConfirmationOptions { Enabled = true });

    [Theory]
    [InlineData(UserStatus.Pending)]
    [InlineData(UserStatus.Inactive)]
    [InlineData(UserStatus.Active)]
    public void TryConfirm_DoesNotChangeUserStatus(UserStatus status)
    {
        var (user, confirmation) = CreateConfirmation(status);
        Assert.Null(confirmation.ConfirmedAt);

        var now = IssuedAt.AddHours(1);
        Assert.True(confirmation.TryConfirm(user, now));
        Assert.Equal(now, confirmation.ConfirmedAt);
        Assert.Equal(status, user.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(false)]
    public async Task Put_RequiresAnExplicitTrue(bool? confirmed)
    {
        var handler = CreateConfirmHandler();
        var result = await handler.Handle(new ConfirmEmailCommand(EmailConfirmationId.Create(Guid.NewGuid()), confirmed), TestContext.Current.CancellationToken);

        Assert.Equal(EmailConfirmationErrors.ConfirmationRequired, Assert.Single(result.Errors).Message);
        _commandRepository.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("new")]
    [InlineData("confirmed")]
    [InlineData("expired")]
    [InlineData("changed-email")]
    public async Task Put_EnqueuesEventOnlyForNewValidConfirmation(string scenario)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var (user, confirmation) = CreateConfirmation(UserStatus.Pending);
        if (scenario == "confirmed")
        {
            Assert.True(confirmation.TryConfirm(user, IssuedAt));
        }
        else if (scenario == "changed-email")
        {
            user.UpdateEmailAddress(EmailAddress.Create("other@example.test"));
        }

        var now = scenario == "expired" ? confirmation.ExpiresAt : IssuedAt.AddHours(1);
        _clock.SetupGet(clock => clock.UtcNow).Returns(now);
        _commandRepository.Setup(repository => repository.FindAsync(confirmation.Id, cancellationToken)).ReturnsAsync(confirmation);
        _users.Setup(repository => repository.FindAsync(user.Id, cancellationToken)).ReturnsAsync(user);
        if (scenario == "new")
        {
            _outbox.Setup(outbox => outbox.EnqueueAsync(
                    It.Is<EmailConfirmed>(message => message.UserId == user.Id.Value && message.EmailAddress == user.EmailAddress.Value),
                    cancellationToken))
                .Returns(Task.CompletedTask);
            _unitOfWork.Setup(unitOfWork => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(Task.CompletedTask);
        }

        var result = await CreateConfirmHandler().Handle(new ConfirmEmailCommand(confirmation.Id, true), cancellationToken);

        Assert.Equal(scenario is "new" or "confirmed", result.IsSuccess);
        Assert.Equal(UserStatus.Pending, user.Status);
        if (scenario == "new")
        {
            Assert.Equal(now, confirmation.ConfirmedAt);
        }
        _outbox.VerifyAll();
        _outbox.VerifyNoOtherCalls();
        _unitOfWork.VerifyAll();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("missing", false)]
    [InlineData("unknown", false)]
    [InlineData("unused", false)]
    [InlineData("confirmed", true)]
    public async Task Get_ReadsOnlyMatchingResources_AndHonorsExpiry(string scenario, bool confirmed)
    {
        var (user, confirmation) = CreateConfirmation(UserStatus.Pending);
        if (confirmed)
        {
            Assert.True(confirmation.TryConfirm(user, IssuedAt.AddHours(1)));
        }

        var now = confirmation.ExpiresAt;
        var cancellationToken = TestContext.Current.CancellationToken;
        _clock.SetupGet(clock => clock.UtcNow).Returns(now);
        EmailConfirmationId? id = scenario switch
        {
            "missing" => null,
            "unknown" => EmailConfirmationId.Create(Guid.NewGuid()),
            _ => confirmation.Id,
        };
        if (scenario != "missing")
        {
            _queryRepository.Setup(repository => repository.FindAsync(id!.Value, cancellationToken))
                .ReturnsAsync(scenario is "unused" or "confirmed" ? ToReadModel(confirmation) : null);
        }

        var handler = new GetEmailConfirmationQueryHandler(_queryRepository.Object, _clock.Object, _options);
        var result = await handler.Handle(new GetEmailConfirmationQuery(id), cancellationToken);

        if (confirmed)
        {
            Assert.Equal(new EmailConfirmationState(true, confirmation.ExpiresAt), result.Value);
        }
        else
        {
            Assert.Equal(EmailConfirmationErrors.InvalidLink, Assert.Single(result.Errors).Message);
            Assert.Equal(UserStatus.Pending, user.Status);
            Assert.Null(confirmation.ConfirmedAt);
        }
    }

    [Fact]
    public async Task Get_ValidUnusedConfirmation_DoesNotActivateTheUser()
    {
        var (user, confirmation) = CreateConfirmation(UserStatus.Pending);
        var cancellationToken = TestContext.Current.CancellationToken;
        _clock.SetupGet(clock => clock.UtcNow).Returns(IssuedAt);
        _queryRepository.Setup(repository => repository.FindAsync(confirmation.Id, cancellationToken))
            .ReturnsAsync(ToReadModel(confirmation));

        var handler = new GetEmailConfirmationQueryHandler(_queryRepository.Object, _clock.Object, _options);
        var result = await handler.Handle(new GetEmailConfirmationQuery(confirmation.Id), cancellationToken);

        Assert.Equal(new EmailConfirmationState(false, confirmation.ExpiresAt), result.Value);
        Assert.Equal(UserStatus.Pending, user.Status);
        Assert.Null(confirmation.ConfirmedAt);
    }

    [Fact]
    public async Task EmailConfirmed_PropagatesCommandFailures()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var message = new EmailConfirmed(Guid.NewGuid(), IssuedAt, Guid.NewGuid(), "user@example.test");
        var mediator = new Mock<IMediator>(MockBehavior.Strict);
        mediator.Setup(value => value.Send(
                It.Is<ICommand<Result>>(command =>
                    command.Equals(new UpdateUserStatusCommand(message.UserId, true, message.EmailAddress))),
                cancellationToken))
            .ReturnsAsync(Result.Fail("Status update failed."));

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            EmailConfirmedHandler.HandleAsync(message, mediator.Object, cancellationToken));

        Assert.Equal("Status update failed.", error.Message);
        mediator.VerifyAll();
    }

    private static EmailConfirmationReadModel ToReadModel(EmailConfirmation confirmation) =>
        new(confirmation.Id, confirmation.CreatedAt, confirmation.EmailAddress, confirmation.ExpiresAt, confirmation.ConfirmedAt);

    private ConfirmEmailCommandHandler CreateConfirmHandler() =>
        new(_commandRepository.Object, _users.Object, _outbox.Object, _unitOfWork.Object, _clock.Object, _options);

    private static (User, EmailConfirmation) CreateConfirmation(UserStatus status)
    {
        var user = User.Create(UserId.Create(Guid.NewGuid()), IssuedAt, EmailAddress.Create("user@example.test"),
            PasswordHash.Create("test-hash"), null, null, status, null);
        return (user, EmailConfirmation.Create(
            EmailConfirmationId.Create(Guid.NewGuid()), user.CreatedAt, user.Id, user.EmailAddress, IssuedAt.AddHours(24)));
    }
}
