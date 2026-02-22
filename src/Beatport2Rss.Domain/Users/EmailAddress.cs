using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.SharedKernel.Common;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Users;

public readonly record struct EmailAddress :
    IValueObject
{
    public const int MaxLength = 100;

    private EmailAddress(string value) => Value = value;

    public string Value { get; }

    public static EmailAddress Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException(ExceptionMessages.EmailAddressEmpty))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.EmailAddressTooLong))
            .MustBeEmailAddress(_ => new InvalidValueObjectValueException(ExceptionMessages.EmailAddressInvalid)));

    public bool Equals(EmailAddress other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}