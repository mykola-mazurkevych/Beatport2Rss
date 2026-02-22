using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.SharedKernel.Common;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Tokens;

public readonly record struct TokenId :
    IId<TokenId>
{
    private TokenId(Guid value) => Value = value;

    public Guid Value { get; }

    public static bool operator <(TokenId left, TokenId right) => left.Value < right.Value;
    public static bool operator >(TokenId left, TokenId right) => left.Value > right.Value;
    public static bool operator <=(TokenId left, TokenId right) => left.Value <= right.Value;
    public static bool operator >=(TokenId left, TokenId right) => left.Value >= right.Value;

    public static TokenId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException(ExceptionMessages.TokenIdEmpty)));

    public int CompareTo(TokenId other) => Value.CompareTo(other.Value);
    public bool Equals(TokenId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}