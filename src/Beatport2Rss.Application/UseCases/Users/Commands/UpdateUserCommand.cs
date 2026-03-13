using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.UseCases.Users.Commands;

public sealed record UpdateUserCommand(
    UserId UserId,
    string? FirstName,
    string? LastName) :
    ICommand<Result>, IRequireUser;

internal sealed class UpdateUserCommandValidator :
    AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(c => c.FirstName).IsFirstName();
        RuleFor(c => c.LastName).IsLastName();
    }
}

internal sealed class UpdateUserCommandHandler(
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateUserCommand, Result>
{
    public async ValueTask<Result> Handle(
        UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userCommandRepository.LoadAsync(command.UserId, cancellationToken);

        user.UpdateFirstName(command.FirstName);
        user.UpdateLastName(command.LastName);

        userCommandRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}