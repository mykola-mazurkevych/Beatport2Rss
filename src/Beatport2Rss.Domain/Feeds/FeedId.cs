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

    public override string ToString() => Value.ToString("D", System.Globalization.CultureInfo.InvariantCulture);
}