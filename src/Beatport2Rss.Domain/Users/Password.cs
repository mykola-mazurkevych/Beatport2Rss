using System.Diagnostics.CodeAnalysis;

using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Users;

public readonly record struct Password : IValueObject
{
    public const int MinLength = 8;
    public const int MaxLength = 100;

    private Password(string value) => Value = value;

    public string Value { get; }

    public static Password Create([NotNull] string? value)
    {
        Guard.Against.NullOrWhiteSpace(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.PasswordEmpty));
        Guard.Against.StringTooShort(value, MinLength,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.PasswordTooShort));
        Guard.Against.StringTooLong(value, MaxLength,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.PasswordTooLong));

        return new Password(value);
    }

    public bool Equals(Password other) => StringComparer.Ordinal.Equals(Value, other.Value);

    public static bool operator ==(Password left, string? right) => StringComparer.Ordinal.Equals(left.Value, right);
    public static bool operator !=(Password left, string? right) => !StringComparer.Ordinal.Equals(left.Value, right);
    public static bool operator ==(string? left, Password right) => StringComparer.Ordinal.Equals(left, right.Value);
    public static bool operator !=(string? left, Password right) => !StringComparer.Ordinal.Equals(left, right.Value);
    
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);
    public override string ToString() => Value;
}