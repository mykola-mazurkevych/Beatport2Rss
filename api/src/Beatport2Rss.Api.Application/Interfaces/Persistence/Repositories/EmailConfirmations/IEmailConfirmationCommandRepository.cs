using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.Miscellaneous.Interfaces;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories.EmailConfirmations;

public interface IEmailConfirmationCommandRepository
{
    Task AddAsync(EmailConfirmation confirmation, CancellationToken cancellationToken = default);
    Task<bool> ConfirmAsync(EmailConfirmationId id, IClock clock, CancellationToken cancellationToken = default);
}
