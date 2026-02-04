using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Sessions;

public readonly record struct SessionId : IValueObject, IParsable<SessionId>
{
    private SessionId(Guid value) => Value = value;

    public Guid Value { get; }

    public static SessionId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException(ExceptionMessages.SessionIdEmpty)));
    
    public static SessionId Parse(string s, IFormatProvider? provider) =>
        Create(Guid.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out SessionId result)
    {
        result = default;

        try
        {
            if (Guid.TryParse(s, provider, out var value))
            {
                result = Create(value);

                return true;
            }

            return false;
        }
        catch (InvalidValueObjectValueException)
        {
            return false;
        }
    }

    public bool Equals(SessionId other) => Value == other.Value;

    public static bool operator ==(SessionId left, Guid right) => left.Value == right;
    public static bool operator !=(SessionId left, Guid right) => left.Value != right;
    public static bool operator ==(Guid left, SessionId right) => left == right.Value;
    public static bool operator !=(Guid left, SessionId right) => left != right.Value;

    public static implicit operator Guid(SessionId sessionId) => sessionId.Value;
    public static implicit operator string(SessionId sessionId) => sessionId.ToString();

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();

    public Guid ToGuid() => Value;
}