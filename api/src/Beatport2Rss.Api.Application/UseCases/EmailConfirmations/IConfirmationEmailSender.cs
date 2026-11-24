using Beatport2Rss.Api.Domain.EmailConfirmations;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations;

public interface IConfirmationEmailSender
{
    Task SendAsync(EmailConfirmation confirmation, CancellationToken cancellationToken);
}
