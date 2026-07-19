using System.Globalization;

using Beatport2Rss.Common.SharedKernel.Exceptions;
using Beatport2Rss.Common.SharedKernel.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Builder.Domain.Common.ValueObjects;

public readonly record struct BeatportId :
    IValueObject
{
    private BeatportId(int value) => Value = value;

    public int Value { get; }

    public static BeatportId Create(int value) =>
        new(value.MustBeGreaterThan(0, (_, _) => new InvalidValueObjectValueException($"{nameof(BeatportId)} must be a positive value")));

    public bool Equals(BeatportId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}