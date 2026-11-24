using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.Domain.Users;

namespace Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;

public interface IEmailConfirmationCommandRepository
{
    Task AddAsync(EmailConfirmation confirmation, CancellationToken cancellationToken = default);
    Task<EmailConfirmation?> FindAsync(EmailConfirmationId id, CancellationToken cancellationToken = default);
    Task<UserId?> FindUserIdAsync(EmailConfirmationId id, CancellationToken cancellationToken = default);
}