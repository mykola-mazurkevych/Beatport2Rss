using Beatport2Rss.Api.Application.ReadModels.EmailConfirmations;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations;

public interface IConfirmationEmailSender
{
    Task SendAsync(EmailConfirmationReadModel confirmation, CancellationToken cancellationToken);
}
