using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Feeds;

public readonly record struct FeedId : IValueObject, IComparable<FeedId>
{
    private FeedId(Guid value) =>
        Value = value;

    public Guid Value { get; }

    public static FeedId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException(ExceptionMessages.FeedIdEmpty)));

    public static bool operator ==(FeedId left, Guid right) => left.Value == right;
    public static bool operator !=(FeedId left, Guid right) => left.Value != right;
    public static bool operator ==(Guid left, FeedId right) => left == right.Value;
    public static bool operator !=(Guid left, FeedId right) => left != right.Value;
    public static bool operator <(FeedId left, FeedId right) => left.Value < right.Value;
    public static bool operator <=(FeedId left, FeedId right) => left.Value <= right.Value;
    public static bool operator >(FeedId left, FeedId right) => left.Value > right.Value;
    public static bool operator >=(FeedId left, FeedId right) => left.Value >= right.Value;

    public static implicit operator Guid(FeedId feedId) => feedId.Value;
    public static implicit operator string(FeedId feedId) => feedId.ToString();

    public override int GetHashCode() =>
        Value.GetHashCode();

    public override string ToString() =>
        Value.ToString();

    public int CompareTo(FeedId other) =>
        Value.CompareTo(other.Value);

    public bool Equals(FeedId other) =>
        Value == other.Value;

    public Guid ToGuid() =>
        Value;
}