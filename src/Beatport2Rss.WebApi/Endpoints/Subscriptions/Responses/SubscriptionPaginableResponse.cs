using Beatport2Rss.Application.Dtos.Subscriptions;
using Beatport2Rss.Domain.Subscriptions;

namespace Beatport2Rss.WebApi.Endpoints.Subscriptions.Responses;

internal sealed record SubscriptionPaginableResponse(
    int Id,
    string Name,
    string Slug,
    BeatportSubscriptionType BeatportType,
    Uri BeatportUri,
    Uri ImageUri,
    IEnumerable<SubscriptionTagResponse> Tags,
    DateTimeOffset? RefreshedAt)
{
    public static SubscriptionPaginableResponse Create(SubscriptionPaginableDto dto) =>
        new(dto.Id.Value,
            dto.Name.Value,
            dto.Slug.Value,
            dto.BeatportType,
            dto.BeatportUri,
            dto.ImageUri,
            dto.Tags.Select(SubscriptionTagResponse.Create),
            dto.RefreshedAt);
}