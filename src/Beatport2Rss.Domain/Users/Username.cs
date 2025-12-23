using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Users;

public readonly partial record struct Username : IValueObject
{
    public const int MaxLength = 200;
    public const string RegexPattern = @"^[a-zA-Z0-9\-_.]+$";

    private Username(string value) => Value = value;

    public string Value { get; }

    public static Username Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.UsernameEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.UsernameTooLong))
            .MustMatch(UsernameRegex(), (_, _) => new InvalidValueObjectValueException(ExceptionMessages.UsernameInvalidCharacters)));

    public bool Equals(Username other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

    public static bool operator ==(Username left, string? right) => StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);
    public static bool operator !=(Username left, string? right) => !StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);
    public static bool operator ==(string? left, Username right) => StringComparer.OrdinalIgnoreCase.Equals(left, right.Value);
    public static bool operator !=(string? left, Username right) => !StringComparer.OrdinalIgnoreCase.Equals(left, right.Value);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;

    [GeneratedRegex(RegexPattern, RegexOptions.None)]
    private static partial Regex UsernameRegex();
}