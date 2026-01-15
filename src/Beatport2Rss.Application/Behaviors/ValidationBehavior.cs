using Beatport2Rss.Application.Extensions;

using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using Mediator;

namespace Beatport2Rss.Application.Behaviors;

public sealed class ValidationBehavior<TMessage, TResponse>(IEnumerable<IValidator> validators) :
    IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
    where TResponse : Result
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        Result validationResult = await ValidateAsync(message, cancellationToken);

        return validationResult.IsSuccess
            ? await next(message, cancellationToken)
            : (TResponse)validationResult;
    }

    private async Task<Result> ValidateAsync(TMessage message, CancellationToken cancellationToken)
    {
        var validator = validators.SingleOrDefault(v => v.GetType().BaseType == typeof(AbstractValidator<TMessage>));

        if (validator is null)
        {
            return Result.Ok();
        }

        var context = new ValidationContext<TMessage>(message);
        var validationResult = await validator.ValidateAsync(context, cancellationToken);

        return validationResult.Errors.Count == 0
            ? Result.Ok()
            : Result.Validation("One or more validation errors occured.", validationResult.Errors.ToMetadata());
    }
}

file static class ValidationFailureExtensions
{
    extension(List<ValidationFailure> failures)
    {
        public Dictionary<string, object> ToMetadata() =>
            failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    object (g) => g.Select(f => f.ErrorMessage).Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
    }
}