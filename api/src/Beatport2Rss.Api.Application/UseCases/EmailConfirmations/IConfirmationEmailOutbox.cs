namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations;

public interface IConfirmationEmailOutbox
{
    ValueTask EnqueueAsync(string identifier);
}
