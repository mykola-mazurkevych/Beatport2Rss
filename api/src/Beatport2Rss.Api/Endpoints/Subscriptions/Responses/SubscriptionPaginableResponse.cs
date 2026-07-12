using Beatport2Rss.Api.Application.Dtos.Subscriptions;
using Beatport2Rss.Api.Domain.Subscriptions;

namespace Beatport2Rss.Api.Endpoints.Subscriptions.Responses;

internal sealed record SubscriptionPaginableResponse(
    Guid Id,
    SubscriptionType Type,
    string Name,
    string Slug,
    Uri BeatportUri,
    Uri ImageUri,
    string? Country,
    int SubscribersCount,
    IEnumerable<SubscriptionTagResponse> Tags)
{
    public static SubscriptionPaginableResponse Create(SubscriptionPaginableDto dto) =>
        new(dto.Id.Value,
            dto.Type,
            dto.Name.Value,
            dto.Slug.Value,
            dto.BeatportUri,
            dto.ImageUri,
            dto.Country?.Value,
            dto.SubscribersCount,
            dto.Tags.Select(SubscriptionTagResponse.Create));
}