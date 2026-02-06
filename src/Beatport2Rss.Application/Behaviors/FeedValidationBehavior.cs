using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.Behaviors;

public sealed class FeedValidationBehavior<TMessage, TResponse>(
    IFeedQueryRepository feedQueryRepository) :
    IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage, IRequireUser, IRequireFeed
    where TResponse : Result
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var exists = await feedQueryRepository.ExistsAsync(message.UserId, message.Slug, cancellationToken);

        return exists
            ? await next(message, cancellationToken)
            : (TResponse)Result.NotFound($"Feed with slug '{message.Slug}' was not found.");
    }
}