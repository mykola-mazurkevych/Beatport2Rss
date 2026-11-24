using Beatport2Rss.Api.Application.Dtos.EmailConfirmations;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.FluentResults.Extensions;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations.Queries;

public sealed record GetEmailConfirmationQuery(EmailConfirmationId Id) :
    IQuery<Result<EmailConfirmationDto>>;

internal sealed class GetEmailConfirmationQueryHandler(
    IEmailConfirmationQueryRepository emailConfirmationQueryRepository,
    IClock clock) :
    IQueryHandler<GetEmailConfirmationQuery, Result<EmailConfirmationDto>>
{
    public async ValueTask<Result<EmailConfirmationDto>> Handle(
        GetEmailConfirmationQuery query,
        CancellationToken cancellationToken)
    {
        var emailConfirmation = await emailConfirmationQueryRepository.FindAsync(query.Id, cancellationToken);
        if (emailConfirmation is null)
        {
            return Result.NotFound("Email confirmation not found");
        }

        return new EmailConfirmationDto(
            emailConfirmation.Id,
            IsExpired: emailConfirmation.ExpiresAt <= clock.UtcNow,
            IsConfirmed: emailConfirmation.ConfirmedAt.HasValue);
    }
}