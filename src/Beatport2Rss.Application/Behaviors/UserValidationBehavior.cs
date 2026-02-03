using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.Behaviors;

file static class ErrorMessages
{
    public const string Unauthorized = "The user is not authorized to perform this action.";
    public const string Forbidden = "The user is not active to perform this action.";
}

internal abstract class UserValidationBehavior<TMessage, TResult>(
    IUserQueryRepository userQueryRepository)
    where TMessage : IMessage, IRequireActiveUser
    where TResult : Result
{
    public async ValueTask<TResult> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResult> next,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(message.UserId);
        var userStatus = await userQueryRepository.LoadUserStatusQueryModelAsync(userId, cancellationToken);

        return userStatus switch
        {
            null => (TResult)Result.Unauthorized(ErrorMessages.Unauthorized),
            { Status: UserStatus.Pending or UserStatus.Inactive } => (TResult)Result.Forbidden(ErrorMessages.Forbidden),
            { Status: UserStatus.Active } => await next(message, cancellationToken),
            _ => throw new NotSupportedException($"User status '{userStatus.Status}' is not supported.")
        };
    }
}

internal abstract class UserValidationBehavior<TMessage, TResult, TResponse>(
    IUserQueryRepository userQueryRepository)
    where TMessage : IMessage, IRequireActiveUser
    where TResult : Result<TResponse>
{
    public async ValueTask<TResult> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResult> next,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(message.UserId);
        var userStatus = await userQueryRepository.LoadUserStatusQueryModelAsync(userId, cancellationToken);

        return userStatus switch
        {
            null => (TResult)Result.Unauthorized(ErrorMessages.Unauthorized),
            { Status: UserStatus.Pending or UserStatus.Inactive } => (TResult)Result.Forbidden(ErrorMessages.Forbidden),
            { Status: UserStatus.Active } => await next(message, cancellationToken),
            _ => throw new NotSupportedException($"User status '{userStatus.Status}' is not supported.")
        };
    }
}