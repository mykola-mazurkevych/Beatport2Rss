using Beatport2Rss.Application.Dtos.Subscriptions;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.WebApi.Endpoints.Subscriptions.Responses;

internal sealed record SubscriptionResponse(
    int Id,
    string Name,
    BeatportSubscriptionType BeatportType,
    int BeatportId,
    string BeatportSlug,
    Uri ImageUri,
    DateTimeOffset? RefreshedAt)
{
    public static SubscriptionResponse Create(SubscriptionDto dto) =>
        new(dto.Id.Value,
            dto.Name,
            dto.BeatportType,
            dto.BeatportId.Value,
            dto.BeatportSlug.Value,
            dto.ImageUri,
            dto.RefreshedAt);
}