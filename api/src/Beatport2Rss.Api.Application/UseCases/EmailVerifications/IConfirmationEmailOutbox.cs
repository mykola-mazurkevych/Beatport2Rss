namespace Beatport2Rss.Api.Application.UseCases.EmailVerifications;

public interface IConfirmationEmailOutbox
{
    ValueTask EnqueueAsync(string identifier);
}
