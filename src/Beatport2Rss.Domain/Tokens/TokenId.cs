using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Tokens;

public readonly record struct TokenId : IValueObject
{
    private TokenId(Guid value) => Value = value;

    public Guid Value { get; }

    public static TokenId Create(Guid value) =>
        new(value.MustNotBeEmpty(() => new InvalidValueObjectValueException(ExceptionMessages.TokenIdEmpty)));

    public bool Equals(TokenId other) => Value == other.Value;

    public static bool operator ==(TokenId left, Guid right) => left.Value == right;
    public static bool operator !=(TokenId left, Guid right) => left.Value != right;
    public static bool operator ==(Guid left, TokenId right) => left == right.Value;
    public static bool operator !=(Guid left, TokenId right) => left != right.Value;

    public static implicit operator Guid(TokenId value) => value.Value;

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();

    public Guid ToGuid() => Value;
}