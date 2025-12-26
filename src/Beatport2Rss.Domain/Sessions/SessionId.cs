using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Sessions;

public readonly record struct SessionId : IValueObject
{
    private SessionId(Guid value) => Value = value;

    public Guid Value { get; }

    public static SessionId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException(ExceptionMessages.SessionIdEmpty)));

    public bool Equals(SessionId other) => Value == other.Value;

    public static bool operator ==(SessionId left, Guid right) => left.Value == right;
    public static bool operator !=(SessionId left, Guid right) => left.Value != right;
    public static bool operator ==(Guid left, SessionId right) => left == right.Value;
    public static bool operator !=(Guid left, SessionId right) => left != right.Value;

    public static implicit operator Guid(SessionId value) => value.Value;

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();

    public Guid ToGuid() => Value;
}