using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.SharedKernel.Common;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Feeds;

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
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException(ExceptionMessages.FeedIdEmpty)));

    public int CompareTo(FeedId other) => Value.CompareTo(other.Value);
    public bool Equals(FeedId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}