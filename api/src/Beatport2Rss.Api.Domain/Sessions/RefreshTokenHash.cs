using System.Diagnostics.CodeAnalysis;

using Beatport2Rss.Api.Domain.Common.Constants;
using Beatport2Rss.Common.SharedKernel.Exceptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Api.Domain.Sessions;

public readonly record struct RefreshTokenHash :
    IValueObject
{
    private readonly byte[] _value;

    private RefreshTokenHash(byte[] value) => _value = value;

    public IReadOnlyCollection<byte> Value => _value.AsReadOnly();

    public static RefreshTokenHash Create([NotNull] byte[]? value) =>
        new(value.MustNotBeNullOrEmpty(_ => new InvalidValueObjectValueException(ExceptionMessages.RefreshTokenHashEmpty)));

    public bool Equals(RefreshTokenHash other) => Value.SequenceEqual(other.Value);
    public override int GetHashCode() => GetHashCodeInternal();
    public override string ToString() => Convert.ToBase64String(_value);

    private int GetHashCodeInternal()
    {
        unchecked
        {
            return _value.Aggregate(17, (current, b) => current * 31 + b);
        }
    }
}