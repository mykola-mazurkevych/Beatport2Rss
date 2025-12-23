using System.Globalization;

using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Subscriptions;

public readonly record struct SubscriptionId : IValueObject
{
    private SubscriptionId(int value) => Value = value;

    public int Value { get; }

    public static SubscriptionId Create(int value)
    {
        Guard.Against.NegativeOrZero(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.SubscriptionIdInvalid));

        return new SubscriptionId(value);
    }

    public bool Equals(SubscriptionId other) => Value == other.Value;

    public static bool operator ==(SubscriptionId left, int right) => left.Value == right;
    public static bool operator !=(SubscriptionId left, int right) => left.Value != right;
    public static bool operator ==(int left, SubscriptionId right) => left == right.Value;
    public static bool operator !=(int left, SubscriptionId right) => left != right.Value;

    public static implicit operator int(SubscriptionId value) => value.Value;

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    public int ToInt32() => Value;
}