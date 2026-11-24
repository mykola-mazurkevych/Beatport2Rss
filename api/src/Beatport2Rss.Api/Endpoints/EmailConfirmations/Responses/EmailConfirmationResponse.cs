using Beatport2Rss.Api.Application.Dtos.EmailConfirmations;

namespace Beatport2Rss.Api.Endpoints.EmailConfirmations.Responses;

internal sealed record EmailConfirmationResponse(
    Guid Id,
    bool IsExpired,
    bool IsConfirmed)
{
    public static EmailConfirmationResponse Create(EmailConfirmationDto dto) =>
        new(dto.Id.Value,
            dto.IsExpired,
            dto.IsConfirmed);
}