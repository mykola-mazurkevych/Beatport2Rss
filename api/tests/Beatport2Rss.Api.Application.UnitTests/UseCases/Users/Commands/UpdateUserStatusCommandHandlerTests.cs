using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.UseCases.Users.Commands;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.Domain.Common;
using Beatport2Rss.Common.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.FluentResults.Errors;

using Moq;

using Xunit;

namespace Beatport2Rss.Api.Application.UnitTests.UseCases.Users.Commands;

public sealed class UpdateUserStatusCommandHandlerTests
{
    private readonly UserId _targetId = UserId.Create(Guid.NewGuid());
    private readonly Mock<IUserCommandRepository> _userCommandRepository = new(MockBehavior.Strict);
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    [Theory]
    [InlineData(UserStatus.Pending, true, UserStatus.Active, 1)]
    [InlineData(UserStatus.Inactive, true, UserStatus.Active, 1)]
    [InlineData(UserStatus.Active, true, UserStatus.Active, 0)]
    [InlineData(UserStatus.Active, false, UserStatus.Inactive, 1)]
    public async Task UpdatesStatusWithoutConfirmingEmail(
        UserStatus initialStatus,
        bool isActive,
        UserStatus expectedStatus,
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

        _userCommandRepository.Setup(repository => repository.FindAsync(_targetId, cancellationToken))
            .ReturnsAsync(user);

        var result = await CreateHandler()
            .Handle(new UpdateUserStatusCommand(_targetId.Value, isActive), cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedStatus, user.Status);
        Assert.Null(confirmation.ConfirmedAt);
        _unitOfWork.Verify(unitOfWork => unitOfWork.SaveChangesAsync(cancellationToken), Times.Exactly(saveCount));
    }

    [Fact]
    public async Task ReturnsNotFoundForMissingTargetWithoutSaving()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        _userCommandRepository.Setup(repository => repository.FindAsync(_targetId, cancellationToken))
            .ReturnsAsync((User?)null);

        var result = await CreateHandler()
            .Handle(new UpdateUserStatusCommand(_targetId.Value, IsActive: true), cancellationToken);

        Assert.IsType<NotFoundError>(Assert.Single(result.Errors));
        _unitOfWork.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ConfirmationUpdatesOnlyTheMatchingPendingUser(bool activated)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var emailAddress = EmailAddress.Create("user@example.com");
        _userCommandRepository.Setup(repository => repository.ActivatePendingAsync(_targetId, emailAddress, cancellationToken))
            .ReturnsAsync(activated);

        var result = await CreateHandler().Handle(
            new UpdateUserStatusCommand(_targetId.Value, IsActive: true, emailAddress.Value), cancellationToken);

        Assert.True(result.IsSuccess);
        _userCommandRepository.Verify(repository => repository.ActivatePendingAsync(_targetId, emailAddress, cancellationToken), Times.Once);
        _userCommandRepository.VerifyNoOtherCalls();
        _unitOfWork.VerifyNoOtherCalls();
    }

    private UpdateUserStatusCommandHandler CreateHandler() =>
        new(_userCommandRepository.Object, _unitOfWork.Object);
}
