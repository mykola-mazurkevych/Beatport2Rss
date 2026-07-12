using Beatport2Rss.Api.Application.Dtos.Subscriptions;
using Beatport2Rss.Api.Domain.Subscriptions;

namespace Beatport2Rss.Api.Endpoints.Subscriptions.Responses;

internal sealed record SubscriptionResponse(
    Guid Id,
    SubscriptionType Type,
    string Name,
    string Slug,
    Uri BeatportUri,
    Uri ImageUri,
    string? Country,
    IEnumerable<SubscriptionTagResponse> Tags)
{
    public static SubscriptionResponse Create(SubscriptionDto dto) =>
        new(dto.Id.Value,
            dto.Type,
            dto.Name.Value,
            dto.Slug.Value,
            dto.BeatportUri,
            dto.ImageUri,
            dto.Country?.Value,
            dto.Tags.Select(SubscriptionTagResponse.Create));
}