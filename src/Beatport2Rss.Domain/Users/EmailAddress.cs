#pragma warning disable CA1031 // Do not catch general exception types

using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;

using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Users;

public readonly record struct EmailAddress : IValueObject
{
    public const int MaxLength = 100;

    private EmailAddress(string value) => Value = value;

    public string Value { get; }

    public static EmailAddress Create([NotNull] string? value)
    {
        Guard.Against.NullOrWhiteSpace(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.EmailAddressEmpty));
        Guard.Against.StringTooLong(value, MaxLength,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.EmailAddressTooLong));
        Guard.Against.InvalidInput(value,
            nameof(value),
            predicate: EmailAddressIsValid,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.EmailAddressInvalid));

        return new EmailAddress(value);
    }

    public bool Equals(EmailAddress other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

    public static bool operator ==(EmailAddress left, string? right) => StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);
    public static bool operator !=(EmailAddress left, string? right) => !StringComparer.OrdinalIgnoreCase.Equals(left.Value, right);
    public static bool operator ==(string? left, EmailAddress right) => StringComparer.OrdinalIgnoreCase.Equals(left, right.Value);
    public static bool operator !=(string? left, EmailAddress right) => !StringComparer.OrdinalIgnoreCase.Equals(left, right.Value);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    public override string ToString() => Value;

    private static bool EmailAddressIsValid(string value)
    {
        bool isValid;

        try
        {
            var mailAddress = new MailAddress(value);

            isValid = string.Equals(mailAddress.Address, value, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            isValid = false;
        }

        return isValid;
    }
}