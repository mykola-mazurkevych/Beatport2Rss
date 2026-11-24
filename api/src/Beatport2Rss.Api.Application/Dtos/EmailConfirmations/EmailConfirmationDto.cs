using Beatport2Rss.Api.Domain.EmailConfirmations;

namespace Beatport2Rss.Api.Application.Dtos.EmailConfirmations;

public sealed record EmailConfirmationDto(
    EmailConfirmationId Id,
    bool IsExpired,
    bool IsConfirmed);