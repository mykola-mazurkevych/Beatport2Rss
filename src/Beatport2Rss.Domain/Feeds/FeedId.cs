using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Feeds;

public readonly record struct FeedId : IValueObject
{
    private FeedId(Guid value) => Value = value;

    public Guid Value { get; }

    public static FeedId Create(Guid value)
    {
        Guard.Against.Default(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.FeedIdEmpty));

        return new FeedId(value);
    }

    public bool Equals(FeedId other) => Value == other.Value;

    public static bool operator ==(FeedId left, Guid right) => left.Value == right;
    public static bool operator !=(FeedId left, Guid right) => left.Value != right;
    public static bool operator ==(Guid left, FeedId right) => left == right.Value;
    public static bool operator !=(Guid left, FeedId right) => left != right.Value;

    public static implicit operator Guid(FeedId value) => value.Value;

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();

    public Guid ToGuid() => Value;
}