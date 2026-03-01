using System.Globalization;

using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Domain.Subscriptions;

public readonly record struct SubscriptionId :
    IId<SubscriptionId>
{
    private SubscriptionId(int value) => Value = value;

    public int Value { get; }

    public static SubscriptionId Create(int value) => new(value);

    public static bool operator <(SubscriptionId left, SubscriptionId right) => left.Value < right.Value;
    public static bool operator >(SubscriptionId left, SubscriptionId right) => left.Value > right.Value;
    public static bool operator <=(SubscriptionId left, SubscriptionId right) => left.Value >= right.Value;
    public static bool operator >=(SubscriptionId left, SubscriptionId right) => left.Value >= right.Value;

    public int CompareTo(SubscriptionId other) => Value.CompareTo(other.Value);
    public bool Equals(SubscriptionId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}