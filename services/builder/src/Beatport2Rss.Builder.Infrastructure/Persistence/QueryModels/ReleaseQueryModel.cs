namespace Beatport2Rss.Builder.Infrastructure.Persistence.QueryModels;

internal sealed record ReleaseQueryModel(
    Guid Id,
    int BeatportId,
    string BeatportSlug,
    DateTimeOffset CreatedAt,
    string Name,
    Uri ImageUri,
    string CatalogNumber,
    DateOnly ReleaseDate,
    ReleaseContributorQueryModel Label,
    IReadOnlyCollection<ReleaseContributorQueryModel> Artists,
    IReadOnlyCollection<ReleaseTrackQueryModel> Tracks,
    IReadOnlyCollection<string> Categories);

internal sealed record ReleaseContributorQueryModel(
    Guid SubscriptionId,
    string Name,
    IReadOnlyCollection<string> Tags);

internal sealed record ReleaseTrackQueryModel(
    int Number,
    string Name,
    string MixName,
    TimeSpan Length,
    IReadOnlyCollection<ReleaseContributorQueryModel> Artists);
