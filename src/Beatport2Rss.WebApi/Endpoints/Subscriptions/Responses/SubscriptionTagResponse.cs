using Beatport2Rss.Application.Dtos.Subscriptions;

namespace Beatport2Rss.WebApi.Endpoints.Subscriptions.Responses;

internal sealed record SubscriptionTagResponse(
    string Name,
    string Slug)
{
    public static SubscriptionTagResponse Create(SubscriptionTagDto dto) =>
        new(dto.Name.Value,
            dto.Slug.Value);
}