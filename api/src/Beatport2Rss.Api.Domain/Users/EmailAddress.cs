using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Common.SharedKernel.Exceptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Api.Domain.Users;

public readonly record struct EmailAddress :
    IValueObject
{
    public const int MaxLength = 100;

    private EmailAddress(string value) => Value = value;

    public string Value { get; }

    public static EmailAddress Create([NotNull] string? value) =>
        new(value
            .MustNotBeNullOrWhiteSpace(_ => new InvalidValueObjectValueException($"{nameof(EmailAddress)} cannot be empty"))
            .MustBeShorterThanOrEqualTo(MaxLength, (_, _) => new InvalidValueObjectValueException($"{nameof(EmailAddress)} is too long"))
            .MustBeEmailAddress(_ => new InvalidValueObjectValueException($"{nameof(EmailAddress)} is not valid")));

    public bool Equals(EmailAddress other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;
}