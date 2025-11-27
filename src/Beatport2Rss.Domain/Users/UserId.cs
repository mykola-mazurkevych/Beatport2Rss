using Ardalis.GuardClauses;

using Beatport2Rss.SharedKernel;
using Beatport2Rss.SharedKernel.Constants;

namespace Beatport2Rss.Domain.Users;

public readonly record struct UserId : IValueObject
{
    private UserId(Guid value) => Value = value;

    public Guid Value { get; }

    public static UserId Create(Guid value)
    {
        Guard.Against.Default(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.UserIdEmpty));

        return new UserId(value);
    }

    public override string ToString() => Value.ToString("D", System.Globalization.CultureInfo.InvariantCulture);
}