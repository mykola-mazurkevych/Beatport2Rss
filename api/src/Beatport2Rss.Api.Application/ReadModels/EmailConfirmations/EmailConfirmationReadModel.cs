using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.Domain.Common;

namespace Beatport2Rss.Api.Application.ReadModels.EmailConfirmations;

public sealed record EmailConfirmationReadModel(
    EmailConfirmationId Id,
    DateTimeOffset CreatedAt,
    EmailAddress EmailAddress,
    DateTimeOffset ExpiresAt,
    DateTimeOffset? ConfirmedAt);
