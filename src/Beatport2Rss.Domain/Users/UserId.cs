using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Users;

public readonly record struct UserId : IValueObject
{
    private UserId(Guid value) => Value = value;

    public Guid Value { get; }

    public static UserId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException(ExceptionMessages.UserIdEmpty)));

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