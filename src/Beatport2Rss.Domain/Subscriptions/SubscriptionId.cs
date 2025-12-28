using System.Globalization;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Subscriptions;

public readonly record struct SubscriptionId : IValueObject
{
    private SubscriptionId(int value) => Value = value;

    public int Value { get; }

    public static SubscriptionId Create(int value) =>
        new(value.MustBeGreaterThan(0, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.SubscriptionIdInvalid)));

    public bool Equals(SubscriptionId other) => Value == other.Value;

    public static bool operator ==(SubscriptionId left, int right) => left.Value == right;
    public static bool operator !=(SubscriptionId left, int right) => left.Value != right;
    public static bool operator ==(int left, SubscriptionId right) => left == right.Value;
    public static bool operator !=(int left, SubscriptionId right) => left != right.Value;

    public static implicit operator int(SubscriptionId subscriptionId) => subscriptionId.Value;
    public static implicit operator string(SubscriptionId subscriptionId) => subscriptionId.ToString();

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    public int ToInt32() => Value;
}