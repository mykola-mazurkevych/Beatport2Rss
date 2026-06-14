using Beatport2Rss.ReleaseCollector.Domain.Common.Constants;
using Beatport2Rss.SharedKernel.Exceptions;
using Beatport2Rss.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.ReleaseCollector.Domain.Subscriptions;

public readonly record struct SubscriptionId :
    IId<SubscriptionId>
{
    private SubscriptionId(Guid value) => Value = value;

    public Guid Value { get; }

    public static SubscriptionId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException(ExceptionMessages.SubscriptionIdEmpty)));

    public static bool operator <(SubscriptionId left, SubscriptionId right) => left.Value < right.Value;
    public static bool operator >(SubscriptionId left, SubscriptionId right) => left.Value > right.Value;
    public static bool operator <=(SubscriptionId left, SubscriptionId right) => left.Value >= right.Value;
    public static bool operator >=(SubscriptionId left, SubscriptionId right) => left.Value >= right.Value;

    public int CompareTo(SubscriptionId other) => Value.CompareTo(other.Value);
    public bool Equals(SubscriptionId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}