using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Application.UseCases.Users.Commands;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.FluentResults.Errors;

using Microsoft.Extensions.Options;

using Moq;

using Xunit;

namespace Beatport2Rss.Api.Application.UnitTests.UseCases.Users.Commands;

public sealed class ActivateUserCommandHandlerTests
{
    private readonly UserId _administratorId = UserId.Create(Guid.NewGuid());
    private readonly UserId _targetId = UserId.Create(Guid.NewGuid());
    private readonly Mock<IUserCommandRepository> _userCommandRepository = new(MockBehavior.Strict);
    private readonly Mock<IUserQueryRepository> _userQueryRepository = new(MockBehavior.Strict);
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    [Theory]
    [InlineData(false, UserStatus.Pending, 1)]
    [InlineData(false, UserStatus.Inactive, 1)]
    [InlineData(false, UserStatus.Active, 0)]
    [InlineData(true, UserStatus.Pending, 1)]
    [InlineData(true, UserStatus.Inactive, 1)]
    [InlineData(true, UserStatus.Active, 0)]
    public async Task ConfiguredAdministratorCanActivateAnotherUserWithoutConfirmingEmail(
        bool emailConfirmationEnabled,
        UserStatus initialStatus,
        int saveCount)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var user = User.Create(
            _targetId,
            DateTimeOffset.UnixEpoch,
            EmailAddress.Create("user@example.com"),
            PasswordHash.Create("test-hash"),
            firstName: null,
            lastName: null,
            initialStatus,
            countryCode: null);
        var confirmation = EmailConfirmation.Create(
            EmailConfirmationId.Create(Guid.NewGuid()),
            user.CreatedAt,
            user.Id,
            user.EmailAddress,
            DateTimeOffset.UnixEpoch.AddDays(1));
        Assert.Null(confirmation.ConfirmedAt);

        _userQueryRepository.Setup(repository => repository.ExistsAsync(_administratorId, cancellationToken))
            .ReturnsAsync(true);
        _userCommandRepository.Setup(repository => repository.FindAsync(_targetId, cancellationToken))
            .ReturnsAsync(user);

        var result = await CreateHandler(new EmailConfirmationOptions
            {
                Enabled = emailConfirmationEnabled,
                AdministratorUserId = _administratorId.Value,
            })
            .Handle(new ActivateUserCommand(_targetId, _administratorId), cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(UserStatus.Active, user.Status);
        Assert.Null(confirmation.ConfirmedAt);
        _unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(cancellationToken), Times.Exactly(saveCount));
    }

    [Theory]
    [InlineData("no administrator")]
    [InlineData("empty administrator")]
    [InlineData("different caller")]
    [InlineData("self-activation")]
    [InlineData("administrator missing")]
    public async Task RejectsUnauthorizedActivationBeforeLookingUpTarget(string scenario)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var options = new EmailConfirmationOptions
        {
            Enabled = scenario == "confirmation enabled",
            AdministratorUserId = scenario switch
            {
                "no administrator" => null,
                "empty administrator" => Guid.Empty,
                _ => _administratorId.Value,
            },
        };
        var callerId = scenario switch
        {
            "different caller" => UserId.Create(Guid.NewGuid()),
            "empty administrator" => default,
            _ => _administratorId,
        };
        var targetId = scenario == "self-activation" ? callerId : _targetId;
        if (scenario == "administrator missing")
        {
            _userQueryRepository.Setup(repository => repository.ExistsAsync(callerId, cancellationToken))
                .ReturnsAsync(false);
        }

        var result = await CreateHandler(options).Handle(new ActivateUserCommand(targetId, callerId), cancellationToken);

        Assert.IsType<ForbiddenError>(Assert.Single(result.Errors));
        _userCommandRepository.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ReturnsNotFoundForMissingTargetWithoutSaving()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        _userQueryRepository.Setup(repository => repository.ExistsAsync(_administratorId, cancellationToken))
            .ReturnsAsync(true);
        _userCommandRepository.Setup(repository => repository.FindAsync(_targetId, cancellationToken))
            .ReturnsAsync((User?)null);

        var result = await CreateHandler(new EmailConfirmationOptions { AdministratorUserId = _administratorId.Value })
            .Handle(new ActivateUserCommand(_targetId, _administratorId), cancellationToken);

        Assert.IsType<NotFoundError>(Assert.Single(result.Errors));
        _unitOfWork.VerifyNoOtherCalls();
    }

    private ActivateUserCommandHandler CreateHandler(EmailConfirmationOptions options) =>
        new(_userCommandRepository.Object, _userQueryRepository.Object, _unitOfWork.Object, Options.Create(options));
}
