using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using Beatport2Rss.Domain.Common.Constants;
using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.SharedKernel.Common;

using Light.GuardClauses;

namespace Beatport2Rss.Domain.Common.ValueObjects;

public readonly record struct BeatportId :
    IValueObject, IParsable<BeatportId>
{
    private BeatportId(int value) =>
        Value = value;

    public int Value { get; }

    public static BeatportId Create(int value) =>
        new(value.MustBeGreaterThan(0, (_, _) => new InvalidValueObjectValueException(ExceptionMessages.BeatportIdInvalid)));

    public static BeatportId Parse(string s, IFormatProvider? provider) =>
        Create(int.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out BeatportId result)
    {
        if (int.TryParse(s, provider, out var value) && value > 0)
        {
            result = Create(value);
            return true;
        }

        result = default;
        return false;
    }

    public bool Equals(BeatportId other) => Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}