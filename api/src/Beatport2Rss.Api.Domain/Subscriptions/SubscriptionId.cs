using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Api.Domain.Subscriptions;

public readonly record struct SubscriptionId :
    IId<SubscriptionId>
{
    private SubscriptionId(Guid value) => Value = value;

    public Guid Value { get; }

    public static SubscriptionId Create(Guid value) => new(value);

    public static bool operator <(SubscriptionId left, SubscriptionId right) => left.Value < right.Value;
    public static bool operator >(SubscriptionId left, SubscriptionId right) => left.Value > right.Value;
    public static bool operator <=(SubscriptionId left, SubscriptionId right) => left.Value >= right.Value;
    public static bool operator >=(SubscriptionId left, SubscriptionId right) => left.Value >= right.Value;

    public int CompareTo(SubscriptionId other) => Value.CompareTo(other.Value);
    public bool Equals(SubscriptionId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}