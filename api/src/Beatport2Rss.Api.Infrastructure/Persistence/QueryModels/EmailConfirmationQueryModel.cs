using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.SharedKernel.Interfaces;

namespace Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

internal sealed record EmailConfirmationQueryModel(
    EmailConfirmationId Id,
    DateTimeOffset ExpiresAt,
    DateTimeOffset? ConfirmedAt) :
    IQueryModel<EmailConfirmationId>;