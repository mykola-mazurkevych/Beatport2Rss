using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.SharedKernel.Common;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Sessions;

public readonly record struct SessionId :
    IId<SessionId>, IParsable<SessionId>
{
    private SessionId(Guid value) => Value = value;

    public Guid Value { get; }

    public static bool operator <(SessionId left, SessionId right) => left.Value < right.Value;
    public static bool operator >(SessionId left, SessionId right) => left.Value > right.Value;
    public static bool operator <=(SessionId left, SessionId right) => left.Value <= right.Value;
    public static bool operator >=(SessionId left, SessionId right) => left.Value >= right.Value;

    public static SessionId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException(ExceptionMessages.SessionIdEmpty)));

    public static SessionId Parse(string s, IFormatProvider? provider) =>
        Create(Guid.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out SessionId result)
    {
        result = default;

        try
        {
            if (!Guid.TryParse(s, provider, out var value))
            {
                return false;
            }

            result = Create(value);

            return true;

        }
        catch (InvalidValueObjectValueException)
        {
            return false;
        }
    }

    public int CompareTo(SessionId other) => Value.CompareTo(other.Value);
    public bool Equals(SessionId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}