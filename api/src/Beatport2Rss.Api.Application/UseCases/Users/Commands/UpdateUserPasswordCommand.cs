using Beatport2Rss.Api.Application.Extensions;
using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Services.Security;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Interfaces;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Users.Commands;

public sealed record UpdateUserPasswordCommand(
    UserId UserId,
    string Password) :
    ICommand<Result>, IRequireUser;

internal sealed class UpdateUserPasswordCommandValidator :
    AbstractValidator<UpdateUserPasswordCommand>
{
    public UpdateUserPasswordCommandValidator()
    {
        RuleFor(c => c.Password).IsPassword();
    }
}

internal sealed class UpdateUserPasswordCommandHandler(
    IPasswordHasher passwordHasher,
    IUserCommandRepository userCommandRepository,
    IUnitOfWork unitOfWork) :
    ICommandHandler<UpdateUserPasswordCommand, Result>
{
    public async ValueTask<Result> Handle(
        UpdateUserPasswordCommand command,
        CancellationToken cancellationToken)
    {
        var password = Password.Create(command.Password);
        var passwordHash = passwordHasher.Hash(password);

        var user = await userCommandRepository.LoadAsync(command.UserId, cancellationToken);

        user.UpdatePasswordHash(passwordHash);

        // TODO: delete all sessions?

        userCommandRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}