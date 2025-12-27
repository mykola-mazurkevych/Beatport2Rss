#pragma warning disable CA1819 // Properties should not return arrays

using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Sessions;

public readonly record struct RefreshTokenHash : IValueObject
{
    private RefreshTokenHash(byte[] value) => Value = value;

    public byte[] Value { get; }

    public static RefreshTokenHash Create([NotNull] byte[]? value) =>
        new(value.MustNotBeNullOrEmpty(_ => new InvalidValueObjectValueException(ExceptionMessages.RefreshTokenHashEmpty)));

    public bool Equals(RefreshTokenHash other) => Value.SequenceEqual(other.Value);

    public static bool operator ==(RefreshTokenHash left, byte[] right) => left.Value.SequenceEqual(right);
    public static bool operator !=(RefreshTokenHash left, byte[] right) => !left.Value.SequenceEqual(right);
    public static bool operator ==(byte[] left, RefreshTokenHash right) => left.SequenceEqual(right.Value);
    public static bool operator !=(byte[] left, RefreshTokenHash right) => !left.SequenceEqual(right.Value);

    public static implicit operator byte[](RefreshTokenHash refreshTokenHash) => refreshTokenHash.Value;

    public override int GetHashCode() => GetHashCodeInternal();
    public override string ToString() => Convert.ToBase64String(Value);

    public byte[] ToByteArray() => Value;

    private int GetHashCodeInternal()
    {
        unchecked
        {
            return Value.Aggregate(17, (current, b) => current * 31 + b);
        }
    }
}