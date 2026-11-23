using Beatport2Rss.Api.Domain.EmailConfirmations;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations;

public interface IConfirmationEmailOutbox
{
    ValueTask EnqueueAsync(EmailConfirmationId id);
}
