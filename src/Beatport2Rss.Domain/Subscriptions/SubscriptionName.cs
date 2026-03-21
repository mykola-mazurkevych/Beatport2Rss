using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.SharedKernel.Common;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Subscriptions;

public readonly record struct SubscriptionName :
    IValueObject
{
    public const int MaxLength = 200;

    private SubscriptionName(string value) => Value = value;

    public string Value { get; }

    public static SubscriptionName Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.SubscriptionNameEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.SubscriptionNameTooLong)));

    public bool Equals(SubscriptionName other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}