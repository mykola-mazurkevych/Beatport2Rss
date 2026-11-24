using Beatport2Rss.Api.Domain.EmailConfirmations;

namespace Beatport2Rss.Api.Application.ReadModels.EmailConfirmations;

public sealed record EmailConfirmationReadModel(
    EmailConfirmationId Id,
    DateTimeOffset ExpiresAt,
    DateTimeOffset? ConfirmedAt);
