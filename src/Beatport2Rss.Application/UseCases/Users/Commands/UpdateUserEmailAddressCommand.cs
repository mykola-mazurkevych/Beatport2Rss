using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;
using Beatport2Rss.SharedKernel.Extensions;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Users.Commands;

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

        var exists = await userCommandRepository.ExistsAsync(
            u => u.Id != command.UserId && u.EmailAddress == emailAddress,
            cancellationToken);
        if (exists)
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