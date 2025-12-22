using System.Diagnostics.CodeAnalysis;

using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Users;

public readonly record struct PasswordHash : IValueObject
{
    public const int MaxLength = 500;

    private PasswordHash(string value) => Value = value;

    public string Value { get; }

    public static PasswordHash Create([NotNull] string? value)
    {
        Guard.Against.NullOrWhiteSpace(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.PasswordHashEmpty));
        Guard.Against.StringTooLong(value, MaxLength,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.PasswordHashTooLong));

        return new PasswordHash(value);
    }

    public override string ToString() => Value;

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    public bool Equals(PasswordHash other) => StringComparer.Ordinal.Equals(Value, other.Value);
}