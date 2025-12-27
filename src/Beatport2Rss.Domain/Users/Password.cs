using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Users;

public readonly record struct Password : IValueObject
{
    public const int MinLength = 8;
    public const int MaxLength = 100;

    private Password(string value) => Value = value;

    public string Value { get; }

    public static Password Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.PasswordEmpty))
            .MustBeLongerThanOrEqualTo(MinLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.PasswordTooShort))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.PasswordTooLong)));

    public bool Equals(Password other) => StringComparer.Ordinal.Equals(Value, other.Value);

    public static bool operator ==(Password left, string? right) => StringComparer.Ordinal.Equals(left.Value, right);
    public static bool operator !=(Password left, string? right) => !StringComparer.Ordinal.Equals(left.Value, right);
    public static bool operator ==(string? left, Password right) => StringComparer.Ordinal.Equals(left, right.Value);
    public static bool operator !=(string? left, Password right) => !StringComparer.Ordinal.Equals(left, right.Value);

    public static implicit operator string(Password password) => password.Value;
    
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    public override string ToString() => Value;
}