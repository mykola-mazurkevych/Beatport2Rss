using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Ardalis.GuardClauses;

using Beatport2Rss.SharedKernel;
using Beatport2Rss.SharedKernel.Constants;

namespace Beatport2Rss.Domain.Users;

public readonly partial record struct Username : IValueObject
{
    public const int MaxLength = 200;
    public const string RegexPattern = @"^[a-zA-Z0-9\-_.]+$";

    private Username(string value) => Value = value;

    public string Value { get; }

    public static Username Create([NotNull] string? value)
    {
        Guard.Against.NullOrWhiteSpace(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.UsernameEmpty));
        Guard.Against.StringTooLong(value, MaxLength,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.UsernameTooLong));
        Guard.Against.InvalidInput(value,
            nameof(value),
            IsValid,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.UsernameInvalidCharacters));

        return new Username(value);
    }

    public override string ToString() => Value;

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public bool Equals(Username other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

    public static bool operator ==(Username left, string? right) =>
        right is not null && StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);

    public static bool operator !=(Username left, string? right) =>
        !(left == right);

    private static bool IsValid(string value) =>
        UsernameRegex().IsMatch(value);

    [GeneratedRegex(RegexPattern, RegexOptions.None)]
    private static partial Regex UsernameRegex();
}