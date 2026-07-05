using Beatport2Rss.Collector.Domain.Artists;
using Beatport2Rss.Collector.Domain.Common.ValueObjects;
using Beatport2Rss.Collector.Domain.Labels;
using Beatport2Rss.Collector.Domain.Releases;
using Beatport2Rss.Collector.Domain.Tracks;

namespace Beatport2Rss.Collector.Domain.Common.Constants;

internal static class ExceptionMessages
{
    public const string ArtistIdEmpty = $"{nameof(ArtistId)} cannot be empty";

    public const string ArtistNameEmpty = $"{nameof(ArtistName)} cannot be empty";
    public const string ArtistNameTooLong = $"{nameof(ArtistName)} too long";

    public const string BeatportIdInvalid = $"{nameof(BeatportId)} must be positive";

    public const string BeatportSlugEmpty = $"{nameof(BeatportSlug)} cannot be empty";
    public const string BeatportSlugTooLong = $"{nameof(BeatportSlug)} is too long";

    public const string CatalogNumberEmpty = $"{nameof(CatalogNumber)} cannot be empty";
    public const string CatalogNumberTooLong = $"{nameof(CatalogNumber)} is too long";

    public const string LabelIdEmpty = $"{nameof(LabelId)} cannot be empty";

    public const string LabelNameEmpty = $"{nameof(LabelName)} cannot be empty";
    public const string LabelNameTooLong = $"{nameof(LabelName)} too long";

    public const string MixNameEmpty = $"{nameof(MixName)} cannot be empty";
    public const string MixNameTooLong = $"{nameof(MixName)} is too long";

    public const string ReleaseIdEmpty = $"{nameof(ReleaseId)} cannot be empty";

    public const string ReleaseNameEmpty = $"{nameof(ReleaseName)} cannot be empty";
    public const string ReleaseNameTooLong = $"{nameof(ReleaseName)} is too long";

    public const string TrackIdEmpty = $"{nameof(TrackId)} cannot be empty";

    public const string TrackNameEmpty = $"{nameof(TrackName)} cannot be empty";
    public const string TrackNameTooLong = $"{nameof(TrackName)} is too long";
}