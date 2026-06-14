using Beatport2Rss.ReleaseCollector.Domain.Common;
using Beatport2Rss.ReleaseCollector.Domain.Releases;
using Beatport2Rss.ReleaseCollector.Domain.Subscriptions;
using Beatport2Rss.ReleaseCollector.Domain.Tracks;

namespace Beatport2Rss.ReleaseCollector.Domain;

internal static class ExceptionMessages
{
    public const string BeatportIdInvalid = $"{nameof(BeatportId)} must be positive";

    public const string BeatportSlugEmpty = $"{nameof(BeatportSlug)} cannot be empty";
    public const string BeatportSlugTooLong = $"{nameof(BeatportSlug)} is too long";

    public const string CatalogNumberEmpty = $"{nameof(CatalogNumber)} cannot be empty";
    public const string CatalogNumberTooLong = $"{nameof(CatalogNumber)} is too long";

    public const string MixNameEmpty = $"{nameof(MixName)} cannot be empty";
    public const string MixNameTooLong = $"{nameof(MixName)} is too long";

    public const string ReleaseIdEmpty = $"{nameof(ReleaseId)} cannot be empty";

    public const string ReleaseNameEmpty = $"{nameof(ReleaseName)} cannot be empty";
    public const string ReleaseNameTooLong = $"{nameof(ReleaseName)} is too long";

    public const string SubscriptionIdEmpty = $"{nameof(SubscriptionId)} cannot be empty";

    public const string SubscriptionNameEmpty = $"{nameof(SubscriptionName)} cannot be empty";
    public const string SubscriptionNameTooLong = $"{nameof(SubscriptionName)} too long";

    public const string TrackIdEmpty = $"{nameof(TrackId)} cannot be empty";

    public const string TrackNameEmpty = $"{nameof(TrackName)} cannot be empty";
    public const string TrackNameTooLong = $"{nameof(TrackName)} is too long";
}