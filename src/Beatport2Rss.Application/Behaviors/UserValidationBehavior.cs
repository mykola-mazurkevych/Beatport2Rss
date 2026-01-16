using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.Behaviors;

public sealed class UserValidationBehavior<TMessage, TResponse>(IUserQueryRepository userRepository) :
    IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage, IRequireActiveUser
    where TResponse : Result
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(message.UserId);
        var user = await userRepository.FindAsync(userId, cancellationToken);

        return user switch
        {
            null => (TResponse)Result.Unauthorized("The user is not authorized to perform this action."),
            { IsActive: false } => (TResponse)Result.Forbidden("The user is not active to perform this action."),
            _ => await next(message, cancellationToken)
        };
    }
}