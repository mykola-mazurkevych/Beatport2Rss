using System.Diagnostics.CodeAnalysis;

using Ardalis.GuardClauses;

using Beatport2Rss.SharedKernel;
using Beatport2Rss.SharedKernel.Constants;

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

    public override string ToString() => Value;

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    public bool Equals(Password other) => StringComparer.Ordinal.Equals(Value, other.Value);
}