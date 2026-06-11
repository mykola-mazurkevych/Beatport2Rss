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
    string? Country,
    int SubscribersCount,
    IEnumerable<SubscriptionTagResponse> Tags)
{
    public static SubscriptionPaginableResponse Create(SubscriptionPaginableDto dto) =>
        new(dto.Id.Value,
            dto.Name.Value,
            dto.Slug.Value,
            dto.BeatportType,
            dto.BeatportUri,
            dto.ImageUri,
            dto.Country?.Value,
            dto.SubscribersCount,
            dto.Tags.Select(SubscriptionTagResponse.Create));
}