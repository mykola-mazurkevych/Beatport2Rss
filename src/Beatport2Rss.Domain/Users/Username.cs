using Ardalis.GuardClauses;

using Beatport2Rss.SharedKernel;
using Beatport2Rss.SharedKernel.Constants;

namespace Beatport2Rss.Domain.Users;

public readonly record struct Username : IValueObject
{
    private const int MaxLength = 200;

    private Username(string value) => Value = value;

    public string Value { get; }

    public static Username Create(string? value)
    {
        Guard.Against.NullOrWhiteSpace(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.UsernameEmpty));
        Guard.Against.StringTooLong(value, MaxLength,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.UsernameTooLong));

        return new Username(value);
    }

    public override string ToString() => Value;

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public bool Equals(Username other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
}