using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.SharedKernel.Common;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Users;

public readonly record struct UserId :
    IId<UserId>, IParsable<UserId>
{
    private UserId(Guid value) => Value = value;

    public Guid Value { get; }

    public static bool operator <(UserId left, UserId right) => left.Value < right.Value;
    public static bool operator >(UserId left, UserId right) => left.Value > right.Value;
    public static bool operator <=(UserId left, UserId right) => left.Value <= right.Value;
    public static bool operator >=(UserId left, UserId right) => left.Value >= right.Value;

    public static UserId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException(ExceptionMessages.UserIdEmpty)));

    public static UserId Parse(string s, IFormatProvider? provider) =>
        Create(Guid.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out UserId result)
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

    public int CompareTo(UserId other) => Value.CompareTo(other.Value);
    public bool Equals(UserId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}