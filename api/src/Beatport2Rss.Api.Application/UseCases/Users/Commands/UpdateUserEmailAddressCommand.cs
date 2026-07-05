using Beatport2Rss.Api.Application.Extensions;
using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.Extensions;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Users.Commands;

public sealed record UpdateUserEmailAddressCommand(
    UserId UserId,
    string EmailAddress) :
    ICommand<Result>, IRequireUser;

internal sealed class UpdateUserEmailAddressCommandValidator :
    AbstractValidator<UpdateUserEmailAddressCommand>
{
    public UpdateUserEmailAddressCommandValidator()
    {
        RuleFor(s => s.EmailAddress).IsEmailAddress();
    }
}

internal sealed class UpdateUserEmailAddressCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateUserEmailAddressCommand, Result>
{
    public async ValueTask<Result> Handle(
        UpdateUserEmailAddressCommand command,
        CancellationToken cancellationToken)
    {
        var emailAddress = EmailAddress.Create(command.EmailAddress);

        if (await userCommandRepository.ExistsExceptAsync(emailAddress, command.UserId, cancellationToken))
        {
            return Result.Conflict($"Email address '{emailAddress}' already taken.");
        }

        var user = await userCommandRepository.LoadAsync(command.UserId, cancellationToken);

        if (user.EmailAddress == emailAddress)
        {
            return Result.Ok();
        }

        user.UpdateEmailAddress(emailAddress);
        ////user.UpdateStatus(false);

        userCommandRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}