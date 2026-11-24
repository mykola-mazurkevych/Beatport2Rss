using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Common.Domain.Exceptions;
using Beatport2Rss.Common.Domain.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Api.Domain.EmailConfirmations;

public readonly record struct EmailConfirmationId :
    IId<EmailConfirmationId>, IParsable<EmailConfirmationId>
{
    private EmailConfirmationId(Guid value) => Value = value;

    public Guid Value { get; }

    public static bool operator <(EmailConfirmationId left, EmailConfirmationId right) => left.Value < right.Value;
    public static bool operator >(EmailConfirmationId left, EmailConfirmationId right) => left.Value > right.Value;
    public static bool operator <=(EmailConfirmationId left, EmailConfirmationId right) => left.Value <= right.Value;
    public static bool operator >=(EmailConfirmationId left, EmailConfirmationId right) => left.Value >= right.Value;

    public static EmailConfirmationId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException($"{nameof(EmailConfirmationId)} cannot be empty")));

    public static EmailConfirmationId Parse(string s, IFormatProvider? provider) =>
        Create(Guid.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out EmailConfirmationId result)
    {
        result = default;

        if (!Guid.TryParse(s, provider, out var id))
        {
            return false;
        }

        try
        {
            result = Create(id);
        }
        catch (InvalidValueObjectValueException)
        {
            return false;
        }

        return true;
    }

    public int CompareTo(EmailConfirmationId other) => Value.CompareTo(other.Value);
    public bool Equals(EmailConfirmationId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();

    
}