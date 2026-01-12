using Beatport2Rss.Application.Interfaces.Messages;

using ErrorOr;

using FluentValidation;
using FluentValidation.Results;

using Mediator;

namespace Beatport2Rss.Application.Behaviors;

public sealed class ValidationBehavior<TMessage, TResponse>(IValidator<TMessage> validator) :
    IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage, IValidate
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(message, cancellationToken);

        if (validationResult.IsValid)
        {
            return await next(message, cancellationToken);
        }

        var error = Error.Validation(
            description: "One or more validation errors occured.",
            metadata: new Dictionary<string, object> { { nameof(ValidationResult), validationResult } });

        return (dynamic)error; // TODO: Fix it
    }
}