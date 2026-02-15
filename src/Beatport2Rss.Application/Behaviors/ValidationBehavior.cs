using Beatport2Rss.Application.Extensions;

using FluentResults;

using FluentValidation;

using Mediator;

namespace Beatport2Rss.Application.Behaviors;

file static class ErrorMessages
{
    public const string Validation = "One or more validation errors occured.";
}

internal abstract class ValidationBehavior<TMessage, TResult>(IValidator<TMessage> validator)
    where TMessage : IMessage
    where TResult : Result
{
    public async ValueTask<TResult> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResult> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TMessage>(message);
        var validationResult = await validator.ValidateAsync(context, cancellationToken);

        return validationResult.IsValid
            ? await next(message, cancellationToken)
            : (TResult)Result.Validation(ErrorMessages.Validation, validationResult.Errors.ToMetadata());
    }
}

internal abstract class ValidationBehavior<TMessage, TResult, TResponse>(IValidator<TMessage> validator)
    where TMessage : IMessage
    where TResult : Result<TResponse>
{
    public async ValueTask<TResult> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResult> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TMessage>(message);
        var validationResult = await validator.ValidateAsync(context, cancellationToken);

        return validationResult.IsValid
            ? await next(message, cancellationToken)
            : (TResult)Result.Validation(ErrorMessages.Validation, validationResult.Errors.ToMetadata());
    }
}