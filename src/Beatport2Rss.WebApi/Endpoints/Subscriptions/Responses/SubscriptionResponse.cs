using Beatport2Rss.Application.Dtos.Subscriptions;

namespace Beatport2Rss.WebApi.Endpoints.Subscriptions.Responses;

internal sealed record SubscriptionResponse(
    int Id,
    string Name,
    string Slug,
    Uri BeatportUri,
    Uri ImageUri,
    IEnumerable<SubscriptionTagResponse> Tags,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? RefreshedAt)
{
    public static SubscriptionResponse Create(SubscriptionDto dto) =>
        new(dto.Id.Value,
            dto.Name.Value,
            dto.Slug.Value,
            dto.BeatportUri,
            dto.ImageUri,
            dto.Tags.Select(SubscriptionTagResponse.Create),
            dto.CreatedAt,
            dto.RefreshedAt);
}