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

internal abstract class UserValidationBehavior<TMessage, TResult>(IUserQueryRepository userRepository)
    where TMessage : IMessage, IRequireActiveUser
    where TResult : Result
{
    public async ValueTask<TResult> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResult> next,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(message.UserId);
        var user = await userRepository.FindAsync(userId, cancellationToken);

        return user switch
        {
            null => (TResult)Result.Unauthorized(ErrorMessages.Unauthorized),
            { IsActive: false } => (TResult)Result.Forbidden(ErrorMessages.Forbidden),
            _ => await next(message, cancellationToken)
        };
    }
}

internal abstract class UserValidationBehavior<TMessage, TResult, TResponse>(IUserQueryRepository userRepository)
    where TMessage : IMessage, IRequireActiveUser
    where TResult : Result<TResponse>
{
    public async ValueTask<TResult> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResult> next,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(message.UserId);
        var user = await userRepository.FindAsync(userId, cancellationToken);

        return user switch
        {
            null => (TResult)Result.Unauthorized(ErrorMessages.Unauthorized),
            { IsActive: false } => (TResult)Result.Forbidden(ErrorMessages.Forbidden),
            _ => await next(message, cancellationToken)
        };
    }
}