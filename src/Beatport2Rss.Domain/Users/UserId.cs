using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Users;

public readonly record struct UserId : IValueObject
{
    private UserId(Guid value) => Value = value;

    public Guid Value { get; }

    public static UserId Create(Guid value)
    {
        Guard.Against.Default(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.UserIdEmpty));

        return new UserId(value);
    }

    public bool Equals(UserId other) => Value == other.Value;

    public static bool operator ==(UserId left, Guid right) => left.Value == right;
    public static bool operator !=(UserId left, Guid right) => left.Value != right;
    public static bool operator ==(Guid left, UserId right) => left == right.Value;
    public static bool operator !=(Guid left, UserId right) => left != right.Value;

    public static implicit operator Guid(UserId value) => value.Value;

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();

    public Guid ToGuid() => Value;
}