using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Subscriptions;

public readonly record struct SubscriptionId : IValueObject
{
    private SubscriptionId(int value) => Value = value;

    public int Value { get; }

    public static SubscriptionId Create(int value)
    {
        Guard.Against.NegativeOrZero(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.SubscriptionIdInvalid));

        return new SubscriptionId(value);
    }

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}