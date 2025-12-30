using FluentValidation.Results;

namespace Beatport2Rss.Application.Types;

public readonly record struct Created;

public readonly record struct EmailAddressAlreadyTaken(string EmailAddress)
{
    public string Message => $"Email address {EmailAddress} already taken.";
}

public readonly record struct None;

public readonly record struct ValidationError
{
    public ValidationError(ValidationResult validationResult) =>
        Errors = validationResult.Errors
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage))
            .AsReadOnly();

    public IReadOnlyDictionary<string, IEnumerable<string>> Errors { get; }
}