using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

namespace Beatport2Rss.Domain.Tokens;

public readonly record struct TokenId : IValueObject
{
    private TokenId(int value) => Value = value;

    public int Value { get; }

    public static TokenId Create(int value)
    {
        Guard.Against.NegativeOrZero(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.TokenIdInvalid));

        return new TokenId(value);
    }

    public bool Equals(TokenId other) => Value == other.Value;

    public static bool operator ==(TokenId left, int right) => left.Value == right;
    public static bool operator !=(TokenId left, int right) => left.Value != right;
    public static bool operator ==(int left, TokenId right) => left == right.Value;
    public static bool operator !=(int left, TokenId right) => left != right.Value; 

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
}