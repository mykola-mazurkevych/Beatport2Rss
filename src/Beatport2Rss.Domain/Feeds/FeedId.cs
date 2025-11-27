using Ardalis.GuardClauses;

using Beatport2Rss.SharedKernel;
using Beatport2Rss.SharedKernel.Constants;

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

    public override string ToString() => Value.ToString();
}