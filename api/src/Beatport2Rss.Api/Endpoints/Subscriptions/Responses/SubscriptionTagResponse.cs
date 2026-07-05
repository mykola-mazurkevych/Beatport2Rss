using Beatport2Rss.Api.Application.Dtos.Subscriptions;

namespace Beatport2Rss.Api.Endpoints.Subscriptions.Responses;

internal sealed record SubscriptionTagResponse(
    string Name,
    string Slug)
{
    public static SubscriptionTagResponse Create(SubscriptionTagDto dto) =>
        new(dto.Name.Value,
            dto.Slug.Value);
}