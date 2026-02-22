using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.SharedKernel.Common;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Common.ValueObjects;

public readonly record struct AccessToken :
    IValueObject
{
    private AccessToken(string value, AccessTokenType type)
    {
        Value = value;
        Type = type;
    }

    public string Value { get; }
    public AccessTokenType Type { get; }

    public static AccessToken Create([NotNull] string? value, AccessTokenType type) =>
        new(value.MustNotBeNullOrWhiteSpace(), type.MustBeValidEnumValue());

    public static AccessToken Bearer([NotNull] string? value) =>
        Create(value, AccessTokenType.Bearer);

    public bool Equals(AccessToken other) => StringComparer.Ordinal.Equals(Value, other.Value) && Type == other.Type;
    public override int GetHashCode() => HashCode.Combine(Value, Type);
    public override string ToString() => $"{Type} {Value}";
}