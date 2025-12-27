using System.Globalization;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.Domain.Common.Interfaces;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Common.ValueObjects;

public readonly record struct BeatportId : IValueObject
{
    private BeatportId(int value) => Value = value;

    public int Value { get; }

    public static BeatportId Create(int value) =>
        new(value.MustBeGreaterThan(0, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.BeatportIdInvalid)));

    public bool Equals(BeatportId other) => Value == other.Value;

    public static bool operator ==(BeatportId left, int right) => left.Value == right;
    public static bool operator !=(BeatportId left, int right) => left.Value != right;
    public static bool operator ==(int left, BeatportId right) => left == right.Value;
    public static bool operator !=(int left, BeatportId right) => left != right.Value;

    public static implicit operator int(BeatportId beatportId) => beatportId.Value;

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    public int ToInt32() => Value;
}