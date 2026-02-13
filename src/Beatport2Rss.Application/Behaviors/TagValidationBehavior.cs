using Beatport2Rss.Application.Extensions;
using Beatport2Rss.Application.Interfaces.Messages;
using Beatport2Rss.Application.Interfaces.Persistence.Repositories;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Application.Behaviors;

file static class ErrorMessages
{
    public const string NotFound = "Tag with slug '{0}' was not found.";
}

internal abstract class TagValidationBehavior<TMessage, TResult>(
    ITagQueryRepository tagQueryRepository)
    where TMessage : IMessage, IRequireUser, IRequireTag
    where TResult : Result
{
    public async ValueTask<TResult> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResult> next,
        CancellationToken cancellationToken)
    {
        var exists = await tagQueryRepository.ExistsAsync(message.UserId, message.Slug, cancellationToken);

        return exists
            ? await next(message, cancellationToken)
            : (TResult)Result.NotFound(ErrorMessages.NotFound, message.Slug);
    }
}

internal abstract class TagValidationBehavior<TMessage, TResult, TResponse>(
    ITagQueryRepository tagQueryRepository)
    where TMessage : IMessage, IRequireUser, IRequireTag
    where TResult : Result<TResponse>
{
    public async ValueTask<TResult> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResult> next,
        CancellationToken cancellationToken)
    {
        var exists = await tagQueryRepository.ExistsAsync(message.UserId, message.Slug, cancellationToken);

        return exists
            ? await next(message, cancellationToken)
            : (TResult)Result.NotFound(ErrorMessages.NotFound, message.Slug);
    }
}