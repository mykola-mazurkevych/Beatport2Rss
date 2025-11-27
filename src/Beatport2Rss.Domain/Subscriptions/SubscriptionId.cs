using Ardalis.GuardClauses;

using Beatport2Rss.SharedKernel;
using Beatport2Rss.SharedKernel.Constants;

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