using Beatport2Rss.Common.Domain.Exceptions;
using Beatport2Rss.Common.Domain.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Api.Domain.EmailVerifications;

public readonly record struct EmailVerificationId :
    IId<EmailVerificationId>
{
    private EmailVerificationId(Guid value) => Value = value;

    public Guid Value { get; }

    public static bool operator <(EmailVerificationId left, EmailVerificationId right) => left.Value < right.Value;
    public static bool operator >(EmailVerificationId left, EmailVerificationId right) => left.Value > right.Value;
    public static bool operator <=(EmailVerificationId left, EmailVerificationId right) => left.Value <= right.Value;
    public static bool operator >=(EmailVerificationId left, EmailVerificationId right) => left.Value >= right.Value;

    public static EmailVerificationId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException($"{nameof(EmailVerificationId)} cannot be empty")));

    public int CompareTo(EmailVerificationId other) => Value.CompareTo(other.Value);
    public bool Equals(EmailVerificationId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}