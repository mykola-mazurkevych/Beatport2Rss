using Beatport2Rss.Common.SharedKernel.Exceptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Api.Domain.Feeds;

public readonly record struct FeedId :
    IId<FeedId>
{
    private FeedId(Guid value) => Value = value;

    public Guid Value { get; }

    public static bool operator <(FeedId left, FeedId right) => left.Value < right.Value;
    public static bool operator >(FeedId left, FeedId right) => left.Value > right.Value;
    public static bool operator <=(FeedId left, FeedId right) => left.Value <= right.Value;
    public static bool operator >=(FeedId left, FeedId right) => left.Value >= right.Value;

    public static FeedId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException($"{nameof(FeedId)} cannot be empty")));

    public int CompareTo(FeedId other) => Value.CompareTo(other.Value);
    public bool Equals(FeedId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}