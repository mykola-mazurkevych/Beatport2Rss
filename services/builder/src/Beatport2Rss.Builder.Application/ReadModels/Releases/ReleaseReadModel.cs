using Beatport2Rss.Builder.Domain.Releases;
using Beatport2Rss.Builder.Domain.Tracks;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Releases;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.Domain.Tags;

namespace Beatport2Rss.Builder.Application.ReadModels.Releases;

public sealed record ReleaseReadModel(
    ReleaseId Id,
    BeatportId BeatportId,
    BeatportSlug BeatportSlug,
    DateTimeOffset CreatedAt,
    ReleaseName Name,
    Uri ImageUri,
    CatalogNumber CatalogNumber,
    DateOnly ReleaseDate,
    ContributorReadModel Label,
    IReadOnlyCollection<ContributorReadModel> Artists,
    IReadOnlyCollection<TrackReadModel> Tracks,
    IReadOnlyCollection<string> Categories);

public sealed record ContributorReadModel(
    SubscriptionId SubscriptionId,
    SubscriptionName Name,
    IReadOnlyCollection<TagName> Tags);

public sealed record TrackReadModel(
    int Number,
    TrackName Name,
    MixName MixName,
    TimeSpan Length,
    IReadOnlyCollection<ContributorReadModel> Artists);
