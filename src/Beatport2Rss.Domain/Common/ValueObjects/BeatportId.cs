using System.Globalization;

using Ardalis.GuardClauses;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;

namespace Beatport2Rss.Domain.Common.ValueObjects;

public readonly record struct BeatportId
{
    private BeatportId(int value) => Value = value;

    public int Value { get; }

    public static BeatportId Create(int value)
    {
        Guard.Against.NegativeOrZero(value,
            exceptionCreator: () => new InvalidValueObjectValueException(ExceptionMessages.BeatportIdInvalid));

        return new BeatportId(value);
    }

    public bool Equals(BeatportId other) => Value == other.Value;

    public static bool operator ==(BeatportId left, int right) => left.Value == right;
    public static bool operator !=(BeatportId left, int right) => left.Value != right;
    public static bool operator ==(int left, BeatportId right) => left == right.Value;
    public static bool operator !=(int left, BeatportId right) => left != right.Value;

    public static implicit operator int(BeatportId value) => value.Value;

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    public int ToInt32() => Value;
}