using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using ErrorOr;

using Mediator;

namespace Beatport2Rss.Application.Behaviors;

public sealed class ForbiddenBehavior<TMessage, TResponse>(IUserQueryRepository userRepository) :
    IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage, IHaveUserId
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(message.UserId);
        var user = await userRepository.FindAsync(userId, cancellationToken);

        if (user is not null && user.IsActive)
        {
            return await next(message, cancellationToken);
        }

        var error = Error.Forbidden(
            code: "User.Forbidden",
            description: "The user is not active to perform this action.");

        return (dynamic)error; // TODO: Fix it
    }
}