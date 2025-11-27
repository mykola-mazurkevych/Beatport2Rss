#pragma warning disable CA1031 // Do not catch general exception types

using System.Net.Mail;

using Ardalis.GuardClauses;

using Beatport2Rss.SharedKernel;
using Beatport2Rss.SharedKernel.Constants;

namespace Beatport2Rss.Domain.Users;

public readonly record struct EmailAddress : IValueObject
{
    private const int MaxLength = 100;

    private EmailAddress(string value) => Value = value;

    public string Value { get; }

    public static EmailAddress Create(string? value)
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

    public override string ToString() => Value;

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public bool Equals(EmailAddress other) => StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

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